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
using System.Collections.ObjectModel;

namespace VoyIteso.Pages
{
    public partial class InfoRoute : PhoneApplicationPage
    {
        private String IDs;
        private ObservableCollection<RouteRInfo> Info = new ObservableCollection<RouteRInfo>();
        private int[] ids = new int[3];
        private double[] latLong = new double[4];
        private String[] RuteInfo;
        private byte[] BufferImage;
        private User user = new User();

        //Web Service
        ServiceReferenceVoyItesoMovil.VoyItesoMovilClient clientVoyIteso = new ServiceReferenceVoyItesoMovil.VoyItesoMovilClient();

        public InfoRoute()
        {
            InitializeComponent();

            user.getInfo(user.key);
            
            clientVoyIteso.GetRouteInfoCompleted += clientVoyIteso_GetRouteInfoCompleted;
            clientVoyIteso.GetPersonImageCompleted += clientVoyIteso_GetPersonImageCompleted;

            
        }

        private void clientVoyIteso_GetRouteInfoCompleted(object sender, ServiceReferenceVoyItesoMovil.GetRouteInfoCompletedEventArgs e)
        {
            RuteInfo = new String[9];
            RuteInfo = e.Result;
            RuteInfo[3] = changeFormatTime(RuteInfo[3]);
            if (RuteInfo[4] == null)
                RuteInfo[4] = "100";
            if (RuteInfo[8] == null)
                RuteInfo[8] = "";
            else
            {
                int i = RuteInfo[8].IndexOf(":");
                RuteInfo[8] = RuteInfo[8].Remove(i - 3, (RuteInfo[8].Length - (i - 3)));
                String s;
                s = RuteInfo[8].Substring(RuteInfo[8].Length - 2, 2);
                s += "/";
                s += RuteInfo[8].Substring(RuteInfo[8].Length - 5, 2);
                s += "/";
                s += RuteInfo[8].Substring(0, 4);
                RuteInfo[8] = s;
            }

            if(BufferImage != null)
                setInfo(RuteInfo, BufferImage);
        }

        private void clientVoyIteso_GetPersonImageCompleted(object sender, ServiceReferenceVoyItesoMovil.GetPersonImageCompletedEventArgs e)
        {
            BufferImage = e.Result;

            if (RuteInfo != null)
                setInfo(RuteInfo, BufferImage);
        }

        #region setInfo
        private void setInfo(String[] routeInfo, byte[] buffer)
        {
            Info[0].Name = routeInfo[0];
            Info[0].Origin = routeInfo[1];
            Info[0].Destiny = routeInfo[2];
            Info[0].Time = routeInfo[3];
            Info[0].Rating = routeInfo[4];
            Info[0].StudentEmployee = routeInfo[5];
            Info[0].Major = routeInfo[6];
            Info[0].Mail = routeInfo[7];
            Info[0].Date = routeInfo[8];
            Info[0].ImageBuffer = buffer;

            list.DataContext = Info;

        }

        #endregion

        #region changeFormatTime
        private String changeFormatTime(String time)
        {
            if (time != null)
            {
                int index = time.IndexOf(':');
                time = time.Remove(0, index - 2);
                return time;
            }
            else
                return "";
        }
        #endregion

        #region Override methods
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            String msg = "";
            RouteRInfo info = new RouteRInfo();

            ////////////////////////////get the ID from the search page
            if (NavigationContext.QueryString.TryGetValue("msg", out msg))

                this.IDs = msg;

            info.Tag = IDs;
            Info.Add(info);
            ////////////////////////////set the IDs and Latitude Longitude to RouteRInfo class
            getIdAndLatLong();
            ///////////////////////////call the web service for the info
            clientVoyIteso.GetRouteInfoAsync(user.Token, Info[0].idRoute, Info[0].idProfile, Info[0].idPerson);
            Dispatcher.BeginInvoke(() => {clientVoyIteso.GetPersonImageAsync(user.Token, Info[0].idRoute, Info[0].idProfile, Info[0].idPerson);});
        }
        #endregion

        #region getIdAndLatLong
        private void getIdAndLatLong()
        {
            
            int index;
            int num;
            double numD;

            index = IDs.IndexOf(",");
            Int32.TryParse(IDs.Substring(0, index), out num);
            Info[0].idRoute = num;
            IDs = IDs.Remove(0, index + 1);

            index = IDs.IndexOf(",");
            Int32.TryParse(IDs.Substring(0, index), out num);
            Info[0].idProfile = num;
            IDs = IDs.Remove(0, index + 1);

            index = IDs.IndexOf(",");
            Int32.TryParse(IDs.Substring(0, index), out num);
            Info[0].idPerson = num;
            IDs = IDs.Remove(0, index + 1);

            index = IDs.IndexOf(",");
            numD = double.Parse(IDs.Substring(0,index));
            Info[0].latitudeOrigin = numD;
            IDs = IDs.Remove(0,index + 1);

            index = IDs.IndexOf(",");
            numD = double.Parse(IDs.Substring(0, index));
            Info[0].longitudeOrigin = numD;
            IDs = IDs.Remove(0, index + 1);

            index = IDs.IndexOf(",");
            numD = double.Parse(IDs.Substring(0, index));
            Info[0].latitudeDestiny = numD;
            IDs = IDs.Remove(0, index + 1);


            numD = double.Parse(IDs.Substring(0, IDs.Length));
            Info[0].longitudeDestiny = numD;
            IDs = IDs.Remove(0, index + 1);
        }
        #endregion

        #region btnRequest_Click
        private void btnRequest_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}