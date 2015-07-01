using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VoyIteso.Class;
using Windows.Storage.Streams;


namespace VoyIteso.Class
{
    class ApiConnector
    {

        //Connects to voyitesoapi 

        //Atributies**************************************
        #region Atributies

        static ApiConnector _instance;//Singleton Instance

        IsolatedStorageSettings settings;
        string _token,_pid;
        User activeUser;


        #endregion





        //properties*****************************************************************************************
        #region Properties

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



        public User ActiveUser { get { return activeUser; } }
        #endregion
        




        //Events*************************************************************************************
        #region Events

        public EventHandler LoginDone;
        protected virtual void OnLoginDone(EventArgs e)
        {


            EventHandler handler = LoginDone;
            if (handler != null)
            {
                handler(this, e);
            }
            //clear resources
        }
        #endregion

        //Methods************************************************************************************
        #region Methods
        //Singleton constructor
        private ApiConnector()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;

        }





        public async Task   createUserFromToken()
        {

            HttpRequest r = new HttpRequest();
            r.setAction("/perfil/"+_pid+"/ver");
            r.setParameter("security_token", _token);
            await r.sendGet();
            activeUser = getUserFromJson(r.Data);
        }
        #endregion



        //Junk
        //Responce event


        //Actions
        public bool isLoggedIn()
        {
            
            try
            {
                string tToken=(string)settings["security_token"];
                string tPID=(string)settings["perfil_id"];


                _token = tToken;
                _pid = tPID;
                
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }

        }
        public void logOut()
        {
            settings.Remove("security_token");
            settings.Remove("perfil_id");
            settings.Remove("nombre_completo");

        }
        HttpRequest loginRequest;
        public async Task logIn(string user, string password)
        {

            loginRequest = new HttpRequest();
            loginRequest.setAction(@"/seguridad/login");

            loginRequest.setParameter("correo", user);
            loginRequest.setParameter("password", password);
            //loginRequest.setParameter("correo", "ie800001");
            //loginRequest.setParameter("password", "PruebaQA2015");
            loginRequest.ResponseObtained += LoginResponseObtained;

            await loginRequest.sendPost();

            //loginRequest.Data;
            if (loginRequest.Status=="OK")
            {
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(loginRequest.Data);
                //activeUser = getUserFromJson(loginRequest.Data);
                settings.Add("security_token", rootJson.security_token);

                settings.Add("perfil_id", rootJson.perfil_id);
                settings.Add("nombre_completo", rootJson.nombre_completo);
                //settings["perfil_id"];

                activeUser = new User();
                activeUser.Name = rootJson.nombre;
                activeUser.profileID = rootJson.perfil_id;
                _token = rootJson.security_token;
                UpdateCurrentProfileImage();

            }
            else if (loginRequest.Status == "Credenciales incorrectas")
            {
                throw new BadLoginExeption();
            }
            else
            {
                throw new TimeoutException(loginRequest.Status);
            }
            
        }

        public class BadLoginExeption : Exception
        {
            
        }
        //get user form json
        public User getUserFromJson(string jsonResponse)
        {
            if (jsonResponse != null)
            {
                User user = new User();
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                if (rootJson.estatus == 1)
                {
                    user.Token = _token;
                    user.completeName = (string)settings["nombre_completo"]; ;
                    user.Name = rootJson.perfil.nombre;
                    user.profileID = rootJson.perfil.perfilId.ToString();

                    return user;
                }
                else
                    return null;
            }
            else
                return null;


        }
        void LoginResponseObtained(object sender, EventArgs e)
        {
            loginRequest.ResponseObtained -= LoginResponseObtained;
            Console.WriteLine("LoginDone");
            //Check json for status
            if (loginRequest.Data != null)//If we got data
            {
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(loginRequest.Data);//Parse Json
                if (rootJson.estatus != 0)//if status is not an error
                {
                    _token = rootJson.security_token;//Copy the token

                }
                /*activeUser = new User();
                activeUser.Name = rootJson.perfil.nombre;
                activeUser.profileID = rootJson.perfil.perfilId.ToString();
                OnLoginDone(EventArgs.Empty);//fire event login done*/

                OnLoginDone(EventArgs.Empty);
            }
        }


        public async void UpdateCurrentProfileImage()
        {
            Uri uri = new Uri(@"https://aplicacionesweb.iteso.mx/VOYAPI/perfil/imagen/" + _pid + "?security_token=" + _token);
            activeUser.Avatar=  await LoadImage(uri);
        }
        public async Task GetProfileImage(string _pid) 
        {
            Uri uri = new Uri(@"https://aplicacionesweb.iteso.mx/VOYAPI/perfil/imagen/"+_pid+"?security_token="+_token);

            HttpClient client = new HttpClient();

            byte[] bytes = await client.GetByteArrayAsync(uri);
            //var streamImage = await client.GetByteArrayAsync(uri)
            
            //activeUser.Avatar = await LoadImage(uri);
            //var webClient = new WebClient();

            //webClient.DownloadStringAsync(uri);
            //BitmapImage image = new BitmapImage(uri); 

            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            //BitmapImage p = new BitmapImage();

            activeUser.Avatar = image;
        }

        void p_ImageOpened(object sender, RoutedEventArgs e)
        {
            
        }

        public async static Task<BitmapImage> LoadImage(Uri uri)
        {
            //BitmapImage bitmapImage = new BitmapImage();

            BitmapImage img = new BitmapImage(uri);
            return img;
        }
    }
}





































/*using Newtonsoft.Json;
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
using VoyIteso.Resources;

namespace VoyIteso.Class
{
    class ApiConnector
    {

        /// <summary>
        /// ApiConnector has a Singleton instance
        /// </summary>

        private static ApiConnector _instance;

        //----------Singleton 
        private ApiConnector() { }
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
        //-----------



        public event EventHandler responseChanged;
        public event EventHandler exceptionChanged;

        private string connectorUrl;
        private string parameters = null;
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

        
        public void UpdateUrl(string actionUrl)
        {
            this.connectorUrl = AppResources.ApiBaseUrl + actionUrl;
        }

        public void setParametersUrl(string value)
        {
            this.parameters = value;
        }

        public void setUrlParametersUrl()
        {
            this.connectorUrl += this.parameters;
        }

        public void SendGetRequest()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += client_OpenReadCompleted;
            client.OpenReadAsync(new Uri(connectorUrl));
        }

        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                Stream streamResponse = e.Result;
                StreamReader streamReader = new StreamReader(streamResponse);
                jsonResponse = streamReader.ReadToEnd();
            }
            catch(Exception ex)
            {
                throwException = "Ocurrio un error al tratar de conectarse al servidor. Verifique su conexión a Internet";
            }
        }

        public void SendPostRequest()
        {
            //For Post request need to set the url before with the specific action
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(connectorUrl);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);

        }

        public void SendPostImageRequest(byte[] photoStream, string userToken, string fileName)
        {
            Dictionary<string,string> parameters = new Dictionary<string, string>();
            parameters.Add("security_token", userToken);
            //UploadFilesToServer(new Uri(this.connectorUrl), parameters, fileName ,"application/octet-stream", photoStream);
            UploadFilesToServer(new Uri(this.connectorUrl), parameters, fileName, "image/jpeg", photoStream);

        }

        private void UploadFilesToServer(Uri uri, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            //string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            string boundary = DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.BeginGetRequestStream((result) =>
            {
            try
            {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                    using (Stream requestStream = request.EndGetRequestStream(result))
                    {
                        WriteMultipartForm(requestStream, boundary, data, fileName, fileContentType, fileData);
                    }
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    jsonResponse = streamReader.ReadToEnd();
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }, null);
            }
                catch (Exception)
            {

                }
            }, httpWebRequest);
            }

        private void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "–-\r\n");
            /// the form data, properly formatted
            //string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\";\r\nContent-Type:Text/plain\r\n\r\n{1}";
            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
        {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }
            
            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName ,fileContentType));
            /// Write the file data to the stream.
            WriteToStream(s, fileData);
            WriteToStream(s, trailer);
        }

        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes byte array to stream. Author : Farhan Ghumra
        /// </summary>
        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        private void GetRequestStreamCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            Stream postStream = request.EndGetRequestStream(asyncResult);
            //string postData;
            //every add values on post
            //postData = "correo=&password=";
            byte[] byteArray = Encoding.UTF8.GetBytes(this.parameters);
            postStream.Write(byteArray, 0, this.parameters.Length);
            postStream.Close();
            
            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
                
        }

        private void GetResponseCallback(IAsyncResult asyncResult)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)asyncResult.AsyncState;
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(asyncResult);
                Stream streamResponse = httpResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                //var responseString = streamReader.ReadToEnd();
                //jsonResponse = JObject.Parse(responseString);
                jsonResponse = streamReader.ReadToEnd();
                streamReader.Close();
                streamResponse.Close();
                httpResponse.Close();
                
            }
            catch(Exception e){        
                throwException = "Ocurrio un error al tratar de conectarse al servidor. Verifique su conexión a Internet";
            }
        }

        public User getUserFromJson()
        {
            if (jsonResponse != null)
            {
                User user = new User();
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                if (rootJson.estatus == 1)
                {
                    user.Token = rootJson.security_token;
                    user.completeName = rootJson.nombre_completo;
                    user.Name = rootJson.nombre;
                    user.profileID = rootJson.perfil_id;

                    return user;
                }
                else
                    return null;
            }
            else
                return null;

            
        }

        public List<RouteResult> getRoutesResult()
        {
            if (jsonResponse != null)
            {
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                if (rootJson.estatus != 0)
                {
                    List<RouteResult> routes = new List<RouteResult>();
                    foreach (Ruta r in rootJson.rutas)
                    {
                        routes.Add(new RouteResult(r));
                    }
                    return routes;
                }
                else
                {
                    this.throwException = "Ocurrio un error al mandar los datos al servidor, vuelve a intentarlo";
                    return null;
                }
            }
            else
            {
                this.throwException = "Ocurrio un error al mandar los datos al servidor, vuelve a intentarlo";
                return null;
            }
        }

        public bool getLiftRequestResult()
        {
            if (jsonResponse != null)
            {
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                return Convert.ToBoolean(rootJson.estatus);
            }
            else
                return false;
        }

        public bool getEditProfileRequestResult()
        {
            if (jsonResponse != null)
            {
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                return Convert.ToBoolean(rootJson.estatus);
            }
            return false;
        }
        
        public Perfil getProfile()
        {
            if (jsonResponse != null)
            {
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                if (rootJson.estatus != 0)
                    return rootJson.perfil;
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}*/
