﻿using System;
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
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

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

            ApiConnector.Instance.ActiveUser.UserDataChanged += UserDataChanged;
            ApiConnector.Instance.UpdateCurrentProfileImage();

            user.getInfo(user.key);
            
            txtUserName.Text = AppResources.HelloUsertxt + " " +  ApiConnector.Instance.ActiveUser.Name + "!";

            profileTile.Title = ApiConnector.Instance.ActiveUser.Name;
            

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

            ApplicationBarMenuItem a = new ApplicationBarMenuItem("ir al nuevo mapa");
            a.Click += a_Click;
            ApplicationBar.MenuItems.Add(a);

            ApplicationBarMenuItem b = new ApplicationBarMenuItem("ir al chat pa' calar");
            b.Click += b_Click;
            ApplicationBar.MenuItems.Add(b);

            ApplicationBarMenuItem c = new ApplicationBarMenuItem("ir a la ventana de detalle de ruta");
            c.Click += c_Click;
            ApplicationBar.MenuItems.Add(c);

            ApplicationBar.StateChanged += ApplicationBar_StateChanged;

            //    // Create a new menu item with the localized string from AppResources.
            //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        private void c_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Notifications/RouteInfo.xaml", UriKind.Relative));
        }

        private void b_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ChatLayout.xaml", UriKind.Relative));
        }

        private void a_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));
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
        }

        private void searchOfferMapTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectType.xaml", UriKind.Relative));
        }

        private void calendarTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/CalendarPage.xaml", UriKind.Relative));
        }

        private void NotificationsTile_OnTap(object sender, GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Notifications.xaml", UriKind.Relative));
        }
    }
}