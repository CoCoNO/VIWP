using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoyIteso.Class
{
    public class RouteRInfo
    {
        public int idRoute;
        public int idProfile;
        public int idPerson;
        public double AverageDistance;
        public double latitudeOrigin;
        public double longitudeOrigin;
        public double latitudeDestiny;
        public double longitudeDestiny;

        private String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private String tag;

        public String Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        
        private String date;

        public String Date
        {
            get { return date; }
            set { date = value; }
        }

        private String origin;

        public String Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        private String destiny;

        public String Destiny
        {
            get { return destiny; }
            set { destiny = value; }
        }
        private String time;

        public String Time
        {
            get { return time; }
            set { time = value; }
        }

        public String Rating;

        private String studentEmployee;

        public String StudentEmployee
        {
            get { return studentEmployee; }
            set { studentEmployee = value; }
        }

        private String major;

        public String Major
        {
            get { return major; }
            set { major = value; }
        }

        public String Mail;

        private byte[] imageBuffer;

        public byte[] ImageBuffer
        {
            get { return imageBuffer; }
            set { imageBuffer = value; }
        }

        private String active;

        public String Active
        {
            get { return active; }
            set { active = value; }
        }

        public RouteRInfo()
        {

        }

        #region getDistanceFromAB
        public void getDistanceFromAB(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
        {
            double distanceA;
            double distanceB;
            double[] pointsARadiants = new double[4];
            double[] pointsBRadiants = new double[4];

            pointsARadiants[0] = (latitudeA * Math.PI) / 180;
            pointsARadiants[1] = (longitudeA * Math.PI) / 180;
            pointsARadiants[2] = (latitudeOrigin * Math.PI) / 180;
            pointsARadiants[3] = (longitudeOrigin * Math.PI) / 180;

            pointsBRadiants[0] = (latitudeB * Math.PI) / 180;
            pointsBRadiants[1] = (longitudeB * Math.PI) / 180;
            pointsBRadiants[2] = (latitudeDestiny * Math.PI) / 180;
            pointsBRadiants[3] = (longitudeDestiny * Math.PI) / 180;

            distanceA = Math.Acos(Math.Sin(pointsARadiants[0]) * Math.Sin(pointsARadiants[2]) + Math.Cos(pointsARadiants[0]) * Math.Cos(pointsARadiants[2]) * Math.Cos(pointsARadiants[1] - pointsARadiants[3])) * 6371;
            distanceB = Math.Acos(Math.Sin(pointsBRadiants[0]) * Math.Sin(pointsBRadiants[2]) + Math.Cos(pointsBRadiants[0]) * Math.Cos(pointsBRadiants[2]) * Math.Cos(pointsBRadiants[1] - pointsBRadiants[3])) * 6371;

            this.AverageDistance = distanceA + distanceB;
        }
        #endregion

        #region getIdFromString
        public int[] getIdFromString(String ids)
        {
            int[] IDS = new int[3];

            int index2 = ids.IndexOf(',');
            int numids;
            Int32.TryParse(ids.Substring(0, index2), out numids);
            IDS[0] = numids;
            ids = ids.Remove(0, index2 + 1);

            index2 = ids.IndexOf(',');
            Int32.TryParse(ids.Substring(0, index2), out numids);
            IDS[1] = numids;
            ids = ids.Remove(0, index2 + 1);

            index2 = ids.IndexOf(',');
            Int32.TryParse(ids.Substring(0, index2), out numids);
            IDS[2] = numids;
            ids = ids.Remove(0, index2 + 1);

            return IDS;
        }
        #endregion

        /*
        #region getLatLong
        public double[] getLatLong(String ids)
        {
            double[] latLong = new double[4];

            double num;
            int index2 = ids.IndexOf(',');
            num = double.Parse(ids.Substring(0, index2));
            latLong[0] = num;
            ids = ids.Remove(0, index2 + 1);

            index2 = ids.IndexOf(',');
            num = double.Parse(ids.Substring(0, index2));
            latLong[1] = num;
            ids = ids.Remove(0, index2 + 1);

            index2 = ids.IndexOf(',');
            num = double.Parse(ids.Substring(0, index2));
            latLong[2] = num;
            ids = ids.Remove(0, index2 + 1);

            num = double.Parse(ids.Substring(0, ids.Length));
            latLong[3] = num;

            return latLong;
        }

        #endregion
        */

    }
}
