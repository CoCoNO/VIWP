
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using VoyIteso.Pages.NotificationsL.ViewModels;

namespace VoyIteso.Pages.NotificationsL
{
    public partial class NotificationWindow : PhoneApplicationPage
    {
        public NotificationWindow()
        {
            InitializeComponent();
            CollectionDataItemViewModel model = this.DataContext as CollectionDataItemViewModel;
            
        }
    }

}
