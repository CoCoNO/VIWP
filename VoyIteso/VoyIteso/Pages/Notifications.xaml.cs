using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Class;
using VoyIteso.Pages.Chat2;
using VoyIteso.Pages.NotificationsStuff;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages
{
    public partial class Notifications : PhoneApplicationPage
    {
        private Progress _progress;
        private readonly List<CajaDeNotificacion> _allMyNotifications;
        private Class.Notifications _listOfNotifications;
        public static Notificacione NotificationItem;

        public Notifications()
        {
            InitializeComponent();
            _allMyNotifications = new List<CajaDeNotificacion>();
            _progress = new Progress();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //Class.Notifications
            _progress.showProgressIndicator(this, "espera un momento");
            _listOfNotifications = await ApiConnector.Instance.NotificationsGet(); 
            _progress.hideProgressIndicator(this);
            foreach (var item in _listOfNotifications.notificaciones)
            {
                ConstructNewNotification(item);
            }
        }
        private void CrearNuevaCajitaFeliz_click(object sender, EventArgs e)// este es de prueba. borrar a la goma ya que termine su proposito. 
        {
            //pinche trampa sucia! pero funciona y se ve muy bien. repetir en caja de resultados en mapa XD. 
            var grid = new Grid();
            grid.Width = 440;
            grid.Height = 20;
            lista.Items.Add(grid);

            //Este es el grid importante. 
            var newBox = new CajaDeNotificacion();
            _allMyNotifications.Add(newBox);
            lista.Items.Add(newBox);
        }

        private void Lista_OnTap(object sender, GestureEventArgs e)//cuando le dio clic a un elemento de la lista.
        {
            var index = lista.SelectedIndex-1;
            //Debug.WriteLine(index/2);

            if (index % 2 != 0 || index < 0)
            {
                return;
            }
            var item = _listOfNotifications.notificaciones[index/2];
            NotificationItem = item;

            NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/RouteInfo.xaml", UriKind.Relative));//?key=value&key2=value
        }

        private void ConstructNewNotification(Notificacione item)
        {
            //pinche trampa sucia! pero funciona y se ve muy bien. repetir en caja de resultados en mapa XD. 
            var grid = new Grid();
            grid.Width = 440;
            grid.Height = 20;
            lista.Items.Add(grid);

            //Este es el grid importante. 
            var newBox = new CajaDeNotificacion();
            newBox.header.Text = item.nombre;
            newBox.body.Text = "";
            newBox.body.Text += item.descripcion;
            _allMyNotifications.Add(newBox);
            lista.Items.Add(newBox);
        }



    }
}