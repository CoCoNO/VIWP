using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages
{
    public partial class SelectType : PhoneApplicationPage
    {
        public SelectType()
        {
            InitializeComponent();
        }

        private void Navigate()
        {
            //NavigationService.Navigate(new Uri("/Pages/SearchRoutePage.xaml", UriKind.Relative));
            NavigationService.Navigate(new Uri("/Pages/TheMap.xaml", UriKind.Relative));
        }

        private void PedirRide_OnTap(object sender, GestureEventArgs e)
        {
            SearchRoutePage.Driver = false;
            SearchRoutePage.Passenger = true;
            Navigate();
        }

        private void DarRide_OnTap(object sender, GestureEventArgs e)
        {
            SearchRoutePage.Passenger = false;
            SearchRoutePage.Driver = true;
            Navigate();
        }
    }
}