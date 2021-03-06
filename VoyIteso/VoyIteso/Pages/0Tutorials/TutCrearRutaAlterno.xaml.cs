﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace VoyIteso.Pages._0Tutorials
{
    public partial class TutCrearRutaAlterno : PhoneApplicationPage
    {
        public TutCrearRutaAlterno()
        {
            InitializeComponent();
        }

        //private void MyPanorama_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    switch (MyPanorama.SelectedIndex)
        //    {
        //        case 0:
        //            MyImage.Source = new BitmapImage(new Uri("/Images/tits/rutas/crearruta1.png", UriKind.Relative));
        //            buildAppBar(false);
        //           // MyImage.Visibility = Visibility.Collapsed;
        //            break;

        //        case 1:
        //            MyImage.Source = new BitmapImage(new Uri("/Images/tits/rutas/crearruta2.png", UriKind.Relative));
        //            buildAppBar(false);
        //            break;

        //        case 2:
        //            MyImage.Source = new BitmapImage(new Uri("/Images/tits/rutas/crearruta3.png", UriKind.Relative));
        //            buildAppBar(false);
        //            break;

        //        case 3:
        //            MyImage.Source = new BitmapImage(new Uri("/Images/tits/rutas/crearruta4.png", UriKind.Relative));
        //            MyImage.Visibility = Visibility.Visible;
        //            buildAppBar(false);
        //            break;
        //    }
        //}

        private void buildAppBar(bool a)
        {

            if (!a)
            {
                ApplicationBar = new ApplicationBar();
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                ApplicationBar.IsVisible = false;
                ApplicationBar.IsMenuEnabled = false;
                MyImage.Visibility = Visibility.Visible;
                return;
            }

            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            //ApplicationBar.BackgroundColor = Color.FromArgb(255, 0, 66, 112);
            //ApplicationBar.ForegroundColor = Colors.White;

            ApplicationBarIconButton ok = new ApplicationBarIconButton(new Uri("/Images/icons/check.png", UriKind.Relative));
            ok.Text = "Continuar";
            ok.Click += ok_Click;
            ApplicationBar.Buttons.Add(ok);

        }

        private void ok_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}