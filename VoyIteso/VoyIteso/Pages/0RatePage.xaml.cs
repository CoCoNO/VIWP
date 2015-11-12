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
            PrimeraPregunta = new RealizoOK();
            //ApiConnector.Instance.GetUserImageById(idconductor)
            var i= ApiConnector.Instance.GetUserImageById(idconductor);
            //PrimeraPregunta.DriverImage = new Image();
            PrimeraPregunta.DriverImage.Source = i;
            PrimeraPregunta.encabezado.Text += " " + nombreConductor + "?";
            ContentPanel.Children.Add(PrimeraPregunta);

            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            
            timer.Tick += OnTimerTick;
            timer.Start();

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
            //delete content.
            ContentPanel.Children.Remove(PrimeraPregunta);
            
            //add content.
            SegundoComponente = new SegundoComp();
            var i = ApiConnector.Instance.GetUserImageById(idconductor);
            SegundoComponente.DriverImage.Source = i;
            SegundoComponente.comotefue.Text += nombreConductor +"?";
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

        private void cerrar_clicked(object sender, EventArgs e)
        {
            hasTuPedoJairo();

            NavigationService.RemoveBackEntry();
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }

        private async void hasTuPedoJairo()
        {
            var calificacion = Convert.ToInt32(SegundoComponente.Calificacion.Value);
            var puntialidad = Convert.ToInt32(SegundoComponente.Puntualidad.Value);
            var comentarios = SegundoComponente.CajaDeComentarios.Text;

            var a = await ApiConnector.Instance.LiftRate(idaventon, successful ? 1 : 0, puntialidad, calificacion, comentarios);

            if (a.estatus==1)
            {
                MessageBox.Show("Aventón calificado exitosamente.");
            }
            else
            {
                MessageBox.Show("El aventón no se pudo calificar, intenta más tarde.","Advertencia.",MessageBoxButton.OK);
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