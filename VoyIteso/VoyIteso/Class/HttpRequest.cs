using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace VoyIteso.Class
{
    class HttpRequest
    {

        public  static string Lstatus;
        //Attributes*************************************************************
        #region Attributes
        List<KeyValuePair<string, string>> _parameters;//Parameters to send
        HttpStatusCode _responseStatus;
        string _data;
        string _status;
        //DispatcherTimer TimeOutTimer = new DispatcherTimer();

        //string _url = @"http://voy.cocoapps.mx/VOYAPI";
        static string _url = @"https://aplicacionesweb.iteso.mx/VOYAPI";
        string _action;

        public static string Url {
            get { return _url;}
        }



        #endregion


        //Propieties*************************************************************
        #region Propieties
        public string Data { get { return _data; } }
        public string Status { get { return _status; } }
        public HttpStatusCode ResponseStatus { get { return _responseStatus; } }


        #endregion


        //Events*************************************************************
        #region Events
        /*public EventHandler ResponseObtained;
        protected virtual void OnResponseObtained(EventArgs e)
        {
            if (this._action == "")
            {
                return;
            }
            EventHandler handler = ResponseObtained;
            if (handler != null)
            {
                handler(this, e);
            }
            //clear resources
            this._action = "";
            this._parameters.Clear();
        }*/
        #endregion


        //Methods*************************************************************
        #region Methods
        public HttpRequest()
        {
            _parameters = new List<KeyValuePair<string, string>>();

            //TimeOutTimer.Interval = TimeSpan.FromSeconds(timeOut);
            //TimeOutTimer.Tick += RequestTimeOut;
        }

        //set Url
        public void setAction(string action)
        {
            //_url = Url;
            _action = action;
        }

        //Set Parameters
        public void setParameter(string paramName, string paramValue)
        {
            _parameters.Add(new KeyValuePair<string, string>(paramName, paramValue));
        }

        //send Post
        public async Task sendPost()
        {
           
            var httpClient = new HttpClient(new HttpClientHandler());
            httpClient.Timeout = TimeSpan.FromSeconds(20);

                HttpResponseMessage response = await httpClient.PostAsync(_url + _action, new FormUrlEncodedContent(_parameters));
                httpClient.Dispose();
                if (response.StatusCode!= HttpStatusCode.OK)
                {
                    _status = response.StatusCode.ToString();
                    Lstatus = _status;
                    //return;
                }
                response.EnsureSuccessStatusCode();

            
                var responseString = await response.Content.ReadAsStringAsync();
                _responseStatus = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (responseString != null)
                {
                    ResponceObject rootJson = JsonConvert.DeserializeObject<ResponceObject>(responseString);
                    if (rootJson.estatus == 1)
                    {
                        Lstatus = "OK";
                        _data = responseString;
                        _status = "OK";
                    }
                    else
                    {
                        _status = rootJson.error;
                        Lstatus = _status;
                    }
                }
            }




        }
        
        //Send get
        public async Task sendGet()
        {

            string getData = ""; //Data to send

            bool firstData = true; //Is the first value?
            foreach (var item in _parameters)
            {
                if (firstData) //Is the first value?
                {
                    firstData = false;
                }
                else
                {
                    getData += "&";

                }
                getData += item.Key + "=" + item.Value;
            }

            var httpClient = new HttpClient(new HttpClientHandler());
            httpClient.Timeout = TimeSpan.FromSeconds(20);

            HttpResponseMessage response = await httpClient.GetAsync(_url + _action + "?" + getData);
            httpClient.Dispose();
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Lstatus = _status;
                _status = response.StatusCode.ToString();
                //return;
            }
            
            var responseString = await response.Content.ReadAsStringAsync();
            _responseStatus = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (responseString != null)
                {
                    ResponceObject rootJson = JsonConvert.DeserializeObject<ResponceObject>(responseString);
                    if (rootJson.estatus == 1)
                    {
                        _data = responseString;
                        _status = "OK";
                        Lstatus = _status;
                    }
                    else
                    {
                        _status = rootJson.error;
                        Lstatus = _status;
                    }
                }
            }
        }

        
        


        
        #endregion
    }
}
