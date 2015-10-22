using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Pages.ShowRoutesComponents;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages
{
    public partial class SelectType : PhoneApplicationPage
    {
        public SelectType()
        {
            InitializeComponent();
            AddContent(false);
            AddContent(true);
        }

        private void AddContent(bool driver)
        {

            //var a = new Image
            //{
            //    Source =
            //        driver
            //            ? new BitmapImage(new Uri("/Images/dar_aventon.png", UriKind.Relative))
            //            : new BitmapImage(new Uri("/Images/pedir_aventon.png", UriKind.Relative))
            //};



            var b = new TypeIcon(!driver)
            {
                //icon = a,
                label = { Text = !driver ? "dar aventón" : "pedir aventón" }
            };
            

            if (driver)
            {
                pedirRide.Content = b;
            }
            else
            {
                darRide.Content = b;
            }
        }

        private void Navigate(string pageRoute)//this will take you to TheNewMap layout.
        {
            NavigationService.Navigate(new Uri(pageRoute, UriKind.Relative));
        }

        private void PedirRide_OnTap(object sender, GestureEventArgs e)
        {
            TheNewMap.Driver = false;
            Navigate("/Pages/HomePage.xaml");
            //Navigate("/Pages/TheNewMap.xaml"); 
        }

        private void DarRide_OnTap(object sender, GestureEventArgs e)
        { 
            TheNewMap.Driver = true;
            Navigate("/Pages/HomePage.xaml");
            //Navigate("/Pages/ShowRoutes.xaml");
        }
    }
}