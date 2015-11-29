using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Pages;
using VoyIteso.Pages.CalendarComponents;

namespace VoyIteso
{
    public partial class TheNewCalendar : PhoneApplicationPage
    {
        private SpecialDays sp;


        public TheNewCalendar()
        {
            sp = new SpecialDays();
            InitializeComponent();

        }


        private DispatcherTimer timer;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //new SpecialDays().UpdateLayout();
            //sp = new SpecialDays();


            if (!this.ContentPanel.Children.Contains(sp))
            {
                this.ContentPanel.Children.Add(sp);
            }
            

            //foreach (var d in DateTime.Today.Month)
            //{
            //    sp.MyCalendar.SelectedValue = new DateTime();
            //}

            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1),
            };

            timer.Tick += OnTimerTick;
            timer.Start();


        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (RouteInfo.fromCalendar)
            {
                NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/RouteInfo.xaml", UriKind.Relative));
                //RouteInfo.fromCalendar = false;
                timer.Stop();
            }
        }


    }
}