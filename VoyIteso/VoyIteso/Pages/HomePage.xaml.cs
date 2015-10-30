using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Class;
using System.Windows.Media.Imaging;
using VoyIteso.Resources;
using System.Windows.Media;
using Telerik.Windows.Controls;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
//ve a la linea 174 
namespace VoyIteso.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {

        /////class fields//////
        User user = new User();
        public static Notificacione NotificationItem; 
        public static Class.Notifications ListOfNotifications;


        public  HomePage()
        {
            //NavigationService.RemoveBackEntry();

            InitializeComponent();

            GetNotifs(); 

            BuildLocalizedApplicationBar();

            //contenido del azulejo de calendario. 
            txtDayString.Text = foo();//mes nombre
            txtDayNumber.Text = DateTime.Today.Day.ToString();//dia numero
            atrasCalendario.Text = "Agenda";
            //termina el contenido del calendario. 

            //contenido notificaciones.

            if (TheNewMap.Driver)
            {
                this.userTypeToogle.Source = new BitmapImage(new Uri("/Images/dar_aventon.png", UriKind.Relative));
                //_pushPinUsuario.Source = new BitmapImage(new Uri("/Images/u.png", UriKind.Relative));
            }
            else
            {
                this.userTypeToogle.Source = new BitmapImage(new Uri("/Images/pedir_aventon.png",UriKind.Relative));
            }

            

        }


        protected override void OnBackKeyPress(CancelEventArgs e)
        {
           System.Windows.Application.Current.Terminate();
            
        }

        private async void GetNotifs()
        {
            new Progress().showProgressIndicator(this, "Ocupado");
            ListOfNotifications = await ApiConnector.Instance.NotificationsGet();
            new Progress().hideProgressIndicator(this);
        }

        private string foo()
        {
            var a = DateTime.Today.Month.ToString();

            switch (a)
            {
                case "1":
                    return "ene";
                    break;
                case "2":
                    return "feb";
                    break;
                case "3":
                    return "mar";
                    break;
                case "4":
                    return "abr";
                    break;
                case "5":
                    return "may";
                    break;
                case "6":
                    return "jun";
                    break;
                case "7":
                    return "jul";
                    break;
                case "8":
                    return "ago";
                    break;
                case "9":
                    return "sep";
                    break;
                case "10":
                    return "oct";
                    break;
                case "11":
                    return "nov";
                    break;
                case "12":
                    return "dic";
                    break;

                default:
                    return "n/a";

            }

        }


        public static Appointment[] apps;

        #region OnNavigatedTo
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationService.RemoveBackEntry();
            //txtUserName.Text =;

            ApiConnector.Instance.ActiveUser.UserDataChanged += UserDataChanged;
            ApiConnector.Instance.UpdateCurrentProfileImage();

            user.getInfo(user.key);

            txtUserName.Text = AppResources.HelloUsertxt + " " + ApiConnector.Instance.ActiveUser.Name + "!";

            profileTile.Title = ApiConnector.Instance.ActiveUser.Name;


            ////if (user.setImageUrl())
            //{
            //    //profileImage.Source = (new BitmapImage(new Uri(string.Format(user.imageUrl + "?Refresh=true&random={0}", Guid.NewGuid()), UriKind.Absolute)));
            //    //profileImage.Source = ApiConnector.instance.ActiveUser.Avatar;
            //    //ApiConnector.instance.ActiveUser.UserDataChanged += UserDataChanged;
            //    //ApiConnector.instance.UpdateCurrentProfileImage();
            //    //profileTile.IsFrozen = false;
            //}
            ////ApiConnector.instance.ActiveUser.OnUserDataChanged +=

            new Progress().showProgressIndicator(this,"cargando citas para el calendario");
            apps = await ApiConnector.Instance.LoadCurrentMonthLifts();//a lift is an appointment
            new Progress().hideProgressIndicator(this);
            Microsoft.Phone.Shell.SystemTray.ForegroundColor = Color.FromArgb(255, 110, 207, 243);

            //checar aventones por revisar.
            var a = await ApiConnector.Instance.LiftCheckIfRateNeeded();
            if (a.aventones.Count>0)
            {
                RatePage.idaventon = a.aventones[0].aventon_id;//para saber cual calificar.
                RatePage.nombreConductor = a.aventones[0].nombre;//.
                RatePage.idconductor = a.aventones[0].perfilconductor_id;//para sacar la foto.
                NavigationService.Navigate(new Uri("/Pages/0RatePage.xaml", UriKind.Relative));
            }
        }
        #endregion
        void UserDataChanged(object sender, EventArgs e)
        {
            ApiConnector.Instance.ActiveUser.UserDataChanged -= UserDataChanged;
            // profileImage.Source = ApiConnector.Instance.ActiveUser.Avatar;
            profileTile.Picture.Source = ApiConnector.Instance.ActiveUser.Avatar;
            profileTile.IsFrozen = false;
            //TestImage.Source = ApiConnector.Instance.ActiveUser.Avatar;//alv con esto, jairo. 
        }

        #region appBar Clicks
        void appBarSingOut_Click(object sender, EventArgs e)
        {
            ApiConnector.Instance.LogOut();
            NavigationService.Navigate(new Uri("/Pages/AutentificationPage.xaml", UriKind.Relative));
        }
        #endregion

        #region BuildLocalizedApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
            ApplicationBar.Opacity = 0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.BackgroundColor = Color.FromArgb(255, 0, 66, 112);
            ApplicationBar.ForegroundColor = Colors.White;

            ApplicationBarMenuItem appBarSingOut = new ApplicationBarMenuItem("Cerrar Sesion");
            appBarSingOut.Click += appBarSingOut_Click;
            ApplicationBar.MenuItems.Add(appBarSingOut);

            ApplicationBarMenuItem a = new ApplicationBarMenuItem("Acerca de...");
            a.Click += a_click;
            ApplicationBar.MenuItems.Add(a);

            //ApplicationBarMenuItem a = new ApplicationBarMenuItem("ir al nuevo mapa");
            //a.Click += a_Click;
            //ApplicationBar.MenuItems.Add(a);

            //ApplicationBarMenuItem b = new ApplicationBarMenuItem("ir al chat pa' ver como se ve");
            //b.Click += b_Click;
            //ApplicationBar.MenuItems.Add(b);

            //ApplicationBarMenuItem c = new ApplicationBarMenuItem("ir a ShowRoute");
            //c.Click += c_Click;
            //ApplicationBar.MenuItems.Add(c);

            ApplicationBar.StateChanged += ApplicationBar_StateChanged;

            //    // Create a new menu item with the localized string from AppResources.
            //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        /// <summary>
        /// TEMPORAL!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void a_click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Pages/0RatePage.xaml",UriKind.Relative));
        }

        private void c_Click(object sender, EventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Pages/Notifications/RouteInfo.xaml", UriKind.Relative));
            NavigationService.Navigate(new Uri("/Pages/ShowRoutes.xaml", UriKind.Relative));
        }

        private void b_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ChatLayout.xaml", UriKind.Relative));
            //NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/NotificationWindow.xaml", UriKind.Relative));
            //NavigationService.Navigate(new Uri("/Pages/ChatStuff/ChatWindow.xaml", UriKind.Relative));


        }

        //private void a_Click(object sender, EventArgs e)
        //{
        //    NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));
        //}

        void ApplicationBar_StateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {

            if (e.IsMenuVisible)
                ApplicationBar.Opacity = 1;
            else
                ApplicationBar.Opacity = 0;

        }
        #endregion

        private void profile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ProfilePage.xaml", UriKind.Relative));
        }

        private void searchOfferMapTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Pages/SelectType.xaml", UriKind.Relative));
            if (TheNewMap.Driver)
            {
                NavigationService.Navigate(new Uri("/Pages/ShowRoutes.xaml", UriKind.Relative));
            }
            else
            {
                TheNewMap.ReadOnly = false;
                NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));
            }
            
        }

        private void calendarTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Pages/CalendarPage.xaml", UriKind.Relative));
            NavigationService.Navigate(new Uri("/Pages/TheNewCalendar.xaml", UriKind.Relative));
        }

        private void NotificationsTile_OnTap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Notifications.xaml", UriKind.Relative));
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox.SelectedIndex = -1;
        }

        private void UserTypeToogle_OnTap(object sender, GestureEventArgs e)
        {
            TheNewMap.Driver = !TheNewMap.Driver;

            this.userTypeToogle.Source = TheNewMap.Driver ? new BitmapImage(new Uri("/Images/dar_aventon.png", UriKind.Relative)) : new BitmapImage(new Uri("/Images/pedir_aventon.png", UriKind.Relative));
        }
    }
}