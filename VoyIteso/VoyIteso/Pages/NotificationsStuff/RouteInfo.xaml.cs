using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
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
            var user = await ApiConnector.Instance.GetUserById(Notificacion.perfil_id.ToString());
            UserDetails.Text = user.Name + "\n" + user.profile.edad + " años\n" + user.profile.carrera;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LiftDetails.Text = "Origen: " + Notificacion.origen + "\n\n" + "Destino: " + Notificacion.destino;
            if (Notificacion.tipo.ToString().Substring(0,1).Equals("A"))//Notificacion.estatus_aventon//Solicitud, Cancelacion, Aceptada
            {
                GridDeBotones.Children.Remove(BotonAceptar);
                GridDeBotones.Children.Remove(BotonRechazar);
                var a = new TextBlock(){ Text = "Aventón aceptado"};
                GridDeBotones.Children.Add(a);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)//chat button clicked. 
        {
            var index = Notificacion.aventon_id.ToString();
            NavigationService.Navigate(new Uri("/Pages/ChatLayout.xaml?key="+index, UriKind.Relative));
        }
    }
}