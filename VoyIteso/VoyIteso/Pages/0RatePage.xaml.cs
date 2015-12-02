using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using VoyIteso.Class;
using VoyIteso.Pages.ChatStuff;
using VoyIteso.Pages._0RateComponents;

namespace VoyIteso.Pages
{
    public partial class RatePage : PhoneApplicationPage
    {


        public static int idaventon = 0;//para saber cual calificar.
        public static string nombreConductor = "";//.
        public static int idconductor = 0;//para sacar la foto.



        private ChatView myChat;
        ObservableCollection<object> messages = new ObservableCollection<object>();
        ObservableCollection<string> dummyMessages = new ObservableCollection<string>();
        public static string key;
        public static bool ok = false;
        private RealizoOK PrimeraPregunta;
        private SegundoComp SegundoComponente;
        private bool successful;

        public RatePage()
        {
            ok = false;
            InitializeComponent();
            successful = false;
            InitLayout();

        }

        private DispatcherTimer timer;
        private async void InitLayout()
        {

            try
            {

                //encabezado
                TextBlock encabezado = new TextBlock();
                encabezado.Text = "¿Se realizó tu aventón con";
                encabezado.Text += " " + nombreConductor + "?";
                encabezado.FontSize = 42;
                encabezado.HorizontalAlignment = HorizontalAlignment.Center;
                encabezado.TextWrapping = TextWrapping.Wrap;
                encabezado.Width = 440;
                encabezado.Margin = new System.Windows.Thickness(0, 0, 0, 568);
                ContentPanel.Children.Add(encabezado);

                //imagen de usuario
                var imag = ApiConnector.Instance.GetUserImageById(idconductor);
                Image driverImage = new Image();
                driverImage.Margin = new System.Windows.Thickness(0, 132, 10, 359);
                driverImage.Source = imag;
                ContentPanel.Children.Add(driverImage);

                /////
                //el grid de botones (contenedor de los botones de si y no.)
                Grid contenedor = new Grid();
                contenedor.Margin = new Thickness(10, 354, 10, 10);


                //boton de si.
                Button sibutton = new Button();
                sibutton.Content = "SI";
                sibutton.Margin = new Thickness(10, 10, 234, 237);
                sibutton.Click += siClicked;
                //boton de no.
                Button nobutton = new Button();
                nobutton.Content = "NO";
                nobutton.Margin = new Thickness(249, 10, 5, 237);
                nobutton.Click += noClicked;

                contenedor.Children.Add(sibutton);
                contenedor.Children.Add(nobutton);
                //////

                ContentPanel.Children.Add(contenedor);
            
            }
            catch (Exception)
            {

                MessageBox.Show("Hubo un problema con el servidor", "Ups! lo sentimos", MessageBoxButton.OK);
            }

        }

        private async void noClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                new Progress().showProgressIndicator(this, "procesando");
                var a = await ApiConnector.Instance.LiftRate(idaventon, successful ? 1 : 0, 0, 0, "");
                new Progress().hideProgressIndicator(this);
                if (a.estatus == 1)
                {
                    MessageBox.Show("Aventón calificado exitosamente.");

                }
                else
                {
                    MessageBox.Show("El aventón no se pudo calificar, intenta más tarde.", "Advertencia.", MessageBoxButton.OK);

                }

                ApplicationBarIconButton b = new ApplicationBarIconButton(new Uri("Images/icons/check.png", UriKind.Relative));
                b.Text = "Continuar";
                b.Click += cerrar_clicked;
                var appbar = new ApplicationBar();
                appbar.Buttons.Add(b);
                appbar.Opacity = 1.0;
                appbar.IsMenuEnabled = true;
                appbar.IsVisible = true;
                appbar.Mode = ApplicationBarMode.Minimized;

                ApplicationBar = appbar;
            }
            catch (Exception)
            {

                MessageBox.Show("Hubo un problema con el servidor", "Ups! lo sentimos", MessageBoxButton.OK);
            }

        }

        private void siClicked(object sender, RoutedEventArgs e)
        {
            loadNExtElement();
            successful = true;
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            if (ok)//le pico que SI se le dio el aventon.
            {
                loadNExtElement();
                successful = true;
                ok = false;
                timer.Stop();
            }
            else// le pico que NO se le dio el aventon. 
            {
                var a = await ApiConnector.Instance.LiftRate(idaventon, successful ? 1 : 0, 0, 0, "");

                if (a.estatus == 1)
                {
                    MessageBox.Show("Aventón calificado exitosamente.");
                }
                else
                {
                    MessageBox.Show("El aventón no se pudo calificar, intenta más tarde.", "Advertencia.", MessageBoxButton.OK);
                }
            }
            
        }

        private void loadNExtElement()
        {
            try
            {
                //delete content.
                //ContentPanel.Children.Remove(PrimeraPregunta);
                ContentPanel.Children.Clear();

                //add content.
                SegundoComponente = new SegundoComp();
                var i = ApiConnector.Instance.GetUserImageById(idconductor);
                SegundoComponente.DriverImage.Source = i;
                SegundoComponente.comotefue.Text += nombreConductor + "?";
                ContentPanel.Children.Add(SegundoComponente);

                ApplicationBarIconButton b = new ApplicationBarIconButton(new Uri("Images/icons/check.png", UriKind.Relative));
                b.Text = "Continuar";
                b.Click += cerrar_clicked;
                var appbar = new ApplicationBar();
                appbar.Buttons.Add(b);

                SegundoComponente.Calificacion.Value = 7;
                SegundoComponente.Puntualidad.Value = 7;
                appbar.Opacity = 1.0;
                appbar.IsMenuEnabled = true;
                appbar.IsVisible = true;
                appbar.Mode = ApplicationBarMode.Minimized;

                ApplicationBar = appbar;
            }
            catch (Exception)
            {

                MessageBox.Show("Hubo un problema con el servidor", "Ups! lo sentimos", MessageBoxButton.OK);
            }

        }

        private void cerrar_clicked(object sender, EventArgs e)
        {
            hasTuPedoJairo();

            NavigationService.RemoveBackEntry();
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }

        private async void hasTuPedoJairo()
        {
            try
            {
                var calificacion = Convert.ToInt32(SegundoComponente.Calificacion.Value);
                var puntialidad = Convert.ToInt32(SegundoComponente.Puntualidad.Value);
                var comentarios = SegundoComponente.CajaDeComentarios.Text;

                new Progress().showProgressIndicator(this, "procesando");
                var a = await ApiConnector.Instance.LiftRate(idaventon, successful ? 1 : 0, puntialidad, calificacion, comentarios);
                new Progress().hideProgressIndicator(this);
                if (a.estatus == 1)
                {
                    MessageBox.Show("Aventón calificado exitosamente.");
                }
                else
                {
                    MessageBox.Show("El aventón no se pudo calificar, intenta más tarde.", "Advertencia.", MessageBoxButton.OK);
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Hubo un problema con el servidor", "Ups! lo sentimos", MessageBoxButton.OK);
            }

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //base.OnBackKeyPress(e);

        }
    }
}