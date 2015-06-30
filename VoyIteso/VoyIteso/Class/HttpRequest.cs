using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VoyIteso.Class
{
    class HttpRequest
    {


        //Attributes*************************************************************
        #region Attributes
        List<KeyValuePair<string, string>> _parameters;//Parameters to send
        HttpStatusCode _responseStatus;
        string data;
        string status;
        //DispatcherTimer TimeOutTimer = new DispatcherTimer();
        int timeOut;//In seconds
        //string _url = @"http://voy.cocoapps.mx/VOYAPI";
        string _url = @"https://aplicacionesweb.iteso.mx/VOYAPI";
        string _action;
        HttpWebRequest _request;
        




        #endregion


        //Propieties*************************************************************
        #region Propieties
        public string Data { get { return data; } }
        public string Status { get { return status; } }
        public HttpStatusCode ResponseStatus { get { return _responseStatus; } }


        #endregion


        //Events*************************************************************
        #region Events
        public EventHandler ResponseObtained;
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
        }
        #endregion


        //Methods*************************************************************
        #region Methods
        public HttpRequest()
        {
            _parameters = new List<KeyValuePair<string, string>>();
            timeOut = 6;
            //TimeOutTimer.Interval = TimeSpan.FromSeconds(timeOut);
            //TimeOutTimer.Tick += RequestTimeOut;
        }

        //set url
        public void setAction(string action)
        {
            //_url = url;
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
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(_url + _action, new FormUrlEncodedContent(_parameters));

                if (response.StatusCode!= HttpStatusCode.OK)
                {
                    status = response.StatusCode.ToString();
                    return;
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
                            data = responseString;
                            status = "OK";
                        }
                        else
                        {
                            status = rootJson.error;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                
            //    throw;
            }
            

        }
        
        //Send get
        public async Task sendGet()
        {

            string getData = "";//Data to send

            bool firstData = true;//Is the first value?
            foreach (var item in _parameters)
            {
                if (firstData)//Is the first value?
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
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(_url + _action+"?"+getData);

                if (response.StatusCode!= HttpStatusCode.OK)
                {
                    status = response.StatusCode.ToString();
                    return;
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
                            data = responseString;
                            status = "OK";
                        }
                        else
                        {
                            status = rootJson.error;
                        }
                    }
                }
                }
            catch (TaskCanceledException)
            {
                
            //    throw;
            }
            /*if (_action == "")//If url is empty or is busy then do nothing
            {
                return;
            }

            string getData = "";//Data to send

            bool firstData = true;//Is the first value?
            foreach (var item in _parameters)
            {
                if (firstData)//Is the first value?
                {
                    firstData = false;
                }
                else
                {
                    getData += "&";

                }
                getData += item.Key + "=" + item.Value;
            }

            _request = (HttpWebRequest)WebRequest.Create(_url + _action + @"/" + getData);//set url+action

            _request.Method = "GET";//Set action to post
            _request.ContentType = "application/x-www-form-urlencoded";

            TimeOutTimer.Start();
            _request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);//Set Callback*/
        }

        //Receive responce callback
        void FinishWebRequest(IAsyncResult result)
        {
            
        }

        private void RequestTimeOut(object sender, EventArgs e)
        {

        }


        #endregion
    }
}
