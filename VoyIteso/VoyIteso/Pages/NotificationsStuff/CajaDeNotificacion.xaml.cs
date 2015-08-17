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

namespace VoyIteso.Pages.NotificationsStuff
{
    public partial class CajaDeNotificacion : UserControl
    {
        public CajaDeNotificacion()
        {
            InitializeComponent();
        }

        public BitmapImage Avatar { set { avatarDelSegundo.Source = value; } }
    }
}
