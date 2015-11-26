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
using RestSharp;
using System.Device.Location;
using Windows.Devices.Geolocation;
namespace VoyIteso.Class
{
    class ApiConnector
    {
        //Todo Add GetUserImgById(_id);
        //---------------------------SubClases---------------------------
        #region SubClasses
        public class BadLoginExeption : Exception
        {

        }
        #endregion




        //---------------------------Atributies---------------------------
        #region Atributies

        static ApiConnector _instance;//Singleton Instance

        IsolatedStorageSettings settings;

        string _token, _pid;
        User _activeUser;
        private int _ranData = 0;//RandData to get the newest request



        #endregion




        //---------------------------Properties---------------------------
        #region Properties

        public static ApiConnector Instance
        {
            get
            {
                return _instance ?? (_instance = new ApiConnector());
            }
        }

        public bool IsLoggedIn { get; private set; }


        public User ActiveUser { get { return _activeUser; } }




        #endregion




        //---------------------------Events---------------------------
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




        //---------------------------Methods---------------------------
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


        public static string EncodeLocation(IEnumerable<GeoCoordinate> points)
        {
            var str = new StringBuilder();

            var encodeDiff = (Action<int>)(diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;
                int rem = shifted;
                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                    rem >>= 5;
                }
                str.Append((char)(rem + 63));
            });

            int lastLat = 0;
            int lastLng = 0;
            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.Latitude * 1E5);
                int lng = (int)Math.Round(point.Longitude * 1E5);
                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);
                lastLat = lat;
                lastLng = lng;
            }
            return str.ToString();
        }


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

                    if (user.profile.platicar == null)
                    {
                        user.profile.platicar =
                        0;
                    }

                    if (user.profile.ultima_conexion ==
                    null)
                    {
                        user.profile.ultima_conexion =
                        0;
                    }

                    if (user.profile.aire ==
                    null)
                    {
                        user.profile.aire =
                        0;
                    }

                    if (user.profile.carrera ==
                    null)
                    {
                        user.profile.carrera = "";
                    }
                    if (user.profile.descripcion ==
                    null)
                    {
                        user.profile.descripcion = "";
                    }

                    if (user.profile.otrasCostumbres ==
                    null)
                    {
                        user.profile.otrasCostumbres =
                        "";
                    }

                    if (user.profile.musica ==
                    null)
                    {
                        user.profile.musica =
                        0;
                    }

                    if (user.profile.mascota ==
                    null)
                    {
                        user.profile.mascota =
                        0;
                    }

                    if (user.profile.aventones_recibidos_count ==
                    null)
                    {
                        user.profile.aventones_recibidos_count =
                        0;
                    }

                    if (user.profile.aventones_dados_count == null)
                    {
                        user.profile.aventones_dados_count =
                        0;
                    }
                    if (user.profile.fuma == null)
                    {
                        user.profile.fuma =
                        0;
                    }
                    if (user.profile.rating == null)
                    {
                        user.profile.rating =
                        0;
                    }
                    if (user.profile.rutas_count == null)
                    {
                        user.profile.rutas_count =
                        0;
                    }



                    return user;
                }

            }

            return null;


        }

        public BitmapImage GetUserImageById(int id)
        {
            Uri uri = new Uri(HttpRequest.Url + @"/perfil/imagen/" + id + "?security_token=" + _token + "&randData=" + _ranData++);

            BitmapImage img = new BitmapImage(uri);
            img.CreateOptions = BitmapCreateOptions.None;
            return img;
        }

        public async Task<User> GetUserById(string uID)
        {
            if (_token == null) return null;

            HttpRequest r = new HttpRequest();

            r.setAction("/perfil/" + uID + "/ver");
            r.setParameter("security_token", _token);

            await r.sendGet();
            return r.Status == "OK" ? GetUserFromJson(r.Data) : null;
        }

        public async Task<Notifications> NotificationsGet(int page = 1)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest("/perfil/notificaciones", Method.GET);

            r.AddParameter("security_token", _token);
            r.AddParameter("pagina", page);
            r.AddParameter("randData", _ranData++);
            var rs = await c.ExecuteTaskAsync<Notifications>(r);

            return rs.Data;

            //return null;
        }



        public async Task<ResponceObject> RouteCreate(string texto_origen, string texto_destino, DateTime fecha_inicio, DateTime fecha_fin, string dias, int numero_personas, IEnumerable<GeoCoordinate> puntos)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest("/ruta/crear", Method.POST);

            r.AddParameter("security_token", _token);
            r.AddParameter("perfil_id", ActiveUser.profileID);

            r.AddParameter("texto_origen", texto_origen);
            r.AddParameter("texto_destino", texto_destino);

            r.AddParameter("latitud_origen", puntos.ElementAt(0).Latitude);
            r.AddParameter("longitud_origen", puntos.ElementAt(0).Longitude);

            r.AddParameter("latitud_destino", puntos.ElementAt(puntos.Count() - 1).Latitude);
            r.AddParameter("longitud_destino", puntos.ElementAt(puntos.Count() - 1).Longitude);


            r.AddParameter("fecha_inicio", string.Format("{0}-{1}-{2}", fecha_inicio.Day, fecha_inicio.Month, fecha_inicio.Year));
            r.AddParameter("fecha_fin", string.Format("{0}-{1}-{2}", fecha_fin.Day, fecha_fin.Month, fecha_fin.Year));
            r.AddParameter("hora", fecha_inicio.Hour);
            r.AddParameter("minuto", fecha_inicio.Minute);
            r.AddParameter("dias", dias);
            r.AddParameter("numero_personas", numero_personas);

            bool isFirst = true;
            string puntosIntermedios = "[";
            foreach (var item in puntos)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    puntosIntermedios += ",";
                }
                puntosIntermedios += "{\"lat\":" + item.Latitude + ",\"lng\":" + item.Longitude + "}";
            }



            
            puntosIntermedios += "]";
            r.AddParameter("puntos_intermedios", puntosIntermedios);

            string puntos_referencia = "{\"start\":{\"lat\":" + puntos.ElementAt(0).Latitude + ",\"lng\":"+ puntos.ElementAt(0).Longitude + "},\"end\":{\"lat\":" + puntos.ElementAt(puntos.Count() - 1).Latitude + ",\"lng\":" + puntos.ElementAt(puntos.Count() - 1).Longitude + "},\"waypoints\":\"[";
            //\"{\"start\":{\"lat\":20.606895,\"lng\":-103.41598999999997},\"end\":{\"lat\":20.710495,\"lng\":-103.41267399999998},\"waypoints\":\"[[20.658954,-103.44683070000002],[20.6680656,-103.38331089999997]]\"}\"
            for(int j =1; j<puntos.Count()-1;j++ )
            {
                puntos_referencia += "[" + puntos.ElementAt(j).Latitude + "," + puntos.ElementAt(j).Latitude + "]";
                if (j < (puntos.Count() - 2))
                {
                    puntos_referencia += ",";
                }
            }

            puntos_referencia += "]\"}";//]\"}\"
            r.AddParameter("puntos_referencia", puntos_referencia);
            r.AddParameter("polilinea_codificada", EncodeLocation(puntos));
            r.AddParameter("tipo_mapa", 4);

#if DEBUG
            Debug.WriteLine("Sending Post Request to (" + HttpRequest.Url + "/ruta/crear" + ")");
            Debug.WriteLine("Params");
            foreach (var item in r.Parameters)
            {
                Debug.WriteLine("   [" + item.Name + "](" + item.Value + ")");
            }
#endif
            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

#if DEBUG
            Debug.WriteLine("Responce");
            Debug.WriteLine(rs.Content);

#endif
            return rs.Data;

            //return null;
        }
        ///aventon/solicitar
        public async Task<ResponceObject> LiftRequest(int routeID, string requestMessage, DateTime requestDate)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/aventon/solicitar"), Method.POST);

            r.AddParameter("security_token", _token);

            r.AddParameter("ruta_id", routeID);

            r.AddParameter("mensaje_solicitud", requestMessage);

            r.AddParameter("fecha_solicitud", string.Format("{0}{1}-{2}-{3}", requestDate.Day < 10 ? "0" : "", requestDate.Day, requestDate.Month, requestDate.Year));

            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;
        }
        public async Task<ResponceObject> LiftAccept(int routeID, string Message = null)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/aventon/{0}/aceptar", routeID), Method.POST);

            r.AddParameter("security_token", _token);
            if (Message != null)
            {
                r.AddParameter("mensaje", Message);
            }

            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;
        }
        public async Task<ResponceObject> LiftReject(int routeID, string Message)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/aventon/{0}/rechazar", routeID), Method.POST);

            r.AddParameter("security_token", _token);

            r.AddParameter("mensaje", Message);


            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;
        }

        public async Task<ResponceObject> LiftCancel(int routeID, string Message)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/aventon/{0}/cancelar", routeID), Method.POST);

            r.AddParameter("security_token", _token);

            r.AddParameter("mensaje", Message);


            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;
        }

        public async Task<ResponceObject> RouteDelete(int routeID)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/ruta/{0}/eliminar", routeID), Method.POST);

            r.AddParameter("security_token", _token);
            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;
        }
        public async Task<Ruta> RouteGet(int routeID)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/ruta/{0}/ver", routeID), Method.GET);

            r.AddParameter("security_token", _token);
            r.AddParameter("randata", _ranData++);
            var rs = await c.ExecuteTaskAsync<RouteRes>(r);

            return rs.Data.ruta;
        }
        public async Task<Rutes> RouteGetAllByUserID(int UserID)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/perfil/{0}/rutas", UserID), Method.GET);

            r.AddParameter("security_token", _token);
            r.AddParameter("randata", _ranData++);
            var rs = await c.ExecuteTaskAsync<Rutes>(r);

            return rs.Data;
        }
        public async Task<ResponceObject> RouteEdit(int routeId, string texto_origen, string texto_destino, DateTime fecha_inicio, DateTime fecha_fin, string dias, int numero_personas, IEnumerable<GeoCoordinate> puntos)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest("/ruta/crear", Method.POST);

            r.AddParameter("security_token", _token);
            r.AddParameter("ruta_id", routeId);

            r.AddParameter("texto_origen", texto_origen);
            r.AddParameter("texto_destino", texto_destino);

            r.AddParameter("latitud_origen", puntos.ElementAt(0).Latitude);
            r.AddParameter("longitud_origen", puntos.ElementAt(0).Longitude);

            r.AddParameter("latitud_destino", puntos.ElementAt(puntos.Count() - 1).Latitude);
            r.AddParameter("longitud_destino", puntos.ElementAt(puntos.Count() - 1).Longitude);


            r.AddParameter("fecha_inicio", string.Format("{0}-{1}-{2}", fecha_inicio.Day, fecha_inicio.Month, fecha_inicio.Year));
            r.AddParameter("fecha_fin", string.Format("{0}-{1}-{2}", fecha_fin.Day, fecha_fin.Month, fecha_fin.Year));

            //r.AddParameter("hora", fecha_inicio.Hour);
            //r.AddParameter("minuto", fecha_inicio.Minute);
            r.AddParameter("dias", dias);
            r.AddParameter("numero_personas", numero_personas);



            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;

            //return null;
        }

        public async Task GetActiveUserFromSettings()
        {
            if (settings.Contains("perfil_id"))
            {
                _activeUser = await GetUserById((string)settings["perfil_id"]);
                if (_activeUser == null)
                {
                    LogOut();
                    //throw new BadLoginExeption();

                }

            }

        }
        #endregion




        //---------------------------Actions---------------------------
        #region Actions



        public async Task UpdateCurrentUserData()
        {
            _activeUser = await GetUserById(_pid);
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

            try
            {
                await loginRequest.sendPost();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return ;
                //throw;
            }
            

            //Check if responce is ok
            if (loginRequest.Status == "OK")
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
        public void LogOut()
        {
            try
            {
                settings.Remove("security_token");
                settings.Remove("perfil_id");
                settings.Remove("nombre_completo");
                IsLoggedIn = false;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<Mensajes> LiftMessagesGet(int liftID)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest("/aventon/" + liftID + "/chat", Method.GET);

            r.AddParameter("security_token", _token);
            r.AddParameter("randata", _ranData++);

            var rs = await c.ExecuteTaskAsync<Mensajes>(r);
            return rs.Data;
        }
        public async Task<ResponceObject> LiftMessagesSend(int routeID, string Message)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/aventon/{0}/nuevomensaje", routeID), Method.POST);

            r.AddParameter("security_token", _token);
            r.AddParameter("texto", Message);

            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;
        }

        /// <summary>
        ///     regresa lista de aventones pendientes por revisar.
        //


        /// </summary>
        /// <returns></returns>
        public async Task<LiftsToRate> LiftCheckIfRateNeeded()
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(@"/perfil/aventones_sin_evaluar", Method.GET);

            r.AddParameter("security_token", _token);
            r.AddParameter("perfil_id", ActiveUser.profileID);
            r.AddParameter("randata", _ranData++);


            var rs = await c.ExecuteTaskAsync<LiftsToRate>(r);

            return rs.Data;
        }


        public async Task<MiRed> GetMyNetwork()
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(@"/perfil/1591/mi_red", Method.GET);

            r.AddParameter("security_token", _token);
            r.AddParameter("perfil_id", ActiveUser.profileID);
            r.AddParameter("randata", _ranData++);


            var rs = await c.ExecuteTaskAsync<MiRed>(r);

            return rs.Data;
        }


        /***/


        public async Task<Evaluaciones> GetRatesByID(int rID = 0)
        {
            if (rID == 0)
            {
                rID = ActiveUser.profile.perfilId;
            }
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(String.Format(@"/perfil/{0}/evaluaciones", rID), Method.GET);

            r.AddParameter("security_token", _token);
            //r.AddParameter("perfil_id", ActiveUser.profileID);
            r.AddParameter("randata", _ranData++);


            var rs = await c.ExecuteTaskAsync<Evaluaciones>(r);

            return rs.Data;
        }

        /// <summary>
        /// LiftRate regresa status de operacion. 

        /// </summary>
        /// <param name="routeID"></param>
        /// <param name="succesful"></param>
        /// <param name="punctuality"></param>
        /// <param name="rating"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public async Task<ResponceObject> LiftRate(int routeID, int succesful, int punctuality, int rating, string Message)
        {
            var c = new RestClient(HttpRequest.Url);
            var r = new RestRequest(string.Format("/aventon/{0}/calificar", routeID), Method.POST);

            r.AddParameter("security_token", _token);
            r.AddParameter("fue_exitoso", succesful);
            r.AddParameter("puntualidad", punctuality);
            r.AddParameter("calificacion", rating);
            r.AddParameter("comentario", Message);


            var rs = await c.ExecuteTaskAsync<ResponceObject>(r);

            return rs.Data;
        }
        public async Task<Appointment[]> LoadCurrentMonthLifts()
        {
            if (_token == null) return null;

            HttpRequest r = new HttpRequest();

            //r.setAction("/perfil/aventones_mes_actual");

            r.setAction("/perfil/aventones_rango_mes");
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
                        p.Details = "Origen:\t" + app.texto_origen + "*Destino:\t" + app.texto_destino + "*Hora:\t\t" + app.hora_llegada +
                                     " *el dia " + app.fecha + " *con " +
                                     (app.conductor == string.Empty ? app.pasajero : app.conductor) + " como " + app.rol + ". Estatus " + app.estatus_aventon;
                        var st = app.fecha.Substring(4, 4);
                        p.StartDate = new DateTime(
                            int.Parse(app.fecha.Substring(4, 4)),//yyyy
                            int.Parse(app.fecha.Substring(2, 2)),//mm
                            int.Parse(app.fecha.Substring(0, 2)),//dd
                            int.Parse(app.hora_llegada.Substring(0, 2)),//hh
                            int.Parse(app.hora_llegada.Substring(3, 2)),//mm
                            int.Parse(app.hora_llegada.Substring(6, 2)));//ss

                        p.LiftID = app.id;
                        p.Location = app.texto_origen;
                        p.EndDate = p.StartDate.AddHours(1);
                        p.Subject = "De " + app.texto_origen + " a " + app.texto_destino;
                        p.OtroBabosoID = app.perfil_id.ToString();

                        appointments.Add(p);
                    }

                    return appointments.ToArray();
                }

            }
            return null;
        }

        //get user form json


        public async Task<int> SaveUserDataToCloud()//Returns 0 on ok -1 on error
        {
            var r = new HttpRequest();
            r.setAction(@"/perfil/editar");
            r.setParameter("security_token", _token);
            r.setParameter("descripcion", _activeUser.profile.descripcion);
            r.setParameter("otrasCostumbres", _activeUser.profile.otrasCostumbres);
            r.setParameter("fuma", _activeUser.profile.fuma.Value.ToString());
            r.setParameter("aire", _activeUser.profile.aire.Value.ToString());
            r.setParameter("musica", _activeUser.profile.musica.Value.ToString());
            r.setParameter("mascota", _activeUser.profile.mascota.Value.ToString());
            r.setParameter("platicar", _activeUser.profile.platicar.ToString());

            await r.sendPost();
            return r.Status == "OK" ? 0 : -1;
        }




        public async Task<string> UploadImage(byte[] b, string filename)
        {
            var client = new RestClient(HttpRequest.Url + @"/perfil/subir_imagen_perfil");
            var request = new RestRequest(Method.POST);

            request.AddFile("file", b, filename.Substring(filename.LastIndexOf('\\')), "image/jpeg");
            request.AddParameter("security_token", _token);
            try
            {
                Debug.WriteLine("Uploading File");
                var r = await client.ExecuteTaskAsync(request);
                Debug.WriteLine(r.Content);
                return r.Content;
            }
            catch (UriFormatException ex)
            {
                Debug.WriteLine("Exeption uploading file: " + ex.Message);
            }

            return null;
        }


        public async Task<Rutes> RouteSearch(string origen, string destino, string fecha, double latitud_destino, double longitud_destino, double latitud_origen, double longitud_origen, string hora)
        {
            var r = new HttpRequest();
            r.setAction(@"/ruta/busqueda");
            r.setParameter("security_token", _token);
            r.setParameter("origen", origen);
            r.setParameter("destino", destino);

            r.setParameter("distancia", "0.045");
            r.setParameter("distancia_destino", "0.045");

            r.setParameter("fecha", fecha);

            r.setParameter("fumador", "-1");
            r.setParameter("rol", "-1");
            r.setParameter("genero", "-1");
            r.setParameter("calificacion", "-1");

            r.setParameter("latitud_destino", latitud_destino.ToString());
            r.setParameter("longitud_destino", longitud_destino.ToString());
            r.setParameter("latitud_origen", latitud_origen.ToString());
            r.setParameter("longitud_origen", longitud_origen.ToString());

            r.setParameter("hora", hora);


            await r.sendPost();

            if (r.Data == String.Empty || r.Status != "OK")
                return null;

            Rutes rootJson = JsonConvert.DeserializeObject<Rutes>(r.Data);
            return rootJson;
        }

        public void UpdateCurrentProfileImage()
        {
            Uri uri = new Uri(HttpRequest.Url + @"/perfil/imagen/" + _pid + "?security_token=" + _token + "&randData=" + _ranData++);

            BitmapImage img = new BitmapImage(uri);
            img.CreateOptions = BitmapCreateOptions.None;
            img.ImageOpened += img_ImageOpened;
            _activeUser.Avatar = img;
        }

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
