using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using VoyIteso.Class;
using VoyIteso.Pages.ShowRoutesComponents;

namespace VoyIteso.Pages
{
    public partial class ShowRoutes : PhoneApplicationPage
    {
        /// <summary>
        /// class fields
        /// </summary>

        enum appBarStateMachine { ITEM_SELECTED, ITEM_NOT_SELECTED  };

        private appBarStateMachine appBarSM;

        public ShowRoutes()
        {
            InitializeComponent();
            ListOfBoxes.SelectionChanged += listaSelectionChanged;
            //ListOfBoxes.MouseMove += listaMOusemoved;
            
            appBarSM=appBarStateMachine.ITEM_NOT_SELECTED;
            BuildAppBar();
        }

        private void listaMOusemoved(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Debug.WriteLine("mouse wheel moved");
            appBarSM=appBarStateMachine.ITEM_NOT_SELECTED;
            BuildAppBar();
        }



        private void BuildAppBar()
        {

            if (appBarSM == appBarStateMachine.ITEM_NOT_SELECTED)
            {
                ApplicationBar = new ApplicationBar();
                ApplicationBar.Mode = ApplicationBarMode.Default;
                ApplicationBar.Opacity = 1.0;
                ApplicationBar.IsMenuEnabled = true;
                ApplicationBar.IsVisible = true;

                //ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("Assets/refresh.png", UriKind.Relative));
                //a.Text = "agregar caja// pruebas";
                //a.Click += a_Click;
                //ApplicationBar.Buttons.Add(a);

                ApplicationBarIconButton b = new ApplicationBarIconButton(new Uri("Assets/add.png", UriKind.Relative));
                b.Text = "nueva ruta";
                b.Click += b_Click;
                ApplicationBar.Buttons.Add(b);
            }
            else
            {
                ApplicationBar = new ApplicationBar();
                ApplicationBar.Mode = ApplicationBarMode.Default;
                ApplicationBar.Opacity = 1.0;
                ApplicationBar.IsMenuEnabled = true;
                ApplicationBar.IsVisible = true;

                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("Images/icons/delete.png", UriKind.Relative));
                a.Text = "borrar";
                a.Click += a_Click;
                ApplicationBar.Buttons.Add(a);

                ApplicationBarIconButton b = new ApplicationBarIconButton(new Uri("Assets/add.png", UriKind.Relative));
                b.Text = "nueva ruta";
                b.Click += b_Click;
                ApplicationBar.Buttons.Add(b);

                ApplicationBarIconButton c = new ApplicationBarIconButton(new Uri("Images/icons/refresh.png", UriKind.Relative));
                c.Text = "repetir";
                c.Click += c_Click;
                ApplicationBar.Buttons.Add(c);
            }

            
            
        }

        private void c_Click(object sender, EventArgs e)
        {
            MessageBox.Show("implementar esto.");
        }

        /// <summary>
        /// estos dos campos son para las rutas y la ruta seleccionada.
        /// </summary>
        Rutes todasLasRutas = new Rutes();
        public Rutai RutaSeleccionadaRutai { get; set; }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var user = ApiConnector.Instance.ActiveUser;
            new Progress().showProgressIndicator(this,"cargando rutas");
            var rutas = await ApiConnector.Instance.RouteGetAllByUserID(Convert.ToInt32(user.profileID));
            new Progress().hideProgressIndicator(this);


            try
            {
                if (!(rutas.rutas.Count > todasLasRutas.rutas.Count))
                {
                    return;
                }
            }
            catch (Exception)
            {
                foreach (var rutai in rutas.rutas)
                {
                    var a = new Grid() { Height = 20 };
                    ListOfBoxes.Items.Add(a);
                    var b = new ShowRouteBox
                    {
                        HeaderLabel = {Text = rutai.persona_nombre},
                        BodyLabel =
                        {
                            Text =
                                rutai.fecha_inicio_formato.Substring(0, 2) + "-" +
                                rutai.fecha_inicio_formato.Substring(2, 2) + "-" +
                                rutai.fecha_inicio_formato.Substring(4) + " a las " + rutai.hora_llegada_formato + "\nDe: " +
                                rutai.texto_origen + "\nA: " + rutai.texto_destino
                        }
                    };
                    ListOfBoxes.Items.Add(b);
                    b.Tap += b_Tap;
                }
            }
            

            todasLasRutas = rutas;
            

        }

        ShowRouteBox tempBox = new ShowRouteBox();
        private Brush tempcol;
        void b_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            appBarSM = appBarStateMachine.ITEM_SELECTED;
            BuildAppBar();
            var index = ListOfBoxes.SelectedIndex - 1;
            //Debug.WriteLine(index/2);

            if (index % 2 != 0 || index < 0)
            {
                return;
            }
            var item = todasLasRutas.rutas[index / 2];
            //
            var a = (ShowRouteBox)ListOfBoxes.SelectedItem;
            RutaSeleccionadaRutai = item;
            if (a == tempBox)
            {
                return;
            }
            
            tempcol = a.Grid.Background;
            a.Grid.Background = new SolidColorBrush(Colors.Blue);
            tempBox = a;

            //MessageBox.Show(item.puntos_intermedios);
            //NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));//?key=value&key2=value
        }



        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
            while (true)
            {
                if (NavigationService.RemoveBackEntry() == null)
                {
                    break;
                }
            }

        }

        //a,.
        async private void a_Click(object sender, EventArgs e)
        {
            //var a = new Grid(){Height = 20};
            //ListOfBoxes.Items.Add(a);
            //ListOfBoxes.Items.Add(new ShowRouteBox());
            new Progress().showProgressIndicator(this, "borrando la ruta");
            var a = await ApiConnector.Instance.RouteDelete(RutaSeleccionadaRutai.ruta_id);
            new Progress().hideProgressIndicator(this);
            
            if (a.estatus == 1)
            {
                MessageBox.Show("ruta borrada");
            }else
               MessageBox.Show("no se pudo borrar ruta. " + a.estatus.ToString());

            OnNavigatedTo(null);
        }

        private void listaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Debug.WriteLine("lista selection changed");
            var a = (ShowRouteBox)ListOfBoxes.SelectedItem;
            if (a == tempBox)
            {
                return;
            }
            tempBox.Grid.Background = tempcol; //83C13F
        }

        private void b_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));
        }

        
        
    }
}