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

namespace VoyIteso.Pages
{
    public partial class RouteInfo : PhoneApplicationPage
    {
        private static Notificacione Notificacion;
        public static bool myBool = true;

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
                Notificacion = Notifications.NotificationItem;
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
            new Progress().showProgressIndicator(this,"espera");
            User user;
            if (!myBool)
            {
                try
                {
                    user = await ApiConnector.Instance.GetUserById(Notificacion.perfil_id.ToString());
                    UserDetails.Text = user.Name + "\n" + user.profile.edad + " años\n" + user.profile.carrera;
                    new Progress().hideProgressIndicator(this);
                }
                catch (Exception)
                {
                    
                    return;
                }
                
            }
            else
            {
                user = await ApiConnector.Instance.GetUserById(myCajaDeResultados.perfil_id);
                UserDetails.Text = user.Name + "\n" + user.profile.edad + " años\n" + user.profile.carrera;
                new Progress().hideProgressIndicator(this);
            }
            ApiConnector.Instance.GetUserImageById(Convert.ToInt32(user.profileID));//Convert.ToInt32(myCajaDeResultados.perfil_id)
            DisplayImage.Source = user.Avatar;
            this.localUser = user;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!myBool)//nomapa
            {
                LiftDetails.Text = "Origen: " + Notificacion.origen + "\n\n" + "Destino: " + Notificacion.destino;
            }
            else
            {
                LiftDetails.Text = "Origen: " + myCajaDeResultados.texto_origen + "\n\n" + "Destino: " + myCajaDeResultados.texto_destino;
            }

            ocultarMierda();
            
        }

        Button botonSolicitar;
        private void ocultarMierda()
        {

            if (myBool)// si viene de la busqueda del mapa.
            {
                GridDeBotones.Children.Remove(BotonAceptar);
                GridDeBotones.Children.Remove(BotonRechazar);
                botonSolicitar = new Button();
                botonSolicitar.Content = "Solicitar aventon";
                botonSolicitar.Foreground = new SolidColorBrush(Color.FromArgb(255,0,24,90));
                botonSolicitar.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 24, 90));
                botonSolicitar.Click += botonSolicitar_OnClick;
                GridDeBotones.Children.Add(botonSolicitar);
                GridDeInformacion.Children.Remove(ChatButton);

            }
            else//sino viene de las noticicaciones.
            {
                    if (Notificacion.tipo.Substring(0, 1).Equals("A"))//Notificacion.estatus_aventon//Solicitud, Cancelacion, Aceptada
                    {
                        GridDeBotones.Children.Remove(BotonAceptar);
                        GridDeBotones.Children.Remove(BotonRechazar);
                        var a = new TextBlock() { Text = "   El aventón ya ha sido aceptado", Foreground = new SolidColorBrush(Colors.Black), Width = 380};
                        GridDeBotones.Children.Add(a);
                    } 
            } 

            
        }


        private DateTime fechaParaLaCualSeSolicitaElAventon;
        private int pressed=0;
        private void botonSolicitar_OnClick(object sender, RoutedEventArgs e)
        {
            if (pressed==0)
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
                    var b = new TextBlock() { Text = "se ha solicitado el aventón", Foreground = new SolidColorBrush(Colors.Black), Width = 380 };
                    GridDeBotones.Children.Add(b);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: "+ex.Message, "No se mandó tu solicitud", MessageBoxButton.OK);
                    throw;
                }

                pressed++;
            }
            buildAppBar(appBarStates.Regresarmenu);
            
        }

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
                NavigationService.Navigate(new Uri("/Pages/ChatLayout.xaml?key="+index, UriKind.Relative)); 
            //}
            
        }

        private async void BotonAceptar_OnClick(object sender, RoutedEventArgs e)
        {
            var a = await ApiConnector.Instance.LiftAccept(Notificacion.aventon_id, "El aventón ha sido aceptado");// response status, si es uno fue exitosa, 0 lo contrario.
            //poner en progress que se ha aceptado. o que fallo la peticion.
            if (a.estatus==1)
            {
                MessageBox.Show("Solicitud aceptada");
                ocultarMierda();
            }
            else
            {
                MessageBox.Show("No se pudo procesar, intenta más tarde");
            }
        }

        public enum appBarStates { Regresarmenu };
        private void buildAppBar(appBarStates abbBarStateMachine)
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            if (abbBarStateMachine == appBarStates.Regresarmenu)
            {
                ApplicationBarIconButton regresarBut = new ApplicationBarIconButton(new Uri("Images/icons.close.png", UriKind.Relative));
                regresarBut.Text = "cerrar";
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
        }

        private void regresar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml",UriKind.Relative));
        }


    }
}