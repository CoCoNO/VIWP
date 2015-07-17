using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;

namespace VoyIteso.Pages
{
    public partial class MapPage : PhoneApplicationPage
    {
        //constructor of class
        public MapPage()
        {
            InitializeComponent();
            _aconfirmed = false;
            ResetValues();
            //Touch.FrameReported += Touch_FrameReported;  
            _pushPinUsuario.Source = new BitmapImage(new Uri("u.png", UriKind.Relative));
            this.Loaded += new RoutedEventHandler(SearchView_Loaded);
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "acc0d8e8-cffc-4bcb-9d28-06444a2fc7d8";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "0FvJj6wXx2HVKh7g-6hRGw"; 
        }


        readonly Image _pushPinUsuario = new Image();
        readonly ProgressIndicator _progress = new ProgressIndicator();
        private MapLayer _layer;
        private MapLayer _waylayer;

        private bool _confirmed;
        private bool _aconfirmed;
        private bool _routeConfirmed;
        private int _pointCount;
        /// <summary>
        /// bPoint has an ubicacion which is a GeoCoordinate object (bpoint.GeoCoordinate), and a content which is the mapIcon object.
        /// </summary>
        private MapOverlay _bPoint;
        private MapLayer _aPoint;
        private List<MapOverlay> wayPointList = new List<MapOverlay>();
        private List<MapLayer> waylayerList = new List<MapLayer>();
        //private List<MapLayer> waylayers = new List<MapLayer>();
        public static bool Driver = false;
        public static bool Passenger = false;

         
        private void ResetValues()
        {
            _routeConfirmed = false;
            _confirmed = false;
            //_aconfirmed = false;
            _bPoint = null;
            _pointCount = 0;
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }


        private async void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            FijarIteso();
            Salute();
            //FijarPosicionActual(); 
            //el progreso debe parar ya que ya se tiene una ubicacion para el usuario.
            ConfirmedChanged += myConfirmedChanged;
        }
        /// <summary>
        /// es disparado cuando hay punto a y b fijados. (origen y destino)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void myConfirmedChanged(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("agrega algunos puntos intermedios para facilitar el cálculo de tu ruta", "una última cosa", MessageBoxButton.OKCancel);
            flag = true;
        }

        private bool flag = false;
        private bool _paramsConfirmed ;
        public bool ParamsConfirmed {
            get { return _paramsConfirmed; }
            set
            {
                if (value && !_paramsConfirmed)
                {
                    onConfirmedChanged(EventArgs.Empty);
                }
                _paramsConfirmed = value;
            }
        }

        public EventHandler ConfirmedChanged;

        protected virtual void onConfirmedChanged(EventArgs e)
        {
            if (ConfirmedChanged!=null)
            {
                ConfirmedChanged(this, e);
            }
        }

        /// <summary>
        /// Este metodo va a introducir al usuario, puede que en un futurio dispare un tutorial.
        /// </summary>
        private void Salute()
        {
            MessageBox.Show("presiona y manten en el lugar que quieres poner tu destino", "¿a dónde vas?", MessageBoxButton.OK);
        }

        /// <summary>
        /// Gets the position of the user geolocation, draws the user, and centers the map on the user.
        /// </summary>
        private async void FijarIteso()
        { 
            GeoCoordinate migeoCoordenada = new GeoCoordinate(20.608390, -103.414512); 
            dibujaru(migeoCoordenada);
            MyMapControl.Center = migeoCoordenada;
            MyMapControl.ZoomLevel = 13;
            ApplicationBar.IsVisible = true;
            _aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            MessageBox.Show("el origen está fijado en el ITESO, pero puedes fijar tu posición en las configuraciones");
        }

        /// <summary>
        /// Gets the position of the user, draws the user, and centers the map on the user.
        /// </summary>
        public async void FijarPosicionActual() // Metodo que obtiene la posicion actual del usuario 
        {
            Geolocator migeolocalizador = new Geolocator();
            Geoposition migeoposicion = await migeolocalizador.GetGeopositionAsync();
            Geocoordinate migeocoordenada = migeoposicion.Coordinate;
            GeoCoordinate migeoCoordenada = convertidirGeocoordinate(migeocoordenada);
            dibujaru(migeoCoordenada);
            MyMapControl.Center = migeoCoordenada;
            MyMapControl.ZoomLevel = 13;
            ApplicationBar.IsVisible = true;
            _aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            MessageBox.Show("origen está fijado en tu posición");
        }

        /// <summary>
        /// Gets a GeoCoordinate.
        /// </summary>
        /// <param name="geocoordenada"></param>
        /// <returns>Geocoordinate. (note Geocoordinate with minus "c". They are different classes.)</returns>
        public static GeoCoordinate convertidirGeocoordinate(Geocoordinate geocoordenada)//Metodo que convierte de Geocoordinate a GeoCoordinate
        {
            return new GeoCoordinate
                (
                geocoordenada.Latitude,
                geocoordenada.Longitude,
                geocoordenada.Altitude ?? Double.NaN,
                geocoordenada.Accuracy,
                geocoordenada.AltitudeAccuracy ?? Double.NaN,
                geocoordenada.Speed ?? Double.NaN,
                geocoordenada.Heading ?? Double.NaN
                );
        }

        /// <summary>
        /// Loads the list of pushpins or recomended places. this method is not implemented yet
        /// </summary>
        public void cargarlista()  // En este metodo es donde se carga la lista de puntos, en este ejemplo esta fija, pero podrian leerlos de un archivo XML ... 
        {
            //StringBuilder output = new StringBuilder();
            //XElement booksFromFile = XElement.Load(@"Resources/data-set.xml");
            //String xmlString = booksFromFile.ToString();



            //// Create an XmlReader
            //using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            //{
            //    int i = 0;
            //    while (i < 402)
            //    {
            //        ImagenPersonalizada imp = new ImagenPersonalizada();
            //        reader.ReadToFollowing("name");
            //        reader.MoveToFirstAttribute();
            //        output.AppendLine(reader.ReadElementContentAsString());
            //        imp.nombre = output.ToString();
            //        imp.nombre = imp.nombre.Substring(0, imp.nombre.Length - 2);
            //        output.Clear();

            //        reader.ReadToFollowing("operador");
            //        reader.MoveToFirstAttribute();
            //        output.AppendLine(reader.ReadElementContentAsString());
            //        imp.operador = output.ToString();
            //        imp.operador = imp.operador.Substring(0, imp.operador.Length - 2);
            //        output.Clear();

            //        reader.ReadToFollowing("address");
            //        reader.MoveToFirstAttribute();
            //        output.AppendLine(reader.ReadElementContentAsString());
            //        imp.address = output.ToString();
            //        imp.address = imp.address.Substring(0, imp.address.Length - 2);
            //        output.Clear();

            //        reader.ReadToFollowing("x");
            //        reader.MoveToFirstAttribute();
            //        output.AppendLine(reader.ReadElementContentAsString());
            //        imp.latitud = output.ToString();
            //        imp.latitud = imp.latitud.Substring(0, imp.latitud.Length-2);
            //        output.Clear();

            //        reader.ReadToFollowing("y");
            //        reader.MoveToFirstAttribute();
            //        output.AppendLine(reader.ReadElementContentAsString());
            //        imp.longitud = output.ToString();
            //        imp.longitud = imp.longitud.Substring(0, imp.longitud.Length-2);
            //        output.Clear();

            //        todoslosobjetos.Add(imp);
            //        ++i;
            //    }

            }

        /// <summary>
        /// Draws the user icon on the map.
        /// </summary>
        /// <param name="ubicacion"> Is the location where the pushpin is going to be placed. </param>
        public void dibujaru(GeoCoordinate ubicacion) // Metodo que dibuja la posicion actual del usuario en el mapa
        {

            var imgusuarioenelmapa = new MapOverlay
            {
                GeoCoordinate = ubicacion,
                Content = _pushPinUsuario
            };
            //var imgusuarioenelmapa = new MapOverlay();
            //imgusuarioenelmapa.GeoCoordinate = ubicacion;
            //imgusuarioenelmapa.Content = _pushPinUsuario;

            var capausuario = new MapLayer {imgusuarioenelmapa};
            _aPoint = capausuario;
            MyMapControl.Layers.Add(capausuario);
            //var capausuario = new MapLayer();
            //capausuario.Add(imgusuarioenelmapa);
            //MyMapControl.Layers.Add(capausuario);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        //{
        //    TouchPoint touchPoint = e.GetTouchPoints(this.ContentPanel).FirstOrDefault();
        //    if (touchPoint.Action == TouchAction.Up)
        //        MessageBox.Show(touchPoint.Position.X + "," + touchPoint.Position.Y);//Displaying x&y co-ordinates of Touch point  
        //}

        /// <summary>
        /// This event is not being implemented.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MyMapControl_OnTap(object sender, GestureEventArgs e)
        {
            ////if the destination has not been added yet and it has no confirmation, then a new bpoint is going to be added to the map.
            //if (!_confirmed)
            //{
            //    if (_pointCount < 1)
            //    {
            //        AddBPoint(sender, e);
            //    }
            //    else
            //    {
            //        MyMapControl_OnCenterChanged(null, null);
            //        MyMapControl_OnTap(sender, e);
            //    }
            //}
        }

        

        

        /// <summary>
        /// Adds the destination point to the map. and does take the value of the local mapOverlay and takes it to the main context, it also gets the temp layer and takes it to the main context, changes the appbarmode to default.
        /// the mapOverlay object contains the ubicacion and the map icon
        /// </summary>
        private void AddBPoint(object sender, GestureEventArgs e)
        {
            //_bPointAdded = true;
            //Find geocoordinate on tapped location.
            var asd = this.MyMapControl.ConvertViewportPointToGeoCoordinate(e.GetPosition(this.MyMapControl));
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var bpoint = new MapOverlay();
            var ubicacion = new GeoCoordinate(asd.Latitude, asd.Longitude);
            bpoint.GeoCoordinate = ubicacion;
            bpoint.Content = mapIcon;
            newLayer.Add(bpoint);
            //add pushpin layer to map layers.
            MyMapControl.Layers.Add(newLayer);
            _pointCount++;

            //get properties of the address from the tapped location. 
            var query = new ReverseGeocodeQuery { GeoCoordinate = new GeoCoordinate(asd.Latitude, asd.Longitude) };
            query.QueryCompleted += (s, ev) =>
            {
                if (ev.Error == null && ev.Result.Count > 0)
                {
                    var locations = ev.Result as List<MapLocation>;
                    // do what you want with the locations...
                    foreach (var newLocation in locations)
                    {
                        //feed.Text += newLocation.Information.Address.Street.ToString();
                        mapIcon.pushPinHeader.Text =

                            //newLocation.Information.Address.Street.ToString();
                            //newLocation.Information.Description.ToString();
                            //newLocation.Information.Address.BuildingName.ToString();
                            newLocation.Information.Address.Street.ToString() + " " + newLocation.Information.Address.HouseNumber + "\n" +
                            newLocation.Information.Address.District.ToString();
                    }

                }
            };
            query.QueryAsync();
            this._bPoint = bpoint;//take the value of the temp map overlay and take it to the main context.
            _layer = newLayer;//takes the value of newLayer so we can find the newLayer in the list and delete it if necesary.
            //change the mode of the applicationBar so the user notices there's an action to be performed (confirm the pushpin location)
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }


        private void MyMapControl_OnCenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            if (_pointCount > 0  &&  !_confirmed)
            {
                MyMapControl.Layers.Remove(_layer);
                _pointCount--;
            }
            
        }
        //clic de confirmacion tanto para un punto b agregado
        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            if (flag)
            {
                if (wayPointList.Count>0)
                {
                    var result = MessageBox.Show("fijar los puntos intermedios a tu ruta, si cancelas tendrás que volver a agregar todos los puntos", "confirmar operación", MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {//issue... acomodar los parametros aqui para el apiconector. fin
                        MessageBox.Show("ruta establecida");
                        _routeConfirmed = true;
                    }
                    else
                    { // el usuario le pico a cancel entonces se va a borrar la lista de waypoints y eliminar todas las capas q contienen pushpins. excepto el punto a y b
                        wayPointList.Clear();
                        foreach (var layer in waylayerList)
                        {
                            MyMapControl.Layers.Remove(layer);  
                        }
                        waylayerList.Clear();
                    }

                }
                else
                {
                    var res2 = MessageBox.Show("no agregaste puntos intermedios, los puntos intermedios ayudan a definir mejor la ruta ¿quieres avanzar sin hacerlo?", "confirmar operación", MessageBoxButton.OKCancel);
                    if (res2 == MessageBoxResult.OK)
                    {
                        MessageBox.Show("ruta establecida");
                        _routeConfirmed = true;
                    }
                }
            }
            else
            {
                _confirmed = true;
                var mapIcon = _bPoint.Content;
                MessageBox.Show("destino fijado");//falta ponerle en donde 
                //
                //MyMapControl.Layers.Add(layer);                    
            }
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        private void ChangeDestinationButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                var ubicacion = _bPoint.GeoCoordinate;
                MessageBoxResult result = MessageBox.Show("El destino se va a eliminar, debes fijar uno nuevo",
                    "atención",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    //var ubicacion = _bPoint.GeoCoordinate;
                    MyMapControl.Center = ubicacion;
                    MyMapControl.ZoomLevel = 13;
                    ResetValues();
                    MyMapControl.Layers.Remove(_layer);
                    MessageBox.Show("destino borrado");
                    ParamsConfirmed = false;
                }
                
                
            }
            catch (Exception)
            {
                MessageBox.Show("Aún no hay destino fijado, debes agregar uno",
                    "atención",
                    MessageBoxButton.OK);
            }
            
        }

        /// <summary>
        /// borra el origin y pone la geolocalizacion actual del celular. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindMeButton_OnClick(object sender, EventArgs e)
        { 
            try
            {
                MyMapControl.Layers.Remove(_aPoint);
                FijarPosicionActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchNowButton_OnClick(object sender, EventArgs e)
        {
            if (_aconfirmed && _confirmed)
            {
                ParamsConfirmed = true;
            }
            else
            {
                ParamsConfirmed = false;
                flag = false;
            }
        }

        /// <summary>
        /// This event adds a new layer with a pushpin on the holded location with district and street and number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMapControl_OnHold(object sender, GestureEventArgs e)
        {
            if (flag && !_routeConfirmed)
            {
                addWayPoint(sender, e);
            }

            //if the destination has not been added yet and it has no confirmation, then a new bpoint is going to be added to the map.
            else if (!_confirmed)
            {
                if (_pointCount < 1)
                {
                    AddBPoint(sender, e);
                }
                else
                {
                    MyMapControl_OnCenterChanged(null, null);
                    MyMapControl_OnHold(sender, e);
                }
            }
        }

        private void addWayPoint(object sender, GestureEventArgs e)
        {
            //Find geocoordinate on tapped location.
            var asd = this.MyMapControl.ConvertViewportPointToGeoCoordinate(e.GetPosition(this.MyMapControl));
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var newpoint = new MapOverlay();
            var ubicacion = new GeoCoordinate(asd.Latitude, asd.Longitude);
            newpoint.GeoCoordinate = ubicacion;
            newpoint.Content = mapIcon;
            newLayer.Add(newpoint);
            //add pushpin layer to map layers.
            MyMapControl.Layers.Add(newLayer);
            //_pointCount++;

            //get properties of the address from the tapped location. 
            var query = new ReverseGeocodeQuery { GeoCoordinate = new GeoCoordinate(asd.Latitude, asd.Longitude) };
            query.QueryCompleted += (s, ev) =>
            {
                if (ev.Error == null && ev.Result.Count > 0)
                {
                    var locations = ev.Result as List<MapLocation>;
                    // do what you want with the locations...
                    foreach (var newLocation in locations)
                    {
                        //feed.Text += newLocation.Information.Address.Street.ToString();
                        mapIcon.pushPinHeader.Text =

                            //newLocation.Information.Address.Street.ToString();
                            //newLocation.Information.Description.ToString();
                            //newLocation.Information.Address.BuildingName.ToString();
                            newLocation.Information.Address.Street.ToString() + " " + newLocation.Information.Address.HouseNumber + "\n" +
                            newLocation.Information.Address.District.ToString();
                    }

                }
            };
            query.QueryAsync();
            wayPointList.Add(newpoint);//add new point to waypoint list.
            waylayerList.Add(newLayer);//takes the value of newLayer so we can find the newLayer in the list and delete it if necesary.
            //change the mode of the applicationBar so the user notices there's an action to be performed (confirm the pushpin location)
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }


        

        private void AddBPoint(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //_bPointAdded = true;
            //Find geocoordinate on tapped location.
            var asd = this.MyMapControl.ConvertViewportPointToGeoCoordinate(e.GetPosition(this.MyMapControl));
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var bpoint = new MapOverlay();
            var ubicacion = new GeoCoordinate(asd.Latitude, asd.Longitude);
            bpoint.GeoCoordinate = ubicacion;
            bpoint.Content = mapIcon;
            newLayer.Add(bpoint);
            //add pushpin layer to map layers.
            MyMapControl.Layers.Add(newLayer);
            _pointCount++;

            //get properties of the address from the tapped location. 
            var query = new ReverseGeocodeQuery { GeoCoordinate = new GeoCoordinate(asd.Latitude, asd.Longitude) };
            query.QueryCompleted += (s, ev) =>
            {
                if (ev.Error == null && ev.Result.Count > 0)
                {
                    var locations = ev.Result as List<MapLocation>;
                    // do what you want with the locations...
                    foreach (var newLocation in locations)
                    {
                        //feed.Text += newLocation.Information.Address.Street.ToString();
                        mapIcon.pushPinHeader.Text =

                            //newLocation.Information.Address.Street.ToString();
                            //newLocation.Information.Description.ToString();
                            //newLocation.Information.Address.BuildingName.ToString();
                            newLocation.Information.Address.Street.ToString() + " " + newLocation.Information.Address.HouseNumber + "\n" +
                            newLocation.Information.Address.District.ToString();
                    }

                }
            };
            query.QueryAsync();
            this._bPoint = bpoint;//take the value of the temp map overlay and take it to the main context.
            _layer = newLayer;//takes the value of newLayer so we can find the newLayer in the list and delete it if necesary.
            //change the mode of the applicationBar so the user notices there's an action to be performed (confirm the pushpin location)
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }



    }
}