﻿using System;
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
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages
{
    public partial class TheMap : PhoneApplicationPage
    {
#region fields

        /// <summary>
        /// Fields
        /// </summary>
        readonly Image _pushPinUsuario = new Image();
        readonly ProgressIndicator _progress = new ProgressIndicator();
        private MapLayer _layer;
        //private MapLayer _waylayer;

        private bool _confirmed;
        private bool _aconfirmed;
        private bool _routeConfirmed;// esta pendejada sirve para que hacer que no se pueda mover una vez q estan fijados los puntos a y b.
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

#endregion


#region constructor of class

        /// <summary>
        /// constructor of class
        /// </summary>
        public TheMap()
        {
            InitializeComponent();
            _aconfirmed = false;
            ResetValues();
            //Touch.FrameReported += Touch_FrameReported;  
            _pushPinUsuario.Source = new BitmapImage(new Uri("/Images/u.png", UriKind.Relative));
            
            this.Loaded += new RoutedEventHandler(SearchView_Loaded);
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "acc0d8e8-cffc-4bcb-9d28-06444a2fc7d8";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "0FvJj6wXx2HVKh7g-6hRGw";
        }

#endregion


#region methods

        private async void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            FijarIteso();
            Salute();
            NavigationService.RemoveBackEntry();
            //FijarPosicionActual(); 
            //el progreso debe parar ya que ya se tiene una ubicacion para el usuario.
            ConfirmedChanged += myConfirmedChanged;
        }

        private void Salute()
        {
            MessageBox.Show("presiona y manten en el lugar que quieras poner el destino", "¿a dónde vas?", MessageBoxButton.OK);
        }

        private async void FijarIteso()
        {
            GeoCoordinate migeoCoordenada = new GeoCoordinate(20.608390, -103.414512);
            dibujaru(migeoCoordenada);
            MyMapControl.Center = migeoCoordenada;
            MyMapControl.ZoomLevel = 13;
            ApplicationBar.IsVisible = true;
            _aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            MessageBox.Show("el origen está fijado en el ITESO, pero puedes fijar tu posición");
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

            var capausuario = new MapLayer { imgusuarioenelmapa };
            _aPoint = capausuario;
            MyMapControl.Layers.Add(capausuario);
            //var capausuario = new MapLayer();
            //capausuario.Add(imgusuarioenelmapa);
            //MyMapControl.Layers.Add(capausuario);
        }

        private void ResetValues()
        {
            _routeConfirmed = false;
            _confirmed = false;
            //_aconfirmed = false;
            _bPoint = null;
            _pointCount = 0;
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        /// <summary>
        /// es disparado cuando hay punto a y b fijados. (origen y destino)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void myConfirmedChanged(object sender, EventArgs eventArgs)
        {
            if (Driver)
            {
                MessageBox.Show("agrega algunos puntos intermedios para facilitar el cálculo de tu ruta", "ya casi..", MessageBoxButton.OKCancel);
            }
            
            flag = true;
        }

        private bool flag = false;
        private bool _paramsConfirmed;
        public bool ParamsConfirmed
        {
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
            if (ConfirmedChanged != null)
            {
                ConfirmedChanged(this, e);
            }
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

        // <summary>
        /// este evento se disparara cuando se encuentren fijados el origen y el destino.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addWayPoint(object sender, GestureEventArgs e)
        {//issue... tener una lista de waypoints a la cual agregaremos 
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


        private async void FijarPosicionActual()
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



#endregion



#region button controls

        /// <summary>
        /// this shit is a temportal fix to the stupid side panel of the telerik dogshit!!!
        /// </summary>
        private void NavigateToSecundaryPage()
        {
            if (Driver)
            {
                MessageBox.Show("ruta establecida");
            }
            _routeConfirmed = true;
            NavigationService.Navigate(new Uri("/Pages/SecondaryMapPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            SearchNowButton_OnClick(null, null);
            //la flag es la que indica que ya existen tanto punto A (origen) como punto B (destino).
            if (flag)
            {
                if (Driver)
                {
                    if (wayPointList.Count > 0)
                    {
                        var result = MessageBox.Show("fijar los puntos intermedios a tu ruta, si cancelas tendrás que volver a agregar todos los puntos", "confirmar operación", MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.OK)
                        {//issue... acomodar los parametros aqui para el apiconector. fin
                            //navegar aqui para pedir los datos que faltan como la hora, la fecha y la chingada.
                            NavigateToSecundaryPage();
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
                            //navegar aqui para pedir los datos que faltan como la hora, la fecha y la chingada.
                            NavigateToSecundaryPage();
                        }
                    }
                }
                else
                {
                    NavigateToSecundaryPage();
                }
                
            }
            //aqui es cuando el usuario le pica a confirmar el destino.
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
            
            if (flag)
            {
                if (wayPointList.Count > 0)
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

        private void MyMapControl_OnCenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            if (_pointCount > 0 && !_confirmed)
            {
                MyMapControl.Layers.Remove(_layer);
                _pointCount--;
            }

        }

        /// <summary>
        /// This event adds a new layer with a puonshpin on the holded location with district and street and number.
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

        #endregion



        
    }
}