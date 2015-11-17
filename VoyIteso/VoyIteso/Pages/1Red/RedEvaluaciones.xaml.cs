using System;
using System.Collections.Generic;
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
    public partial class RedEvaluaciones : PhoneApplicationPage
    {
        /// <summary>
        /// class fields
        /// </summary>

        enum appBarStateMachine { ITEM_SELECTED, ITEM_NOT_SELECTED  };

        private appBarStateMachine appBarSM;

        public RedEvaluaciones()
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ListOfBoxes.Items.Clear();
            var user = ApiConnector.Instance.ActiveUser;
            new Progress().showProgressIndicator(this, "espera");

            //ApiConnector.Instance.GetRatesByID();//void means active user.
            var v = await ApiConnector.Instance.GetRatesByID();
            foreach (var evaluacione in v.evaluaciones)
            {
                var a = new Grid
                {
                    Height = 20
                };
                ListOfBoxes.Items.Add(a);

                var caja = new CajaRed();
                //caja.Image.Source = ApiConnector.Instance.GetUserImageById(evaluacione.perfil_id);
                caja.Avatar = ApiConnector.Instance.GetUserImageById(evaluacione.perfil_id);
                caja.Name.Text = evaluacione.nombre;
                caja.Description.Text = evaluacione.comentario;
                ListOfBoxes.Items.Add(caja);
            }

            new Progress().hideProgressIndicator(this);
        }



        private void BuildAppBar()
        {

            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;


            ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("Images/icons/close.png", UriKind.Relative));
            a.Text = "Cerrar";
            a.Click += cerrar_Click;
            ApplicationBar.Buttons.Add(a);

            
        }


        /// <summary>
        /// boton de cerrar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cerrar_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();;
        }


        CajaRed tempBox = new CajaRed();
        private Brush tempcol;
        private List<PerfilDado> _aventonesDados;
        private List<PerfilRecibido> _aventonesRecibidos;
        private bool _dar = false;

        void b_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Debug.WriteLine("<b_tap>");


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


        private void listaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var a = (CajaRed)ListOfBoxes.SelectedItem;
            //if (a == tempBox)
            //{
            //    return;
            //}
            //tempBox.Grid.Background = tempcol; //83C13F blue//el q tiene ahroita = 85C340
        }

        
    }
}