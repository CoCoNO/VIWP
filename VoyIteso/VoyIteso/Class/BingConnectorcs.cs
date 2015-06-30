using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VoyIteso.Resources;

namespace VoyIteso.Class
{
    /*class BingConnectorcs
    {
        public event EventHandler responseChanged;
        public event EventHandler exceptionChanged;

        private RootObject root;
        private string connectorUrl;
        private string parameters;
        private string bingKey;
        public string address;
        private string _throwException;
        public string throwException
        {
            get
            {
                return this._throwException;
            }
            set
            {
                this._throwException = value;
                if (this.exceptionChanged != null)
                    this.exceptionChanged(this, new EventArgs());
            }
        }
        private string _jsonResponse;
        public string jsonResponse
        {
            get
            {
                return this._jsonResponse;
            }
            set
            {
                this._jsonResponse = value;
                if (this.responseChanged != null)
                    this.responseChanged(this, new EventArgs());
            }
        }

        public BingConnectorcs()
        {
            this.connectorUrl = AppResources.BingServiceUrl;
        }

        public void setParameters()
        {
            parameters = string.Format("?countryRegion=mexico&adminDistrict=jalisco&locality=guadalajara&addressLine={0}&maxResults=10&o=json&key={1}", this.address, AppResources.BingServiceKey);
        }

        public void SendGetRequest()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += client_OpenReadCompleted;
            setParameters();
            string uri = string.Format(connectorUrl, parameters);
            client.OpenReadAsync(new Uri(uri));
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                Stream streamResponse = e.Result;
                StreamReader streamReader = new StreamReader(streamResponse);
                //var responseString = streamReader.ReadToEnd();
                //jsonResponse = JObject.Parse(responseString);
                jsonResponse = streamReader.ReadToEnd();
                streamReader.Close();
                streamResponse.Close();
            }
            catch (Exception ex)
            {
                throwException = "Ocurrio un error al tratar de conectarse al servidor. Verifique su conexión a Internet";
            }
        }

        public List<Street> getStreets()
        {
            List<Street> streets = new List<Street>();
            root = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
            foreach(Resource resource in root.resourceSets.ElementAt(0).resources )
            {
                Street street = new Street(resource);
                streets.Add(street);
            }
            return streets;
        }
    }*/
}
