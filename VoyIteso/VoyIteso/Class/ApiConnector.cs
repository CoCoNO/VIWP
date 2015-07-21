using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VoyIteso.Class;
using VoyIteso.Pages;
using Windows.Storage;
using Windows.Storage.Streams;


namespace VoyIteso.Class
{
    class ApiConnector
    {


        //Atributies**************************************
        #region Atributies

        static ApiConnector _instance;//Singleton Instance

        IsolatedStorageSettings settings;
        string _token,_pid;
        User _activeUser;

        #endregion





        //properties*****************************************************************************************
        #region Properties

        public static ApiConnector Instance
        {
            get { return _instance ?? (_instance = new ApiConnector()); }
        }

        public bool IsLoggedIn { get; private set; }


        public User ActiveUser { get { return _activeUser; } }
        #endregion
        




        //Events*************************************************************************************
        #region Events

        /*public EventHandler LoginDone;
        protected virtual void OnLoginDone(EventArgs e)
        {


            EventHandler handler = LoginDone;
            if (handler != null)
            {
                handler(this, e);
            }
            //clear resources
        }*/
        #endregion

        //Methods************************************************************************************
        #region Methods
        //Singleton constructor
        private ApiConnector()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;

        }

        public bool CheckIfLoggedIn()//Todo  Move to methods
        {

            try
            {
                string tToken = (string)settings["security_token"];
                string tPID = (string)settings["perfil_id"];


                _token = tToken;
                _pid = tPID;
                IsLoggedIn = true;
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }

        }



        /*public async Task   createUserFromToken()
        {

            HttpRequest r = new HttpRequest();
            r.setAction("/perfil/"+_pid+"/ver");
            r.setParameter("security_token", _token);
            await r.sendGet();
            _activeUser = getUserFromJson(r.Data);
        }*/

        public User GetUserFromJson(string jsonResponse)
        {
            if (jsonResponse != null)
            {
                User user = new User();
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(jsonResponse);
                if (rootJson.estatus == 1)
                {
                    user.Token = _token;
                    user.completeName = (string)settings["nombre_completo"];
                    ;
                    user.Name = rootJson.perfil.nombre;
                    user.profileID = rootJson.perfil.perfilId.ToString();

                    user.profile = rootJson.perfil;
                    return user;
                }

            }

            return null;


        }



        async Task<User> GetUserById(string uID)
        {
            if (_token == null) return null;

            HttpRequest r = new HttpRequest();

            r.setAction("/perfil/" + uID + "/ver");
            r.setParameter("security_token", _token);

            await r.sendGet();
            return r.Status == "OK" ? GetUserFromJson(r.Data) : null;
        }

        
        public async Task<string> UploadImage(Stream file,string filename)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(HttpRequest.Url);
            MultipartFormDataContent form = new MultipartFormDataContent();
            HttpContent content = new StringContent(_token);
            form.Add(content, "security_token");

            var stream = file;
            content = new StreamContent(stream);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = filename
            };
            form.Add(content);
            var response = await client.PostAsync("/perfil/subir_imagen_perfil", form);
            return response.Content.ReadAsStringAsync().Result;
        }
        public async Task GetActiveUserFromSettings()
        {
            if (settings.Contains("perfil_id"))
            {
                _activeUser = await GetUserById((string)settings["perfil_id"]);
                if (_activeUser== null)
                {
                    throw new BadLoginExeption();
                }

            }

        }
        #endregion



        //Junk
        //Responce event


        //Actions
#region Actions
        
        public void logOut()
        {
            try
            {
                settings.Remove("security_token");
                settings.Remove("perfil_id");
                settings.Remove("nombre_completo");
            }
            catch (Exception)
            {
                
                throw;
            }
            

        }
        
        public async Task LogIn(string user, string password)
        {
            //Create Request
            HttpRequest loginRequest = new HttpRequest();
            
            //Set Action
            loginRequest.setAction(@"/seguridad/login");


            //Set Parameters
            loginRequest.setParameter("correo", user);
            loginRequest.setParameter("password", password);
            //Debug Credential
            //loginRequest.setParameter("correo", "ie800001");
            //loginRequest.setParameter("password", "PruebaQA2015");

            //Wait response 
            await loginRequest.sendPost();

            //Check if responce is ok
            if (loginRequest.Status=="OK")
            {
                //Parse json
                RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(loginRequest.Data);
                
                //Extract login info  and save it to settings
                settings.Add("security_token", rootJson.security_token);
                settings.Add("perfil_id", rootJson.perfil_id);
                settings.Add("nombre_completo", rootJson.nombre_completo);
                
                //Create User 
                //Todo use get  get user by id
                //_activeUser = new User();
                //_activeUser.Name = rootJson.nombre;
                //_activeUser.profileID = rootJson.perfil_id;
                _token = rootJson.security_token;

                _pid = rootJson.perfil_id;
                _activeUser = await GetUserById(_pid);
                
                
                
                //_activeUser.profile = rootJson.perfil;

            }
            else if (loginRequest.Status == "Credenciales incorrectas")
            {
                throw new BadLoginExeption();
                //If credentials are wrong trow exeption
            }
            else
            {
                throw new TimeoutException(loginRequest.Status);
                //If timeout then trow exception
            }
            
        }

        public class BadLoginExeption : Exception
        {
            
        }

        public async Task<Appointment[]> LoadCurrentMonthLifts()
        {
            if (_token == null) return null;

            HttpRequest r = new HttpRequest();

            r.setAction("/perfil/aventones_mes_actual");
            r.setParameter("security_token", _token);

            await r.sendGet();
            //r.Data
            if (r.Status == "OK" && r.Data != String.Empty)
            {

                NextLifts rootJson = JsonConvert.DeserializeObject<NextLifts>(r.Data);
                if (rootJson.estatus == 1)
                {
                    List<Appointment> appointments = new List<Appointment>();
                    foreach (var app in rootJson.aventones)
                    {
                        var p = new Appointment();
                        p.Details = "De " + app.texto_origen + " a " + app.texto_destino + " a las " + app.hora_llegada +
                                    " el dia " + app.fecha + " con " +
                                    (app.conductor == string.Empty ? app.pasajero : app.conductor) + " como " + app.rol + ". Estatus " + app.estatus_aventon;
                        var st = app.fecha.Substring(4, 4);
                        p.StartDate = new DateTime(
                            int.Parse(app.fecha.Substring(4, 4)),//yyyy
                            int.Parse(app.fecha.Substring(2, 2)),//mm
                            int.Parse(app.fecha.Substring(0, 2)),//dd
                            int.Parse(app.hora_llegada.Substring(0, 2)),//hh
                            int.Parse(app.hora_llegada.Substring(3, 2)),//mm
                            int.Parse(app.hora_llegada.Substring(6, 2)));//ss
                        p.Location = app.texto_origen;
                        p.EndDate = p.StartDate.AddHours(1);
                        p.Subject = "De " + app.texto_origen + " a " + app.texto_destino;
                        appointments.Add(p);
                    }

                    return appointments.ToArray();
                }

            }
            return null;
        }

        //get user form json
        

        public void SaveUserDataToCloud()
        {
            var r= new HttpRequest();
            r.setAction(@"/perfil/editar");
            r.setParameter("security_token", _token);
            r.setParameter("descripcion", _activeUser.profile.descripcion);
            r.setParameter("otrasCostumbres", _activeUser.profile.otrasCostumbres);
            r.setParameter("fuma", _activeUser.profile.fuma.Value.ToString());
            r.setParameter("aire", _activeUser.profile.aire.Value.ToString());
            r.setParameter("musica", _activeUser.profile.musica.Value.ToString());
            r.setParameter("mascota", _activeUser.profile.mascota.Value.ToString());
            r.setParameter("platicar", _activeUser.profile.platicar.ToString());

            r.sendPost();
        }

        public async Task<Rutas> SearchRoute(string origen, string destino, string fecha, double latitud_destino, double longitud_destino, double latitud_origen, double longitud_origen, string hora)
        {
            var r = new HttpRequest();
            r.setAction(@"/ruta/busqueda");
            r.setParameter("security_token", _token);
            r.setParameter("origen",origen );
            r.setParameter("destino", destino);
            r.setParameter("fecha", fecha);
            r.setParameter("latitud_destino",latitud_destino.ToString() );
            r.setParameter("longitud_destino",longitud_destino.ToString());
            r.setParameter("latitud_origen",latitud_origen.ToString() );
            r.setParameter("longitud_origen",longitud_origen.ToString() );
            r.setParameter("hora", hora);
            

            r.sendPost();

            if (r.Status == "Ok" || r.Data== String.Empty)
                return null;

            Rutas rootJson = JsonConvert.DeserializeObject<Rutas>(r.Data);
            return rootJson;
        }

        public void UpdateCurrentProfileImage()
        {
            Uri uri = new Uri(@"https://aplicacionesweb.iteso.mx/VOYAPI/perfil/imagen/" + _pid + "?security_token=" + _token);

            BitmapImage img = new BitmapImage(uri);
            img.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            img.ImageOpened += img_ImageOpened;
            _activeUser.Avatar = img;
        }
        //Todo Add GetUserImgById(_id);
        void img_ImageOpened(object sender, RoutedEventArgs e)
        {
            _activeUser.Avatar = (BitmapImage)(sender);
        }
#endregion

        
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
            //For Post request need to set the Url before with the specific action
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
