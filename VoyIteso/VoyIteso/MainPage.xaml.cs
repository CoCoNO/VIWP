using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Resources;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Windows.Devices.Geolocation;
using System.Device.Location;
using VoyIteso.Class;
using System.Windows.Threading;
using System.Windows.Input;

namespace VoyIteso
{
    public partial class MainPage : PhoneApplicationPage
    {
        //User 
        User user = new User();
        //Timer for the Splash Screen
        DispatcherTimer SplashTimer = new DispatcherTimer();
        //Web Service
        //ServiceReferenceVoyItesoMovil.VoyItesoMovilClient clientVoyIteso = new ServiceReferenceVoyItesoMovil.VoyItesoMovilClient();

        private bool canChange;

        public MainPage()
        {
            InitializeComponent();

            //////Set the timer and start it
            SplashTimer.Interval = TimeSpan.FromSeconds(2);
            SplashTimer.Tick += SplashTimer_Tick;
            SplashTimer.Start();
            //user.deleteInfo(user.key);
            canChange = false;

            //Web Service
            //clientVoyIteso.GetUserNameCompleted += clientVoyIteso_GetUserNameCompleted;

        }
        /*
        void clientVoyIteso_GetUserNameCompleted(object sender, ServiceReferenceVoyItesoMovil.GetUserNameCompletedEventArgs e)
        {
            String Info;
            //String name;
            //String gender;
            int index;

            Info = e.Result;
            index = Info.IndexOf(":");
            user.Name = Info.Substring(0, index);

            user.Gender = Info.Substring(index+1);
            
            
            user.setInfo(user.key);
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }*/
        
        void SplashTimer_Tick(object sender, EventArgs e)
        {
            user.getInfo(user.key);

            if (user.Token == null)
                NavigationService.Navigate(new Uri("/Pages/AutentificationPage.xaml", UriKind.Relative));

            else if (user.Token != null && user.Name != null && user.profileID != null)
                NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
            /*
            else if (user.Name == null || user.Gender == null)
                clientVoyIteso.GetUserNameAsync(user.Token);
             


            else if(user.Token != null && user.Name != null && user.Gender != null)
                NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
            */
            SplashTimer.Stop();
        }
        
    }
         
}