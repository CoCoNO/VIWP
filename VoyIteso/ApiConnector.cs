using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace ApiConectorTest
{
    class ApiConnector
    {
        //Connects to voyitesoapi 
        //Atributes
        static ApiConnector _instance;//Singleton Instance
        string _url;
        List<KeyValuePair<string, string>> _parameters;//Parameters to send
        HttpWebRequest _request;
        string _token;
        bool _busy;



        //Propities
        public static ApiConnector instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ApiConnector();
                }
                return _instance;
            }
        }




        //Methods
        //Singleton constructor
        private ApiConnector()
        {
            _parameters = new List<KeyValuePair<string, string>>();
            _busy = false;
        }
        //set url
        public void setUrl(string url)
        {
            _url = url;
        }
        //Set Parameters
        public void setParameter(string paramName, string paramValue)
        {
            _parameters.Add(new KeyValuePair<string, string>(paramName, paramValue));
        }

        //send Post
        public void sendPost()
        {
            if (_url == "" || _busy)//If url is empty or is busy then do nothing
            {
                return;
            }
            _busy = false;//block the api connector
            _request = (HttpWebRequest)WebRequest.Create(_url);//set url

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

            using (var stream = _request.GetRequestStream())//Append Post data
            {
                stream.Write(data, 0, data.Length);
            }
            _request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);//Set Callback
        }
        //Send get
        //Recive responce callback
        void FinishWebRequest(IAsyncResult result)
        {
            _request.EndGetResponse(result);
            var response = (HttpWebResponse)_request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        //Responce event
    }
}
