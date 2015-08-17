using System;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Calendar;

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



            if (info.Date.Value.DayOfWeek == DayOfWeek.Monday ||
                info.Date.Value.DayOfWeek == DayOfWeek.Friday
                )
            {
                return SpecialTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        public DataTemplate SpecialTemplate
        {
            get;
            set;
        }
    }
}