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
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages.ShowRoutesComponents
{
    public partial class TypeIcon : UserControl
    {
        public TypeIcon(bool driver)
        {
            InitializeComponent();

            icon.Source = driver ? new BitmapImage(new Uri("/Images/dar_aventon.png", UriKind.Relative)) : new BitmapImage(new Uri("/Images/pedir_aventon.png", UriKind.Relative));
        }
    }
}
