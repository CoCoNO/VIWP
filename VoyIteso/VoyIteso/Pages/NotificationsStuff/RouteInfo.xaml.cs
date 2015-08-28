using System;
using System.Collections.Generic;
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

namespace VoyIteso.Pages
{
    public partial class RouteInfo : PhoneApplicationPage
    {
        private Notificacione Notificacion;

        public RouteInfo()
        {
            InitializeComponent();
            Notificacion = Notifications.NotificationItem;
            loadData();
            //texto de origen,
            //texto destino,
            //id de aventon.
            //hora aventon.
            //fecha aventon.
            //id de la otra persona.
            //
        }

        private async void loadData()
        {
            new Progress().showProgressIndicator(this,"espera");
            var user = await ApiConnector.Instance.GetUserById(Notificacion.perfil_id.ToString());
            UserDetails.Text = user.Name + "\n" + user.profile.edad + " años\n" + user.profile.carrera;
            new Progress().hideProgressIndicator(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LiftDetails.Text = "Origen: " + Notificacion.origen + "\n\n" + "Destino: " + Notificacion.destino;
            ocultarMierda();
            
        }

        private void ocultarMierda()
        {
            //si el estatus del aventon ha sido aceptado.
            if (Notificacion.tipo.ToString().Substring(0, 1).Equals("A"))//Notificacion.estatus_aventon//Solicitud, Cancelacion, Aceptada
            {
                GridDeBotones.Children.Remove(BotonAceptar);
                GridDeBotones.Children.Remove(BotonRechazar);
                var a = new TextBlock() { Text = "El aventón ya ha sido aceptado", Foreground = new SolidColorBrush(Colors.Black) };
                GridDeBotones.Children.Add(a);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)//chat button clicked. 
        {
            var index = Notificacion.aventon_id.ToString();
            NavigationService.Navigate(new Uri("/Pages/ChatLayout.xaml?key="+index, UriKind.Relative));
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
    }
}