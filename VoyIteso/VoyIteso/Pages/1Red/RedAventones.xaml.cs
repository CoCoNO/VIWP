using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using VoyIteso.Class;
using VoyIteso.Pages.NotificationsStuff;
using VoyIteso.Pages.ShowRoutesComponents;

namespace VoyIteso.Pages
{
    public partial class RedAventones : PhoneApplicationPage
    {
        /// <summary>
        /// class fields
        /// </summary>

        enum appBarStateMachine { ITEM_SELECTED, ITEM_NOT_SELECTED  };

        private appBarStateMachine appBarSM;

        public RedAventones()
        {
            InitializeComponent();
            ListOfBoxes.SelectionChanged += listaSelectionChanged;
            //ListOfBoxes.MouseMove += listaMOusemoved;
            
            appBarSM=appBarStateMachine.ITEM_NOT_SELECTED;
            BuildAppBar();
        }




        /// <summary>
        /// estos dos campos son para las rutas y la ruta seleccionada.
        /// </summary>
        Rutes todasLasRutas = new Rutes();
        public Rutai RutaSeleccionadaRutai { get; set; }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ListOfBoxes.Items.Clear();
            var user = ApiConnector.Instance.ActiveUser;
            new Progress().showProgressIndicator(this, "cargando aventones");
            var aventones = await ApiConnector.Instance.GetMyNetwork();
            _aventonesDados = aventones.perfil_dados;
            _aventonesRecibidos = aventones.perfil_recibidos;
            //ApiConnector.Instance.GetRatesByID();//void means active user.
            new Progress().hideProgressIndicator(this);
        }


        private void listaMOusemoved(object sender, System.Windows.Input.MouseEventArgs e)
        {
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

                ApplicationBarMenuItem dados = new ApplicationBarMenuItem();
                dados.Text = "mostrar aventones dados";
                dados.Click += dadosClick;
                ApplicationBar.MenuItems.Add(dados);

                ApplicationBarMenuItem recibidos = new ApplicationBarMenuItem();
                recibidos.Text = "mostrar aventones recibidos";
                recibidos.Click += recibidosClick;
                ApplicationBar.MenuItems.Add(recibidos);

            }
            else
            {
                ApplicationBar = new ApplicationBar();
               ApplicationBar.Mode = ApplicationBarMode.Default;
                
                ApplicationBar.Opacity = 1.0;
                ApplicationBar.IsMenuEnabled = true;
                ApplicationBar.IsVisible = true;

                ApplicationBarMenuItem dados = new ApplicationBarMenuItem();
                dados.Text = "mostrar aventones dados";
                dados.Click += dadosClick;
                ApplicationBar.MenuItems.Add(dados);

                ApplicationBarMenuItem recibidos = new ApplicationBarMenuItem();
                recibidos.Text = "mostrar aventones recibidos";
                recibidos.Click += recibidosClick;
                ApplicationBar.MenuItems.Add(recibidos);
            }

            
            
        }

        private void recibidosClick(object sender, EventArgs e)
        {
            //_dar = false;
            //ContentPanel.Children.Remove(dados);
            //ContentPanel.Children.Remove(recibidos);
            ListOfBoxes.Items.Clear();

            foreach (var aventon in _aventonesRecibidos)
            {
                var a = new Grid() { Height = 20 };
                ListOfBoxes.Items.Add(a);
                var imagen = ApiConnector.Instance.GetUserImageById(aventon.perfil_id);
                var b = new CajaRed()
                {
                    Name = { Text = aventon.nombre },
                    Description = { Text = aventon.descripcion }

                };
                b.Image.Source = imagen;
                ListOfBoxes.Items.Add(b);
                b.Tap += b_Tap;
            }
        }

        private void dadosClick(object sender, EventArgs e)
        {
            //_dar = true;
            //ContentPanel.Children.Remove(dados);
            //ContentPanel.Children.Remove(recibidos);
            ListOfBoxes.Items.Clear();

            foreach (var aventon in _aventonesDados)
            {
                var a = new Grid() { Height = 20 };
                ListOfBoxes.Items.Add(a);
                var imagen = ApiConnector.Instance.GetUserImageById(aventon.perfil_id);
                var b = new CajaRed()
                {
                    Name = { Text = aventon.nombre },
                    Description = { Text = aventon.descripcion }
                };
                b.Image.Source = imagen;
                ListOfBoxes.Items.Add(b);
                b.Tap += b_Tap;
            }
            //Contenedor.Children.Add(ListOfBoxes);

        }
        /// <summary>
        /// this is the view button that will show the route on a map.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void d_Click(object sender, EventArgs e)
        {
            //coordenadas a.
            //coordenadas b.
            //coordenadas puntos intermedios.
            var a =  RutaSeleccionadaRutai.puntos_intermedios;

            var w = a.Split(Convert.ToChar(","));
            var v = new List<string>();

            //
            short i = 0, coun = 0;
            foreach (var variable in w)
            {
                if (coun >= w.Count()-1)
                {   
                    i = 3;
                }else if (i>2)
                {
                    i = 1;
                }
                switch (i)
                {
                    case 0:
                        v.Add(variable.Substring(8));
                        break;
                    case 1:
                        v.Add(variable.Substring(6, variable.Length - 7));
                        break;
                    case 2:
                        v.Add(variable.Substring(7));
                        break;
                    case 3:
                        v.Add(variable.Substring(6, variable.Length - 9));
                        break;
                        
                    default:
                        break;
                }
                coun++;
                i++;
            }

            //load points to waypoint list
            var waypoints = new List<GeoCoordinate>();
            for(i=0;i<=v.Count-2;i+=2)
            {
                var wayvar = new GeoCoordinate(Double.Parse(v[i]), Double.Parse(v[i+1]));
                waypoints.Add(wayvar);
            }

            TheNewMap.ReadOnly = true;
            TheNewMap._readonlypoints = waypoints;
            //TheNewMap.MyObjects = 

            NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));
            
        }

        CajaRed tempBox = new CajaRed();
        private Brush tempcol;
        private List<PerfilDado> _aventonesDados;
        private List<PerfilRecibido> _aventonesRecibidos;
        private bool _dar = false;

        void b_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //appBarSM = appBarStateMachine.ITEM_SELECTED;
            //BuildAppBar();
            //var index = ListOfBoxes.SelectedIndex - 1;
            ////Debug.WriteLine(index/2);

            //if (index % 2 != 0 || index < 0)
            //{
            //    return;
            //}
            //var item = todasLasRutas.rutas[index / 2];
            ////
            //var a = (CajaRed)ListOfBoxes.SelectedItem;
            //RutaSeleccionadaRutai = item;
            //if (a == tempBox)
            //{
            //    return;
            //}
            
            //tempcol = a.Grid.Background;
            //a.Grid.Background = new SolidColorBrush(Color.FromArgb(255, 133, 195, 64));
            //tempBox = a;

            //MessageBox.Show(item.puntos_intermedios);
            //NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));//?key=value&key2=value
        }
         

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
                MessageBox.Show("Ruta borrada");
                OnNavigatedTo(null);
                appBarSM = appBarStateMachine.ITEM_NOT_SELECTED;
                BuildAppBar();
            }else
               MessageBox.Show("No se pudo realizar movimiento");
             
            
        }

        private void listaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = (CajaRed)ListOfBoxes.SelectedItem;
            if (a == tempBox)
            {
                return;
            }
            tempBox.Grid.Background = tempcol; //83C13F blue//el q tiene ahroita = 85C340
        }

        /// <summary>
        /// boton de nueva ruta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_Click(object sender, EventArgs e)
        {
            TheNewMap.ReadOnly = false;
            NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));
        }

        
    }
}