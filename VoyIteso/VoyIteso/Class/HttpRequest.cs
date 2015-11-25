using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private static int meaninglessData = 0;
        //DispatcherTimer TimeOutTimer = new DispatcherTimer();

        //static string _url = @"http://voy.cocoapps.mx/VOYAPI";
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
#if DEBUG
            Debug.WriteLine("Sending Post Request to ("+_url + _action+")");
            Debug.WriteLine("Params");
            foreach (var item in _parameters)
            {
                Debug.WriteLine("   ["+item.Key+"]("+item.Value+")");
            }
#endif
            var httpClient = new HttpClient(new HttpClientHandler());
            //httpClient.MaxResponseContentBufferSize
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.Timeout = TimeSpan.FromSeconds(20);

                HttpResponseMessage response = await httpClient.PostAsync(_url + _action, new FormUrlEncodedContent(_parameters));
                httpClient.Dispose();
            #if DEBUG
                Debug.WriteLine("Responce: " + response.StatusCode);
#endif
                if (response.StatusCode!= HttpStatusCode.OK)
                {
                    _status = response.StatusCode.ToString();
                    Lstatus = _status;
                    //return;
                }
                response.EnsureSuccessStatusCode();

            
                var responseString = await response.Content.ReadAsStringAsync();
                _responseStatus = response.StatusCode;
#if DEBUG
                Debug.WriteLine("Data: ");
                Debug.WriteLine(responseString);
#endif
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
            _parameters.Add(new KeyValuePair<string, string>("randData",meaninglessData++.ToString()));//Dirty but it works
#if DEBUG
            Debug.WriteLine("Sending Get Request to (" + _url + _action + ")");
            Debug.WriteLine("Params");
            foreach (var item in _parameters)
            {
                Debug.WriteLine("   [" + item.Key + "](" + item.Value + ")");
            }
#endif
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

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            httpClient.DefaultRequestHeaders.CacheControl= new CacheControlHeaderValue();
            httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;
            httpClient.DefaultRequestHeaders.CacheControl.MaxAge = TimeSpan.Zero;
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //httpClient.DefaultRequestHeaders.CacheControl.NoCache = true;



            httpClient.Timeout = TimeSpan.FromSeconds(20);

            HttpResponseMessage response = await httpClient.GetAsync(_url + _action + "?" + getData);
            httpClient.Dispose();
#if DEBUG
            Debug.WriteLine("Responce: " + response.StatusCode);
#endif
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Lstatus = _status;
                _status = response.StatusCode.ToString();
                //return;
            }
            
            var responseString = await response.Content.ReadAsStringAsync();
            _responseStatus = response.StatusCode;
#if DEBUG
            Debug.WriteLine("Data: ");
            Debug.WriteLine(responseString);
#endif
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
            else
            {
                throw new Exception("[SendPost ]Status != OK ("+ response.StatusCode + ")");
            }
        }

        
        


        
        #endregion
    }
}
