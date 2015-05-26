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
using System.Windows.Media;

namespace VoyIteso.Pages
{
    public partial class TypePAge : PhoneApplicationPage
    {
        //User
        User user = new User();
        //Web Service
        //ServiceReferenceVoyItesoMovil.VoyItesoMovilClient clientVoyIteso = new ServiceReferenceVoyItesoMovil.VoyItesoMovilClient();
        private bool isAskLift;
        private bool isOfferLift;

        SolidColorBrush navyColor = new SolidColorBrush(Color.FromArgb(255, 16, 69, 114));
        SolidColorBrush skyColor = new SolidColorBrush(Color.FromArgb(255, 110, 207, 243));


        public TypePAge()
        {
            InitializeComponent();
            user.getInfo(user.key);
            isAskLift = false;
            isOfferLift = false;
            BuildLocalizedApplicationBar();
            //Web Service
            //clientVoyIteso.GetUserNameCompleted += clientVoyIteso_GetUserNameCompleted;
        }

        /*
        #region clientVoyIteso_GetUserNameCompleted
        void clientVoyIteso_GetUserNameCompleted(object sender, ServiceReferenceVoyItesoMovil.GetUserNameCompletedEventArgs e)
        {
            String Info;
            //String name;
            //String gender;
            int index;

            Info = e.Result;
            index = Info.IndexOf(":");
            user.Name = Info.Substring(0, index);

            user.Gender = Info.Substring(index + 1);
            user.setInfo(user.key);
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }
        #endregion
        */
        /*
        #region btnDriver & btnWalker Click
        private void btnDriver_Click(object sender, RoutedEventArgs e)
        {
            user.Type = "driver";
            goToHome();
        }
        
        private void btnWalker_Click(object sender, RoutedEventArgs e)
        {
            user.Type = "walker";
            goToHome();
        }
        #endregion
        */
        #region goToHome
        private void goToHome()
        {
            user.setInfo(user.key);
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }
        #endregion 
        
        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationService.RemoveBackEntry();
        }
        #endregion

        #region AskLiftGrid_Tap
        private void AskLiftGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(isOfferLift)
            {
                txtOfferLift.Foreground = skyColor;
                isOfferLift = false;
            }
            txtAskLift.Foreground = navyColor;   
            isAskLift = true;
        }
        #endregion

        #region OfferLiftGrid_Tap
        private void OfferLiftGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(isAskLift)
            {
                txtAskLift.Foreground = skyColor;
                isAskLift = false;
            }
            txtOfferLift.Foreground = navyColor;
            isOfferLift = true;
        }
        #endregion

        #region BuildLocalizedApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 0;
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.IsVisible = true;
            ApplicationBar.ForegroundColor =  Color.FromArgb(255, 110, 207, 243);

            ApplicationBarIconButton appBarIconCheck = new ApplicationBarIconButton(new Uri("Images/check.png",UriKind.Relative));
            appBarIconCheck.Text = "Listo";
            appBarIconCheck.Click += appBarIconCheck_Click;
            ApplicationBar.Buttons.Add(appBarIconCheck);

            //    // Create a new menu item with the localized string from AppResources.
            //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        void appBarIconCheck_Click(object sender, EventArgs e)
        {
            if (!isAskLift && !isOfferLift)
            {
                MessageBox.Show("Selecciona un perfil", "Atencion", MessageBoxButton.OK);
                return;
            }
            else if (isAskLift)
                user.Type = "walker";
            else if (isOfferLift)
                user.Type = "driver";
            
            goToHome();
        }
        #endregion
    }
}