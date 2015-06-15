using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VoyIteso.Class
{
    class HttpRequest
    {

        public enum ApiConnectorResponse
        {
            NoRequest,
            Ok,
            NetworkError,
            TimeOut,
            Failed,
            Other
        }
        //Attributes*************************************************************
        #region Attributes
        List<KeyValuePair<string, string>> _parameters;//Parameters to send
        ApiConnectorResponse _responseStatus;
        string data;
        string status;
        DispatcherTimer TimeOutTimer = new DispatcherTimer();
        int timeOut;//In seconds
        string _url = @"http://voy.cocoapps.mx/VOYAPI";
        string _action;
        HttpWebRequest _request;
        #endregion


        //Propieties*************************************************************
        #region Propieties
        public string Data { get { return data; } }
        public string Status { get { return status; } }
        public ApiConnectorResponse ResponseStatus { get { return _responseStatus; } }


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
            TimeOutTimer.Interval = TimeSpan.FromSeconds(timeOut);
            TimeOutTimer.Tick += RequestTimeOut;
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
        public void sendPost()
        {
            if (_action == "")//If url is empty or is busy then do nothing
            {
                return;
            }
            //_busy = true;//block the api connector
            _request = (HttpWebRequest)WebRequest.Create(_url + _action);//set url+action

            string postData = "";//Data to send

            bool firstData = true;//Is the first value?
            foreach (var item in _parameters)
            {
                if (firstData)//Is the first value?
                {
                    firstData = false;
                }
                else
                {
                    postData += "&";
                }
                postData += item.Key + "=" + item.Value;
            }

            var data = Encoding.ASCII.GetBytes(postData);//Encode data

            _request.Method = "POST";//Set action to post
            _request.ContentType = "application/x-www-form-urlencoded";
            _request.ContentLength = data.Length;
            try
            {
                using (var stream = _request.GetRequestStream())//Append Post data
                {
                    stream.Write(data, 0, data.Length);
                }
                TimeOutTimer.Start();
                _request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);//Set Callback
            }
            catch (System.Net.WebException e)
            {
                _responseStatus = ApiConnectorResponse.NetworkError;
                OnResponseObtained(EventArgs.Empty);
            }

        }

        //Send get
        public void sendGet()
        {
            if (_action == "")//If url is empty or is busy then do nothing
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
            _request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);//Set Callback
        }

        //Receive responce callback
        void FinishWebRequest(IAsyncResult result)
        {
            TimeOutTimer.Stop();//Stop timer from timing out

            _request.EndGetResponse(result);//end request
            var response = (HttpWebResponse)_request.GetResponse();//get result

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();//read result
            data = responseString;
            status = response.StatusDescription;
            if (status == "OK")
            {
                _responseStatus = ApiConnectorResponse.Ok;
            }

            OnResponseObtained(EventArgs.Empty);//fire eventPor q
        }

        private void RequestTimeOut(object sender, EventArgs e)
        {
            TimeOutTimer.Stop();
            Console.WriteLine("TimeOut");
            _responseStatus = ApiConnectorResponse.TimeOut;
            OnResponseObtained(EventArgs.Empty);//fire eventPor q
        }


        #endregion
    }
}
