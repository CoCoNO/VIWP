using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Class;

namespace VoyIteso.Pages
{
    public partial class SecondaryMapPage : PhoneApplicationPage
    {
        public SecondaryMapPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ///aqui puto
            string origen = origentxt.Text;
            string destino = destinotxt.Text;
            string fecha = DatePicker.ValueString;
            double lat_destino = TheMap.BPoint.GeoCoordinate.Latitude;
            double lon_destino = TheMap.BPoint.GeoCoordinate.Longitude;
            double lat_origen = TheMap.APoint.GeoCoordinate.Longitude;
            double lon_origen = TheMap.APoint.GeoCoordinate.Longitude;
            string hora = TimePicker.ValueString;

            if (TheMap.Driver)
            {
                
            }
            else
            {
                ApiConnector.Instance.SearchRoute(origen, destino, fecha, lat_destino,lon_destino,lat_origen,lon_origen,hora);  
            }

        }
    }
}