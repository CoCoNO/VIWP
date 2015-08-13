using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Pages.Chat2;

namespace VoyIteso.Pages
{
    public partial class Notifications : PhoneApplicationPage
    {
        public Notifications()
        {
            InitializeComponent();
        }

        private void CrearNuevaCajitaFeliz_click(object sender, EventArgs e)
        {
            //pinche trampa sucia! pero funciona y se ve muy bien. 
            var grid = new Grid();
            grid.Width = 440;
            grid.Height = 20;
            lista.Items.Add(grid);

            //Este es el grid importante. 



            grid = new Grid();
            grid.Width = 440;
            grid.Height = 112;//150
            grid.Background = new SolidColorBrush(Color.FromArgb(255, 133, 187, 220));//este es el color de prueba. 

            var sgrid = new Grid();
            sgrid.Margin = new Thickness(10,0,366,0);//10,0,366,0
            var img = new Image();
            var bmp = new BitmapImage();
            var uir = new Uri("Images/you.jpg", UriKind.Relative);//("ms-appx:///Images/you.png");
            bmp.UriSource = uir;
            img.Source = bmp;
            sgrid.Children.Add(img);

            var sp = new StackPanel();
            sp.Margin = new Thickness(91,0,0,0);
            var tb = new TextBlock();
            tb.FontSize = 13;
            tb.Text = "header";
            var tb2 = new TextBlock();
            tb2.Text = "content";

            sp.Children.Add(sgrid);
            sp.Children.Add(tb);
            sp.Children.Add(tb2);

            grid.Children.Add(sp);

            lista.Items.Add(grid);



        }
    }
}