using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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
            var grid = new Grid();
            grid.Width = 400;
            grid.Height = 150;
            grid.Background = new SolidColorBrush(Color.FromArgb(255, 133, 187, 220));

            var sp = new StackPanel();
            var tb = new TextBlock();
            tb.Text = "header";
            var tb2 = new TextBlock();
            tb2.Text = "content";

            sp.Children.Add(tb);
            sp.Children.Add(tb2);

            grid.Children.Add(sp);

            lista.Items.Add(grid);



        }
    }
}