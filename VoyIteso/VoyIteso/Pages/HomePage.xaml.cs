using System;
using System.Collections.Generic;
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

namespace VoyIteso.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        //User
        User user = new User();
        
        //Web Service
        //ServiceReferenceVoyItesoMovil.VoyItesoMovilClient clientVoyIteso = new ServiceReferenceVoyItesoMovil.VoyItesoMovilClient();
        

        public HomePage()
        {
            InitializeComponent();

            //User
            
            //AppBar
            BuildLocalizedApplicationBar();                                     
            //WebService
            //clientVoyIteso.GetPersonImageCompleted += clientVoyIteso_GetPersonImageCompleted;
            
                /*
            else
            {

            }*/
            
           // if (user.Name.Length > 7)
             //   profile.FontSize = 5;

            
            
        }
        /*
        private void clientVoyIteso_GetPersonImageCompleted(object sender, ServiceReferenceVoyItesoMovil.GetPersonImageCompletedEventArgs e)
        {
            byte[] buffer;

            buffer = e.Result;
            if(buffer == null)
            {
                if (user.Gender == "M")
                    user.defaultImage += "H.png";
                else
                    user.defaultImage += "M.png";
                Uri uri = new Uri(user.defaultImage, UriKind.Absolute);
                BitmapImage img = new BitmapImage(uri);
                img.DecodePixelHeight = 70;
                img.DecodePixelWidth = 70;
                profile.Source = img;
                
            }
            else
            {
                user.UserImage = buffer;
            }

            profile.IsFrozen = false;

            //user.UserImage = e.Result;

            //set the Image of default
            //if (user.UserImage != null)
                //code
            //profile.IsFrozen = false;
        }
        */

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationService.RemoveBackEntry();
            //txtUserName.Text =;

            ApiConnector.instance.ActiveUser.UserDataChanged += UserDataChanged;
            ApiConnector.instance.UpdateCurrentProfileImage();

            user.getInfo(user.key);
            
            txtUserName.Text = AppResources.HelloUsertxt + " " +  ApiConnector.instance.ActiveUser.Name + "!";

            profileTile.Title = ApiConnector.instance.ActiveUser.Name;
            

            //if (user.setImageUrl())
            {
                //profileImage.Source = (new BitmapImage(new Uri(string.Format(user.imageUrl + "?Refresh=true&random={0}", Guid.NewGuid()), UriKind.Absolute)));
                //profileImage.Source = ApiConnector.instance.ActiveUser.Avatar;
                //ApiConnector.instance.ActiveUser.UserDataChanged += UserDataChanged;
                //ApiConnector.instance.UpdateCurrentProfileImage();
                //profileTile.IsFrozen = false;
            }
            //ApiConnector.instance.ActiveUser.OnUserDataChanged +=
            Microsoft.Phone.Shell.SystemTray.ForegroundColor = Color.FromArgb(255, 110, 207, 243);
            
        }
        #endregion
        void UserDataChanged(object sender, EventArgs e)
        {
            ApiConnector.instance.ActiveUser.UserDataChanged -= UserDataChanged;
            profileImage.Source = ApiConnector.instance.ActiveUser.Avatar;
            profileTile.IsFrozen = false;
            TestImage.Source = ApiConnector.instance.ActiveUser.Avatar;
        }

        #region appBar Clicks
        void appBarSingOut_Click(object sender, EventArgs e)
        {
            ApiConnector.instance.logOut();
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

            ApplicationBar.StateChanged += ApplicationBar_StateChanged;

            //    // Create a new menu item with the localized string from AppResources.
            //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

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
            //Use this navigation example to see other user profile
            //NavigationService.Navigate(new Uri("/Pages/ProfilePage.xaml?otherUserId=1", UriKind.Relative));
        }

        private void searchOfferMapTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Pages/TheMap.xaml", UriKind.Relative));

            //se le debe preguntar al usuario si va a dar un aventon o pedir ride, para esto se manda a una pagina nueva llamada 
            NavigationService.Navigate(new Uri("/Pages/SelectType.xaml", UriKind.Relative));

            ////emmanuel implementation
            //if (user.getLocationAllowed())
            //{
            //    if (user.Type == "walker")
            //        NavigationService.Navigate(new Uri("/Pages/SearchRoutePage.xaml", UriKind.Relative));
            //    else
            //        NavigationService.Navigate(new Uri("/Pages/OfferRoutePage.xaml", UriKind.Relative));
            //}
            //else
            //{
            //    MessageBoxResult result = MessageBox.Show("Tu ubicación actual nos ayuda a proporcionarte mejores servicios de búsqueda y ubicación.\n\nTu informacion no se usará para identificarte ni ponerse en contacto contigo es solo uso y funcionamiento para VoyIteso.", "¿Permitir que VoyIteso acceda a tu ubicación?", MessageBoxButton.OKCancel);
            //    if (result == MessageBoxResult.OK)
            //    {
            //        user.setLocationPermission(true);
            //        user.setInfo(user.key);
            //        if (user.Type == "walker")
            //            NavigationService.Navigate(new Uri("/Pages/SearchRoutePage.xaml", UriKind.Relative));
            //        else
            //            NavigationService.Navigate(new Uri("/Pages/OfferRoutePage.xaml", UriKind.Relative));
            //    }
            //    else if (result == MessageBoxResult.Cancel)
            //    {
            //        user.setLocationPermission(false);
            //        user.setInfo(user.key);
            //    }

            //}
        }

        private void calendarTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/CalendarPage.xaml", UriKind.Relative));
        }

    }
}