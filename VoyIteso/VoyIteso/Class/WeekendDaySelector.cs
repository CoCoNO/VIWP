using System;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;
using VoyIteso.Class;
using VoyIteso.Pages;
using VoyIteso.Pages.CalendarComponents;

namespace VoyIteso
{
    public class WeekendDaySelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            CalendarButtonContentInfo info = item as CalendarButtonContentInfo;
            CalendarButton button = container as CalendarButton;

            if (!button.IsFromCurrentView) return null;
            if (info.Date == null) return null;


            foreach (var day in SpecialDays.Days)
            {
                if (day.Day == info.Date.Value.Day)
                {
                    return SpecialTemplate;
                }
            }

            //if (info.Date.Value.DayOfWeek == DayOfWeek.Monday ||
            //    info.Date.Value.DayOfWeek == DayOfWeek.Tuesday ||
            //    info.Date.Value.DayOfWeek == DayOfWeek.Wednesday ||
            //    info.Date.Value.DayOfWeek == DayOfWeek.Thursday ||
            //    info.Date.Value.DayOfWeek == DayOfWeek.Friday ||
            //    info.Date.Value.DayOfWeek == DayOfWeek.Saturday
            //    )
            //{
            //    return SpecialTemplate;
            //}

            return base.SelectTemplate(item, container);
        }

        public DataTemplate SpecialTemplate
        {
            get;
            set;
        }
    }
}