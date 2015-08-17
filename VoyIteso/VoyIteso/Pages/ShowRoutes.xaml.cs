﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Pages.ShowRoutesComponents;

namespace VoyIteso.Pages
{
    public partial class ShowRoutes : PhoneApplicationPage
    {
        public ShowRoutes()
        {
            InitializeComponent();
            BuildAppBar();
        }

        private void BuildAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("Assets/refresh.png", UriKind.Relative));
            a.Text = "agregar caja// pruebas";
            a.Click += a_Click;
            ApplicationBar.Buttons.Add(a);

            ApplicationBarIconButton b = new ApplicationBarIconButton(new Uri("Assets/add.png", UriKind.Relative));
            b.Text = "agregar ruta";
            b.Click += b_Click;
            ApplicationBar.Buttons.Add(b);
            
        }
        
        private void a_Click(object sender, EventArgs e)
        {
            var a = new Grid(){Height = 20};
            ListOfBoxes.Items.Add(a);
            ListOfBoxes.Items.Add(new ShowRouteBox());
        }

        private void b_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/TheNewMap.xaml", UriKind.Relative));
        }

    }
}