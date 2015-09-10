using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Pages.CalendarComponents;

namespace VoyIteso
{
    public partial class TheNewCalendar : PhoneApplicationPage
    {
        private SpecialDays sp;
        public TheNewCalendar()
        {
            sp=new SpecialDays();
            InitializeComponent();
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //new SpecialDays().UpdateLayout();
            //sp = new SpecialDays();
            this.ContentPanel.Children.Add(sp);

            //foreach (var d in DateTime.Today.Month)
            //{
            //    sp.MyCalendar.SelectedValue = new DateTime();
            //}

        }
    }
}