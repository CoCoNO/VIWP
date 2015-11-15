using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Diagnostics;
using VoyIteso.Class;
using System.Windows.Threading;
using System.Windows.Input;
using System.Reflection;
using System.IO.IsolatedStorage;

namespace VoyIteso
{
    public partial class MainPage : PhoneApplicationPage
    {
        
        //User 
        //User user = new User();
        //Timer for the Splash Screen
        DispatcherTimer SplashTimer = new DispatcherTimer();
        public IsolatedStorageSettings Settings;
        //Web Service
        //ServiceReferenceVoyItesoMovil.VoyItesoMovilClient clientVoyIteso = new ServiceReferenceVoyItesoMovil.VoyItesoMovilClient();

        public MainPage()
        {
            InitializeComponent();
            versionControlSplash.Text = "v " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //////Set the timer and start it
            SplashTimer.Interval = TimeSpan.FromSeconds(2);
            SplashTimer.Tick += SplashTimer_Tick;
            SplashTimer.Start();
            //user.deleteInfo(user.key);

            //Web Service
            //clientVoyIteso.GetUserNameCompleted += clientVoyIteso_GetUserNameCompleted;

            Settings = IsolatedStorageSettings.ApplicationSettings;
            
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (ApiConnector.Instance.CheckIfLoggedIn())
            {
                try
                {
                    //new Progress().showProgressIndicator(this, "Espera unos momentos, por favor...");
                    //ApiConnector.instance.createUserFromToken();
                    await ApiConnector.Instance.GetActiveUserFromSettings();
                    
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
            //new Progress().hideProgressIndicator(this);
        }

        int counter=0;
        async void SplashTimer_Tick(object sender, EventArgs e)
        {
            SplashTimer.Stop();
            
            //Check if there is a local session

            Debug.WriteLine(counter++ + SplashTimer.Interval.Ticks);

            if (!ApiConnector.Instance.IsLoggedIn)
            {
                NavigationService.Navigate(new Uri("/Pages/AutentificationPage.xaml", UriKind.Relative));
            }
            else
            {
                if (ApiConnector.Instance.ActiveUser == null)
                {
                    SplashTimer.Interval= TimeSpan.FromSeconds(0.25);
                    SplashTimer.Start();
                }
                else
                {
                    NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                }
                
            }
            //new Progress().hideProgressIndicator(this);
             
            
        }
        
    }
         
}