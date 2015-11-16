using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoyIteso.Class
{

    public class Street
    {
        public string name { get; set; }
        public string addressLine { get; set; }

        public Street(Resource resource)
        {
            this.name = resource.name;
            this.addressLine = string.Format("{0} {1} {2} {3}", resource.address.addressLine, resource.address.locality, resource.address.adminDistrict, resource.address.countryRegion);
        }
    }

    public class Mensaje
    {
        public int id { get; set; }
        public int perfil_id { get; set; }
        public string texto { get; set; }
        public string tiempo { get; set; }
        public long fecha { get; set; }
        public string fecha_publicacion { get; set; }
    }

    public class Mensajes
    {
        public List<Mensaje> mensajes { get; set; }
        public int estatus { get; set; }
    }

    public class RouteResult
    {
        public string destiny { get; set; }
        public string creation_date { get; set; }
        public int profile_id { get; set; }
        public string user_name { get; set; }
        public string arrival_time { get; set; }
        public string points { get; set; }
        //public object end_date { get; set; }
        public int numer_people { get; set; }
        public GeoCoordinate destiny_coordinate { get; set; }
        public GeoCoordinate origin_coordinate { get; set; }
        public string origin { get; set; }
        public int route_id { get; set; }
        public string begin_date { get; set; }
        static public int ResultWidth { get; set; }
        static public int ResultHeight { get; set; }

        public RouteResult(Ruta ruta)
        {
            this.destiny = ruta.texto_destino;
            this.profile_id = ruta.perfil_id;
            this.user_name = ruta.persona_nombre;
            //this.end_date = ruta.fecha_fin;
            this.numer_people = ruta.numero_personas;
            this.origin = ruta.texto_origen;
            this.route_id = ruta.ruta_id;
            this.creation_date = getDate(ruta.fecha_creacion).ToString("dd-MM-yyyy");
            this.arrival_time = getDate(ruta.hora_llegada).ToString("hh:mm tt");
            this.begin_date = getDate(ruta.fecha_inicio).ToString("dd-MM-yyyy");
            //this.points = ruta.puntos;
            getPoints();
        }



        private DateTime getDate(long miliseconds)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return start.AddMilliseconds(miliseconds).ToLocalTime();
        }

        private void getPoints()
        {
            RootObject rootJson = JsonConvert.DeserializeObject<RootObject>(this.points);
            //this.origin_coordinate = new GeoCoordinate(rootJson.start.lat, rootJson.start.lng);
            //this.destiny_coordinate = new GeoCoordinate(rootJson.end.lat, rootJson.end.lng);
        }

    }

    public class Address
    {
        public string addressLine { get; set; }
        public string adminDistrict { get; set; }
        public string countryRegion { get; set; }
        public string locality { get; set; }
    }

    public class Resource
    {
        public string name { get; set; }
        public Address address { get; set; }
    }

    public class ResourceSet
    {
        public int estimatedTotal { get; set; }
        public List<Resource> resources { get; set; }
    }

    /*public class Ruta
    {
        public string texto_destino { get; set; }
        public long fecha_creacion { get; set; }
        public int perfil_id { get; set; }
        public string persona_nombre { get; set; }
        public long hora_llegada { get; set; }
        public string puntos { get; set; }
        public double latitud_origen { get; set; }
        public object fecha_fin { get; set; }
        public double longitud_origen { get; set; }
        public int numero_personas { get; set; }
        public double latitud_destino { get; set; }
        public string texto_origen { get; set; }
        public double longitud_destino { get; set; }
        public int ruta_id { get; set; }
        public long fecha_inicio { get; set; }
    }*/

    public class Start
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class End
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Perfil
    {
        public int perfilId { get; set; }
        public int evaluaciones_count { get; set; }
        public List<string> endpoints { get; set; }
        public string telefono { get; set; }
        public string descripcion { get; set; }
        public int? platicar { get; set; }
        public long? ultima_conexion { get; set; }
        public int? aire { get; set; }
        public string carrera { get; set; }
        public string nombre { get; set; }
        public string otrasCostumbres { get; set; }
        public int? musica { get; set; }
        public string edad { get; set; }
        public int? mascota { get; set; }
        public int? aventones_recibidos_count { get; set; }
        public int aventones_dados_count { get; set; }
        public int? fuma { get; set; }
        public int? rating { get; set; }
        public int? rutas_count { get; set; }
        public string correo { get; set; }
    }

    public class RootObject
    {
        public List<ResourceSet> resourceSets { get; set; }
        public int statusCode { get; set; }
        public string nombre { get; set; }
        public string perfil_id { get; set; }
        public int estatus { get; set; }
        public string security_token { get; set; }
        public string nombre_completo { get; set; }
        public List<Ruta> rutas { get; set; }
        public Start start { get; set; }
        public End end { get; set; }
        //public List<object> waypoints { get; set; }
        public Perfil perfil { get; set; }
        public string error { get; set; }

    }//*/
    /*
    public class Perfil
    {
        public int perfilId { get; set; }
        public int evaluaciones_count { get; set; }
        public List<object> endpoints { get; set; }
        public string telefono { get; set; }
        public object descripcion { get; set; }
        public object platicar { get; set; }
        public object ultima_conexion { get; set; }
        public object aire { get; set; }
        public string carrera { get; set; }
        public string nombre { get; set; }
        public object otrasCostumbres { get; set; }
        public object musica { get; set; }
        public string edad { get; set; }
        public object mascota { get; set; }
        public int aventones_recibidos_count { get; set; }
        public int aventones_dados_count { get; set; }
        public object fuma { get; set; }
        public int rating { get; set; }
        public int rutas_count { get; set; }
        public string correo { get; set; }
    }
    */
    /*public class RootObject
    {
        public string message { get; set; }
        public Perfil perfil { get; set; }
        public int estatus { get; set; }
    }*/
    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public class ResponceObject
    {
        //public int statusCode;
        public int estatus { get; set; }
        public string error { get; set; }
        public string message { get; set; }
    }
    //public class Aventone
    //{
    //    public int id { get; set; }
    //    public string texto_destino { get; set; }
    //    public string fecha { get; set; }
    //    public string conductor { get; set; }
    //    public string estatus_aventon { get; set; }
    //    public string rol { get; set; }
    //    public string texto_origen { get; set; }
    //    public string pasajero { get; set; }
    //    public double latitud_origen { get; set; }
    //    public double latitud_destino { get; set; }
    //    public double logitud_origen { get; set; }
    //    public double longitud_destino { get; set; }
    //    public string hora_llegada { get; set; }
    //    public int perfilconductor_id { get; set; }
    //    public int perfilpasajero_id { get; set; }
    //}
    public class Aventone
    {
        public string texto_destino { get; set; }
        public int perfil_id { get; set; }
        public string fecha { get; set; }
        public string conductor { get; set; }
        public string fecha_aventon { get; set; }
        public string estatus_aventon { get; set; }
        public string hora_llegada { get; set; }
        public string conductor_corto { get; set; }
        public string pasajero { get; set; }
        public string carrera { get; set; }
        public string nombre { get; set; }
        public int id { get; set; }
        public int aventon_id { get; set; }
        public string destino { get; set; }
        public double latitud_origen { get; set; }
        public int perfilconductor_id { get; set; }
        public double latitud_destino { get; set; }
        public string hora_llegada_formato { get; set; }
        public string rol { get; set; }
        public string pasajero_corto { get; set; }
        public double longitud_destino { get; set; }
        public string texto_origen { get; set; }
        public int perfilpasajero_id { get; set; }
        public string origen { get; set; }
        public double logitud_origen { get; set; }
    }
    public class NextLifts
    {
        public int estatus { get; set; }
        public string mes { get; set; }
        public int persona_id { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
        public List<string> dias { get; set; }
        public List<Aventone> aventones { get; set; }
    }
    public class Ruta
    {
        public string texto_destino { get; set; }
        public long fecha_creacion { get; set; }
        public int perfil_id { get; set; }
        public string persona_nombre { get; set; }
        public string persona_carrera { get; set; }
        public string persona_edad { get; set; }
        public long ultima_conexion { get; set; }
        public long hora_llegada { get; set; }
        //public string puntos { get; set; }
        public double latitud_origen { get; set; }
        public long fecha_fin { get; set; }
        public double longitud_origen { get; set; }
        public int numero_personas { get; set; }
        public double latitud_destino { get; set; }
        public string texto_origen { get; set; }
        public double longitud_destino { get; set; }
        public int ruta_id { get; set; }
        public long fecha_inicio { get; set; }
        public Puntos puntos { get; set; }
    }
    public class RouteRes
    {
        public Ruta ruta { get; set; }
        public int estatus { get; set; }
    }

    public class Puntos
    {
        public Start start { get; set; }
        public End end { get; set; }
        public List<List<double>> waypoints { get; set; }
    }
    public class Rutai
    {
        public string texto_destino { get; set; }
        public object fecha_creacion { get; set; }
        public int perfil_id { get; set; }
        public string persona_carrera { get; set; }
        public string persona_nombre { get; set; }
        public object ultima_conexion { get; set; }
        public object hora_llegada { get; set; }
        public string persona_edad { get; set; }
        public string puntos { get; set; }
        public string fecha_inicio_formato { get; set; }
        public double latitud_origen { get; set; }
        public object fecha_fin { get; set; }
        public double longitud_origen { get; set; }
        public int numero_personas { get; set; }
        public string polilinea_codificada { get; set; }
        public string puntos_intermedios { get; set; }
        public double latitud_destino { get; set; }
        public string hora_llegada_formato { get; set; }
        public double longitud_destino { get; set; }
        public string texto_origen { get; set; }
        public int? tipo_mapa { get; set; }
        public object fecha_inicio { get; set; }
        public int ruta_id { get; set; }
    }

    public class Rutes
    {
        public List<Rutai> rutas { get; set; }
        public int estatus { get; set; }
    }
    /*******************/
    public class Rutas
    {
        public List<Ruta> rutas { get; set; }
    }
    public class Notificacione
    {
        public string nombre { get; set; }
        public int perfil_id { get; set; }
        public int aventon_id { get; set; }
        public string fecha { get; set; }
        public string fecha_aventon { get; set; }
        public object descripcion { get; set; }
        public string estatus_aventon { get; set; }
        public string rol { get; set; }
        public object foto { get; set; }
        public int activo { get; set; }
        public int perfilconductor_id { get; set; }
        public int perfilpasajero_id { get; set; }
        public int id { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }
        public string tipo { get; set; }

    }

    public class Notifications
    {
        public List<Notificacione> notificaciones { get; set; }
    }


    public class LiftsToRate
    {
        public List<Aventone> aventones { get; set; }
        public int estatus { get; set; }
    }
    public class PerfilDado
    {
        public string nombre { get; set; }
        public string imagen { get; set; }
        public int perfil_id { get; set; }
        public string descripcion { get; set; }
    }

    public class PerfilRecibido
    {
        public string nombre { get; set; }
        public string imagen { get; set; }
        public int perfil_id { get; set; }
        public string descripcion { get; set; }
    }

    public class MiRed
    {
        public int estatus { get; set; }
        public List<PerfilDado> perfil_dados { get; set; }
        public List<PerfilRecibido> perfil_recibidos { get; set; }
    }

    public class Evaluacione
    {
        public string nombre { get; set; }
        public int perfil_id { get; set; }
        public int eval_id { get; set; }
        public string comentario { get; set; }
        public int calificacion { get; set; }
    }

    public class Evaluaciones
    {
        public List<Evaluacione> evaluaciones { get; set; }
    }
    //****/

    //****/

}
