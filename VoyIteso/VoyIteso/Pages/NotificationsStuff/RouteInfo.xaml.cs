using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework;
using VoyIteso.Class;
using VoyIteso.Pages.MapStuff;
using Color = System.Windows.Media.Color;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using System.Device.Location;

namespace VoyIteso.Pages
{

    public partial class RouteInfo : PhoneApplicationPage
    {


        public static int aventonid = 0;
        public static int idsegundo;
        private static Notificacione Notificacion;
        public static bool myBool = true;
        public static bool fromCalendar = false;

        public RouteInfo()
        {
            InitializeComponent();


            myCajaDeResultados = TheNewMap.caja;
            try
            {
                myBool = myCajaDeResultados.myBool;
                DateTime myDateTime;
                myDateTime = new DateTime();
                myDateTime = DateTime.ParseExact(TheNewMap.dateString, "dd-MM-yyyy",
                                                 null);
                this.fechaParaLaCualSeSolicitaElAventon = myDateTime;
            }
            catch (Exception)
            {
                myBool = false;
            }

            if (!myBool)//nomap
            {
                if (!fromCalendar)//
                {
                    Notificacion = Notifications.NotificationItem; 
                }
                else
                {
                    //InitFromCalendar();//notificacion = n; 
                }
            }

            loadData();

            //texto de origen,
            //texto destino,
            //id de aventon.
            //hora aventon.
            //fecha aventon.
            //id de la otra persona.
            //
        }


        private async void InitFromCalendar()
        {



            //var notif = await ApiConnector.Instance.NotificationsGet();


            //foreach (var n in notif.notificaciones)
            //{
            //    if (n.aventon_id==aventonid)
            //    {
            //        //segundapersonaid = n.perfil_id;
            //        Notificacion = n;
            //        return;
            //    }
                
            //}

            //Notificacion = ApiConnector.Instance.
            //localUser = ApiConnector.Instance.GetUserById()
        }

        public void foo(cajaDeResultados caja)
        {
            myCajaDeResultados = caja;
            myBool = true;
        }


        public static cajaDeResultados myCajaDeResultados { get; set; }

        public RouteInfo(Notificacione notif)
        {
            InitializeComponent();
            Notificacion = notif;
            loadData();

        }


        public RouteInfo(cajaDeResultados caja)
        {
            InitializeComponent();
            myCajaDeResultados = caja;
            loadData();

        }

        private User localUser;
        private async void loadData()
        //esta seccion requiere de los siguientes parametros: 
        //string perfil_id (el id de la segunda persona)
        //string aventon_id
        //string texto_origen
        //string texto_destino
        {
            new Progress().showProgressIndicator(this, "espera");
            User user;
            if (!myBool)//no map
            {
                try
                {

                    if (fromCalendar)
                    {
                        user = await ApiConnector.Instance.GetUserById(idsegundo.ToString());
                        UserDetails.Text = user.Name + "\n" + user.profile.carrera;
                        
                        var rut =await ApiConnector.Instance.RouteGet(aventonid);//no esta obteniendo la ruta bien. 
                        LiftDetails.Text = "Origen: " + rut.texto_origen + "\n\n" + "Destino: " + rut.texto_destino;


                        new Progress().hideProgressIndicator(this);
                    }
                    else
                    {
                        user = await ApiConnector.Instance.GetUserById(Notificacion.perfil_id.ToString());
                        //UserDetails.Text = user.Name + "\n" + user.profile.edad + " años\n" + user.profile.carrera;
                        UserDetails.Text = user.Name + "\n" + user.profile.carrera;
                        LiftDetails.Text = "Origen: " + Notificacion.origen + "\n\n" + "Destino: " + Notificacion.destino;
                        new Progress().hideProgressIndicator(this);
                    }
                    
                }
                catch (Exception)
                {

                    return;
                }

            }
            else
            {
                user = await ApiConnector.Instance.GetUserById(myCajaDeResultados.perfil_id);
                //la edad que sea opcional mostrarla.. en todA lo que conozco asi es. 
                UserDetails.Text = user.Name + "\n" + user.profile.carrera;
                //UserDetails.Text = user.Name + "\n" + user.profile.edad + " años\n" + user.profile.carrera;

                LiftDetails.Text = "Origen: " + myCajaDeResultados.texto_origen + "\n\n" + "Destino: " + myCajaDeResultados.texto_destino;

                new Progress().hideProgressIndicator(this);
            }
            var image = ApiConnector.Instance.GetUserImageById(Convert.ToInt32(user.profileID));//Convert.ToInt32(myCajaDeResultados.perfil_id)
            DisplayImage.Source = image;
            this.localUser = user;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //if (!myBool)//nomapa
            //{
            //    if (!fromCalendar)//no cal. notifs
            //    {
            //        LiftDetails.Text = "Origen: " + Notificacion.origen + "\n\n" + "Destino: " + Notificacion.destino;
            //    } 
                
            //}
            //else
            //{
            //    LiftDetails.Text = "Origen: " + myCajaDeResultados.texto_origen + "\n\n" + "Destino: " + myCajaDeResultados.texto_destino;
            //}

            ocultarMierda();
            fromCalendar = false;
        }

        Button botonSolicitar;
        private void ocultarMierda()
        {

            if (myBool)// si viene de la busqueda del mapa.
            {
                GridDeBotones.Children.Remove(BotonAceptar);
                GridDeBotones.Children.Remove(BotonRechazar);
                botonSolicitar = new Button();
                botonSolicitar.Content = "Solicitar aventón";
                botonSolicitar.Foreground = new SolidColorBrush(Colors.White);
                botonSolicitar.BorderBrush = new SolidColorBrush(Colors.White);
                botonSolicitar.Click += botonSolicitar_OnClick;
                GridDeBotones.Children.Add(botonSolicitar);
                GridDeInformacion.Children.Remove(ChatButton);
                buildAppBar(appBarStates.VerRuta);
            }
            else//sino viene de las noticicaciones.
            {
                if (_flag)
                {
                    GridDeBotones.Children.Remove(BotonAceptar);
                    GridDeBotones.Children.Remove(BotonRechazar);

                    buildAppBar(appBarStates.Regresarmenu);
                    return;
                }


                try
                {

                    if (Notificacion.tipo == null)
                    {
                        GridDeBotones.Children.Remove(BotonAceptar);
                        GridDeBotones.Children.Remove(BotonRechazar);
                        var a = new TextBlock() { Text = "Aventón terminado.", Foreground = new SolidColorBrush(Colors.White), Width = 380 };
                        GridDeBotones.Children.Add(a);
                        buildAppBar(appBarStates.Regresarmenu);
                    }
                    else if (Notificacion.tipo.Substring(0, 1).Equals("A"))//Notificacion.estatus_aventon//Solicitud, Cancelacion, Aceptada
                    {
                        GridDeBotones.Children.Remove(BotonAceptar);
                        GridDeBotones.Children.Remove(BotonRechazar);
                        var a = new TextBlock() { Text = "Aventón aceptado.", Foreground = new SolidColorBrush(Colors.White), Width = 380 };
                        GridDeBotones.Children.Add(a);
                    }
                    else if (Notificacion.tipo.Substring(0, 1).Equals("R"))//Notificacion.estatus_aventon//Solicitud, Recha, Aceptada
                    {
                        GridDeBotones.Children.Remove(BotonAceptar);
                        GridDeBotones.Children.Remove(BotonRechazar);
                        var a = new TextBlock() { Text = "Aventón rechazado :(.", Foreground = new SolidColorBrush(Colors.White), Width = 380 };
                        GridDeBotones.Children.Add(a);
                    }
                    else if (Notificacion.tipo.Substring(0, 1).Equals("S"))//Notificacion.estatus_aventon//Solicitud, Cancelacion, Aceptada
                    {

                        if (Notificacion.rol.Equals("Pasajero"))
                        {
                            GridDeBotones.Children.Remove(BotonAceptar);
                            GridDeBotones.Children.Remove(BotonRechazar);
                            var a = new TextBlock() { Text = "Solicitud en espera.", Foreground = new SolidColorBrush(Colors.White), Width = 380 };
                            GridDeBotones.Children.Add(a);
                        }

                    }


                }
                catch (Exception)
                {

                    GridDeBotones.Children.Remove(BotonAceptar);
                    GridDeBotones.Children.Remove(BotonRechazar);
                    //var a = new TextBlock() { Text = "Solicitud en espera.", Foreground = new SolidColorBrush(Colors.White), Width = 380 };
                    //GridDeBotones.Children.Add(a);
                    //throw;
                }

                



            }


        }


        private DateTime fechaParaLaCualSeSolicitaElAventon;
        private int pressed = 0;
        private void botonSolicitar_OnClick(object sender, RoutedEventArgs e)
        {
            if (pressed == 0)
            {
                new Progress().showProgressIndicator(this, "espera...");

                try
                {
                    var a = ApiConnector.Instance.LiftRequest(Convert.ToInt32(myCajaDeResultados.routeID), localUser.Name + " quiere que le des ride", fechaParaLaCualSeSolicitaElAventon);// no requiere hora el datetime.
                    //Debug.WriteLine(fechaParaLaCualSeSolicitaElAventon);
                    new Progress().hideProgressIndicator(this);
                    GridDeBotones.Children.Remove(this.botonSolicitar);

                    GridDeBotones.Children.Remove(BotonAceptar);
                    GridDeBotones.Children.Remove(BotonRechazar);
                    var b = new TextBlock() { Text = "se ha solicitado el aventón", Foreground = new SolidColorBrush(Colors.White), Width = 400 };
                    GridDeBotones.Children.Add(b);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "No se mandó tu solicitud", MessageBoxButton.OK);
                    throw;
                }

                pressed++;
            }
            buildAppBar(appBarStates.Regresarmenu);

        }

        /// <summary>
        /// el botoncito que lleva al chat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)//chat button clicked. 
        {
            //if (myBool)
            //{
            //    var index = myCajaDeResultados.aventon_id;
            //    NavigationService.Navigate(new Uri("/Pages/ChatLayout.xaml?key=" + index, UriKind.Relative));
            //}
            //else
            //{
            var index = Notificacion.aventon_id.ToString();
            NavigationService.Navigate(new Uri("/Pages/ChatLayout.xaml?key=" + index, UriKind.Relative));
            //}

        }

        private bool _flag = false;
        private int segundapersonaid;
        private async void BotonAceptar_OnClick(object sender, RoutedEventArgs e)
        {




            if (pressed == 0)
            {
                new Progress().showProgressIndicator(this, "espera...");


                var a = await ApiConnector.Instance.LiftAccept(Notificacion.aventon_id, "El aventón ha sido aceptado");// response status, si es uno fue exitosa, 0 lo contrario.
                //poner en progress que se ha aceptado. o que fallo la peticion.
                if (a.estatus == 1)
                {
                    MessageBox.Show("Solicitud aceptada");
                    _flag = true;
                    ocultarMierda();

                    GridDeBotones.Children.Remove(BotonAceptar);
                    GridDeBotones.Children.Remove(BotonRechazar);
                    var b = new TextBlock() { Text = "se ha aceptado el aventón", Foreground = new SolidColorBrush(Colors.White), Width = 400 };
                    GridDeBotones.Children.Add(b);

                }
                else
                {
                    MessageBox.Show("No se pudo realizar operación");
                }



                pressed++;
            }
            //
            buildAppBar(appBarStates.Regresarmenu);




        }

        public enum appBarStates { Regresarmenu, VerRuta };
        private void buildAppBar(appBarStates abbBarStateMachine)
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            if (abbBarStateMachine == appBarStates.Regresarmenu)
            {
                ApplicationBarIconButton regresarBut = new ApplicationBarIconButton(new Uri("Images/icons/close.png", UriKind.Relative));
                regresarBut.Text = "Cerrar";
                regresarBut.Click += regresar_Click;
                ApplicationBar.Buttons.Add(regresarBut);

                //ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                //appBarResultButton.Text = "Resultados";
                //appBarResultButton.Click += appBarResultButton_Click;
                //ApplicationBar.Buttons.Add(appBarResultButton);

                //ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                //fijarme.Text = "fijar mi posición";
                //fijarme.Click += fijarme_OnClick;
                //ApplicationBar.MenuItems.Add(fijarme);

                //if (_pointCount <= 0) return;
                //ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                //changeDestination.Text = "cambiar el destino";
                //changeDestination.Click += ChangeDestinationButton_OnClick;
                //ApplicationBar.MenuItems.Add(changeDestination);
            }
            else if (abbBarStateMachine == appBarStates.VerRuta)
            {
                ApplicationBarIconButton regresarBut = new ApplicationBarIconButton(new Uri("Images/icons/Point Objects-50.png", UriKind.Relative));
                regresarBut.Text = "ver puntos";
                regresarBut.Click += ver_Click;
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                ApplicationBar.Buttons.Add(regresarBut);

            }
        }




        private void ver_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("howdy boy!");

            //coordenadas a.
            //coordenadas b.
            //coordenadas puntos intermedios.
            var a = myCajaDeResultados.ruta.puntos_intermedios;

            if (a == null)
            {
                a = myCajaDeResultados.ruta.puntos;
            }

            var w = a.Split(Convert.ToChar(","));
            var v = new List<string>();

            //
            short i = 0, coun = 0;
            foreach (var variable in w)
            {
                if (coun >= w.Count() - 1)
                {
                    i = 3;
                }
                else if (i > 2)
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
            for (i = 0; i <= v.Count - 2; i += 2)
            {
                var wayvar = new GeoCoordinate(Double.Parse(v[i]), Double.Parse(v[i + 1]));
                waypoints.Add(wayvar);
            }

            TheNewMap.ReadOnly = true;
            TheNewMap._readonlypoints = waypoints;


            NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));


        }



        private void regresar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }

        private async void BotonRechazar_Click(object sender, RoutedEventArgs e)
        {
            //var a = await ApiConnector.Instance.LiftAccept(Notificacion.aventon_id, "El aventón ha sido aceptado");// response status, si es uno fue exitosa, 0 lo contrario.
            var a = await ApiConnector.Instance.LiftReject(Notificacion.aventon_id, "El aventón ha sido rechazado");
            //poner en progress que se ha aceptado. o que fallo la peticion.
            if (a.estatus == 1)
            {
                MessageBox.Show("Solicitud rechazada");
                _flag = true;
                ocultarMierda();
            }
            else
            {
                MessageBox.Show("No se pudo realizar operación");
            }

            buildAppBar(appBarStates.Regresarmenu);

        }


        private void DisplayImage_OnTap(object sender, GestureEventArgs e)
        {
            ProfilePageExternal.Perfildelotrowey = Notificacion.perfil_id;
            NavigationService.Navigate(new Uri("/Pages/ProfilePageExternal.xaml", UriKind.Relative));
        }
    }
}