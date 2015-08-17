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

        private void Navigate(string pageRoute)//this will take you to TheNewMap layout.
        {
            NavigationService.Navigate(new Uri(pageRoute, UriKind.Relative));
        }

        private void PedirRide_OnTap(object sender, GestureEventArgs e)
        {
            TheNewMap.Driver = false;
            Navigate("/Pages/TheNewMap.xaml"); 
        }

        private void DarRide_OnTap(object sender, GestureEventArgs e)
        { 
            TheNewMap.Driver = true;
            Navigate("/Pages/ShowRoutes.xaml");
        }
    }
}