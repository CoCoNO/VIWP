using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using VoyIteso.Class;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VoyIteso.Pages.MapStuff;
using VoyIteso.Resources;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using System.Windows.Navigation;
using Telerik.Windows.Controls;


namespace VoyIteso.Pages
{
    public partial class TheNewMap : PhoneApplicationPage
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
        public static MapOverlay BPoint { private set; get; }
        public static MapOverlay APoint { get; private set; }
        private MapLayer _aPoint;
        private List<MapOverlay> wayPointList = new List<MapOverlay>();
        private List<MapLayer> waylayerList = new List<MapLayer>();
        //private List<MapLayer> waylayers = new List<MapLayer>();
        public static bool Driver;
        public static bool ReadOnly = false;

        //nuevas cosas.
        public static List<GeoCoordinate> _readonlypoints = new List<GeoCoordinate>();
        //nuevos campos.

        #endregion


        public TheNewMap()
        {
            _searchin = false;
            InitializeComponent();

            if (Driver)
            {
                GenderPickerGrid.Children.Remove(pikerGender);
                SmokerPickerGrid.Children.Remove(pikerSmoke);
            }

            _pushPinUsuario.Source = new BitmapImage(new Uri("/Images/u.png", UriKind.Relative));

            Loaded += SearchView_Loaded;
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "acc0d8e8-cffc-4bcb-9d28-06444a2fc7d8";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "0FvJj6wXx2HVKh7g-6hRGw";


            if (!ReadOnly)
            {
                _aconfirmed = false;
                ResetValues();
                //Touch.FrameReported += Touch_FrameReported;  
                
                states = appBarStates.Init;
                BuildLocalizedApplicationBar();

                if (!positionAquired)
                {
                    //FijarPosicionActual();
                    FijarIteso();
                }

            }
            else
            {
                loadReadOnlyMapPins();

                states = appBarStates.Readonly;
                BuildLocalizedApplicationBar();
            }

            #region cannibaled constructor

            canChangeState = true;
            isSearchTerm = false;
            isOrigin = true;
            isConfirmRoute = false;

            //user.getInfo(user.key); 
            //LocationTimer.Interval = TimeSpan.FromSeconds(1);
            //LocationTimer.Tick += LocationTimer_Tick;

            datePicker.ValueChanged += datePicker_ValueChanged;
            timePicker.ValueChanged += timePicker_ValueChanged;

            progress = new Progress();

            maping = new Maping(myMap, InvisibleCanvas);
            //apiConnector = ApiConnector.instance; 
            //BuildLocalizedApplicationBar();

            setLayout();

            ShowLeftPanelAnimation.Completed += ShowLeftPanelAnimation_Completed;
            ShowLeftPanelFromRightAnimation.Completed += ShowLeftPanelFromRightAnimation_Completed;
            ShowRightPanelAnimation.Completed += ShowRightPanelAnimation_Completed;
            ShowRightPanelFromLeftAnimation.Completed += ShowRightPanelFromLeftAnimation_Completed;
            HideLeftPanelAnimation.Completed += HideLeftPanelAnimation_Completed;
            HideRightPanelAnimation.Completed += HideRightPanelAnimation_Completed;

            //apiConnector.exceptionChanged += apiConnector_exceptionChanged;
            //apiConnector.responseChanged += apiConnector_responseChanged;

            #endregion

        }



        #region methods
        private void loadReadOnlyMapPins()
        {
            dubujarpuntocualquira(_readonlypoints[0],"Origen");
            for (int i = 1; i <= _readonlypoints.Count-2; i++)
            {
                //AddBPoint(_readonlypoints[i]);
                //AddROPoint(_readonlypoints[i], i.ToString());
                dubujarpuntocualquira(_readonlypoints[i], "Punto "+i.ToString());
            }
            dubujarpuntocualquira(_readonlypoints[_readonlypoints.Count-1],"Destino");
            //AddROPoint(_readonlypoints[_readonlypoints.Count - 1], "destino");
            //AddBPoint(_readonlypoints[_readonlypoints.Count-1]);
            myMap.Center = _readonlypoints[_readonlypoints.Count-1];
            myMap.ZoomLevel = 12;
        }

        private void dubujarpuntocualquira(GeoCoordinate geoCoordinate, string p)
        {
            
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            mapIcon.pushPinHeader.Text = p;
            var ubicacion = geoCoordinate;
            var bpoint = new MapOverlay()
            {
                GeoCoordinate = ubicacion,
                Content = mapIcon
            };
            newLayer.Add(bpoint);
            //add pushpin layer to map layers.
            myMap.Layers.Add(newLayer);

            
            
        }

        public static List<MyObject> MyObjects = new List<MyObject>(); 
        private void AddROPoint(GeoCoordinate geoCoordinate, string p)
        {

            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var bpoint = new MapOverlay();
            var ubicacion = geoCoordinate;
            bpoint.GeoCoordinate = ubicacion;
            bpoint.Content = mapIcon;

            mapIcon.pushPinHeader.Text = p;

            newLayer.Add(bpoint);

            //add pushpin layer to map layers.
            myMap.Layers.Add(newLayer);

            
        }


        private async void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            //este se utiliza para que cuando se regrese el usuario no le salga la ventana de seleccionar entre aventon o dar ride, el usuario va a regresar directamente al menu principal.
            NavigationService.RemoveBackEntry();
            ConfirmedChanged += MyConfirmedChanged;

            Microsoft.Phone.Shell.SystemTray.ForegroundColor = System.Windows.Media.Color.FromArgb(255, 208, 236, 255);
            Microsoft.Phone.Shell.SystemTray.BackgroundColor = Color.FromArgb(255, 23, 23, 23);
            RouteResult.ResultHeight = (int)(Application.Current.Host.Content.ActualHeight / 4);
            RouteResult.ResultWidth = (int)((Application.Current.Host.Content.ActualWidth / 4) * 3);

        }

        //private MapLayer ALayer;
        private async void FijarIteso()
        {
            var migeoCoordenada = new GeoCoordinate(20.608390, -103.414512);//iteso
            dibujaru(migeoCoordenada);
            myMap.Center = migeoCoordenada;
            myMap.ZoomLevel = 13;
            //ApplicationBar.IsVisible = true;
            _aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            //MessageBox.Show("por defecto, el origen está fijado en el ITESO, pero puedes modificarlo.");
            ReverseQuery();
        }

        private async void FijarOtroLado(GeoCoordinate coordinate)
        {
            var migeoCoordenada = coordinate;//el otro lado.
            dibujaru(migeoCoordenada);
            myMap.Center = migeoCoordenada;
            myMap.ZoomLevel = 13;
            //ApplicationBar.IsVisible = true;
            _aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            //MessageBox.Show("por defecto, el origen está fijado en el ITESO, pero puedes modificarlo.");
            ReverseQuery();
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
            APoint = imgusuarioenelmapa;
            //maping.locationA = APoint;

            var capausuario = new MapLayer { imgusuarioenelmapa };
            myMap.Layers.Remove(_aPoint);
            _aPoint = capausuario;
            myMap.Layers.Add(capausuario);
            //var capausuario = new MapLayer();
            //capausuario.Add(imgusuarioenelmapa);
            //MyMapControl.Layers.Add(capausuario);
        }

        /// <summary>
        /// este metodo pedorro regresa los valores a los predeterminados.
        /// </summary>
        private void ResetValues()
        {
            _routeConfirmed = false;
            _confirmed = false;
            _paramsConfirmed = false;
            //_aconfirmed = false;
            _flag = false;
            BPoint = null;
            _pointCount = 0;
            //ApplicationBar.Mode = ApplicationBarMode.Minimized;//there is no app bar build yet.
            positionAquired = false;
            //dateString = DateTime.Today.ToString();
            dateString = string.Format("{0:dd-MM-yyyy}", DateTime.Today);
            //timeString = DateTime.Now.ToString();
            timeString = string.Format("{0:HH:mm}", DateTime.Now);
        }

        /// <summary>
        /// es disparado cuando hay punto a y b fijados. (origen y destino). play>>>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void MyConfirmedChanged(object sender, EventArgs eventArgs)
        {
            if (Driver)
            {
                MessageBox.Show("Para facilitar el cálculo de la ruta agrega algunos puntos intermedios y luego presiona en confirmar. Puedes omitir este paso presionando confirmar.", "Agrega puntos intermedios", MessageBoxButton.OKCancel);
                states = appBarStates.Waypoint;
                BuildLocalizedApplicationBar();
            }

            _flag = true;
        }

        private bool _flag = false;
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

        private static List<GeoCoordinate> Readonlypoints
        {
            get { return _readonlypoints; }
            set { _readonlypoints = value; }
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
        /// Gets a Geocoordinate and returns a GeoCoordinate.
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
        private void AddWayPoint(object sender, GestureEventArgs e)
        {//issue... tener una lista de waypoints a la cual agregaremos 
            //Find geocoordinate on tapped location.
            var asd = this.myMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(this.myMap));
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var newpoint = new MapOverlay();
            var ubicacion = new GeoCoordinate(asd.Latitude, asd.Longitude);
            newpoint.GeoCoordinate = ubicacion;
            newpoint.Content = mapIcon;
            newLayer.Add(newpoint);
            //add pushpin layer to map layers.
            myMap.Layers.Add(newLayer);
            //_pointCount++;

            //get properties of the address from the tapped location. we are repeating ourselves, there is a need to refactor code later.
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

            states = appBarStates.Waypoint;
            BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Adds the destination point to the map. and does take the value of the local mapOverlay and takes it to the main context, it also gets the temp layer and takes it to the main context, changes the appbarmode to default.
        /// the mapOverlay object contains the ubicacion and the map icon
        /// </summary>
        public void AddBPoint(object sender, GestureEventArgs e)
        {
            //_bPointAdded = true;
            //Find geocoordinate on tapped location.
            var asd = this.myMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(this.myMap));
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var bpoint = new MapOverlay();
            var ubicacion = new GeoCoordinate(asd.Latitude, asd.Longitude);
            bpoint.GeoCoordinate = ubicacion;
            bpoint.Content = mapIcon;
            newLayer.Add(bpoint);
            //add pushpin layer to map layers.
            myMap.Layers.Add(newLayer);
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
                        mapIcon.StreetName = newLocation.Information.Address.Street.ToString();
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
            BPoint = bpoint;//take the value of the temp map overlay and take it to the main context.
            _layer = newLayer;//takes the value of newLayer so we can find the newLayer in the list and delete it if necesary.
            //change the mode of the applicationBar so the user notices there's an action to be performed (confirm the pushpin location)
            if (ReadOnly)
            return;
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }

        public void AddBPoint(GeoCoordinate geoCoordinate)
        {
            //_bPointAdded = true;
            //Find geocoordinate on tapped location.
            //var asd = this.myMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(this.myMap));
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var bpoint = new MapOverlay();
            var ubicacion = geoCoordinate;
            bpoint.GeoCoordinate = ubicacion;
            bpoint.Content = mapIcon;
            newLayer.Add(bpoint);
            //add pushpin layer to map layers.
            myMap.Layers.Add(newLayer);
            _pointCount++;

            //get properties of the address from the tapped location. 
            var query = new ReverseGeocodeQuery { GeoCoordinate = new GeoCoordinate(geoCoordinate.Latitude, geoCoordinate.Longitude) };
            query.QueryCompleted += (s, ev) =>
            {
                if (ev.Error == null && ev.Result.Count > 0)
                {
                    var locations = ev.Result as List<MapLocation>;
                    // do what you want with the locations...
                    foreach (var newLocation in locations)
                    {
                        mapIcon.StreetName = newLocation.Information.Address.Street.ToString();
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
            BPoint = bpoint;//take the value of the temp map overlay and take it to the main context.
            _layer = newLayer;//takes the value of newLayer so we can find the newLayer in the list and delete it if necesary.
            //change the mode of the applicationBar so the user notices there's an action to be performed (confirm the pushpin location)

            if (ReadOnly) return;
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }


        DispatcherTimer t;
        private async void FijarPosicionActual()
        {
            
            Geolocator migeolocalizador = new Geolocator();

            if (migeolocalizador.LocationStatus == PositionStatus.Disabled)
            {
                var res = MessageBox.Show("Tu ubicación actual nos ayuda a proporcionarte mejores servicios de búsqueda y ubicación.\n\nTu informacion no se usará para identificarte ni ponerse en contacto contigo es solo uso y funcionamiento para VoyIteso.", "¿Permitir que VoyIteso acceda a tu ubicación?", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    //ir a prender el localizador.
                    var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                    return;
                }
                else
                {
                    return;
                }
            }
            progress.showProgressIndicator(this, "calculando tu ubicación...");
            Geoposition migeoposicion = await migeolocalizador.GetGeopositionAsync();
            progress.hideProgressIndicator(this);
            positionAquired = true;
            Geocoordinate migeocoordenada = migeoposicion.Coordinate;
            GeoCoordinate migeoCoordenada = convertidirGeocoordinate(migeocoordenada);
            dibujaru(migeoCoordenada);
            myMap.Center = migeoCoordenada;
            myMap.ZoomLevel = 13;
            ApplicationBar.IsVisible = true;
            _aconfirmed = true;

            ReverseQuery();//get back here

            
            MessageBox.Show("Origen fijado en tu posición");
        }

        private void t_Tick(object sender, EventArgs e)
        {

            try
            {
                t.Stop();
                txtOriginRojo.Text = aStreetName;
            }
            catch (Exception)
            {

                t = new DispatcherTimer();

                t.Tick += t_Tick;
                t.Interval = new TimeSpan(00, 0, 1);
                t.Start();
            }

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
                MessageBox.Show("Ruta establecida");
            }
            _routeConfirmed = true;
            //NavigationService.Navigate(new Uri("/Pages/SecondaryMapPage.xaml", UriKind.Relative));

            //en lugar de navegar a la pag secundaria que habia hecho vamos a abrir el panel izquierdo del menu hamburguesa.
            appBarSearchButton_Click(null, null);
        }

        /// <summary>
        /// este es el boton de confirmar, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play(object sender, EventArgs e)// 
        {
            //SearchNowButton_OnClick(null, null);
            //la flag es la que indica que ya existen tanto punto A (origen) como punto B (destino). cambiar estas babosadas
            if (_flag)
            {

                if (Driver)
                {
                    if (wayPointList.Count > 0)// si hay puntos intermedios
                    {
                        var result = MessageBox.Show("Vas fijar los puntos intermedios a tu ruta, si cancelas tendrás que volver a agregarlos", "Confirmar operación", MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.OK)
                        {//issue... acomodar los parametros aqui para el apiconector. fin
                            //navegar aqui para pedir los datos que faltan como la hora, la fecha y la chingada.
                            NavigateToSecundaryPage();// cambiarle nombre, ya no hace lo q su nombre dice.. esta pendejo ese nombre.. o sacar las cosas de la funcion
                        }
                        else
                        { // el usuario le pico a cancel entonces se va a borrar la lista de waypoints y eliminar todas las capas q contienen pushpins. excepto el punto a y b
                            wayPointList.Clear();
                            //se sugiere myMap.Layers.Clear();
                            foreach (var layer in waylayerList)
                            {
                                myMap.Layers.Remove(layer);

                            }
                            waylayerList.Clear();
                        }

                    }
                    else// si no hay puntos intermedios
                    {
                        var res2 = MessageBox.Show("No agregaste puntos intermedios, los puntos intermedios ayudan a definir mejor la ruta ¿quieres avanzar sin hacerlo?", "Confirmar operación", MessageBoxButton.OKCancel);
                        if (res2 == MessageBoxResult.OK)
                        {
                            //navegar aqui para pedir los datos que faltan como la hora, la fecha y la chingada.
                            NavigateToSecundaryPage();
                        }
                    }
                }





                else
                {
                    ShowLeftPanelAnimation.Begin();
                    //NavigateToSecundaryPage();
                }

            }
            //aqui es cuando el usuario le pica a confirmar el destino.
            else
            {
                _confirmed = true;
                var mapIcon = (MapIcon)BPoint.Content;//destino
                //var mapIcon2 = (MapIcon) APoint.Content;//origen
                MessageBox.Show("Destino fijado");
                if (aStreetName == null)
                {
                    txtOriginRojo.Text = "escribe una dirección";
                    txtDestinyRojo.Text = "escribe una dirección";
                }
                else
                {
                    txtOriginRojo.Text = aStreetName;
                    if (mapIcon.StreetName == null)
                    {
                        txtDestinyRojo.Text = "No encontramos la dirección, escríbela";
                    }
                    else
                        txtDestinyRojo.Text = mapIcon.StreetName;
                }


                SearchNowButton_OnClick(null, null);
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
                if (Driver) return;
                ShowLeftPanelAnimation.Begin();

            }
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }


        private void ReverseQuery()
        {
            var asd = APoint.GeoCoordinate;
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
                        aStreetName = newLocation.Information.Address.Street.ToString();
                        //feed.Text += newLocation.Information.Address.Street.ToString();
                        //mapIcon.pushPinHeader.Text =

                        //    //newLocation.Information.Address.Street.ToString();
                        //    //newLocation.Information.Description.ToString();
                        //    //newLocation.Information.Address.BuildingName.ToString();
                        //    newLocation.Information.Address.Street.ToString() + " " + newLocation.Information.Address.HouseNumber + "\n" +
                        //    newLocation.Information.Address.District.ToString();
                    }

                }
            };
            query.QueryAsync();

            t = new DispatcherTimer();

            t.Tick += t_Tick;
            t.Interval = new TimeSpan(00, 0, 1);
            t.Start();

        }




        private void ChangeDestinationButton_OnClick(object sender, EventArgs e)
        {

            ResetValues();
            if (Driver)
            {
                startOverWayPoints_OnClick(null, null);
            }
            var a = states;
            MyMapControl_OnCenterChanged(null, null);
            myMap.Layers.Remove(_layer);
            MessageBox.Show("Coloca un nuevo destino para poder continuar", "Destino eliminado", MessageBoxButton.OK);
            ResetValues();
            if (a == appBarStates.Left)
            {
                HideLeftPanelAnimation.Begin();
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
                _flag = false;
            }
        }

        private bool _searchin;
        private void MyMapControl_OnCenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            //if (Driver)
            //{//wayPointList.Count>0
            //    //cuando comienza a poner puntos intermedios va a forzar al usuario a enfocarse a solo hacer eso. 
            //    states = (_pointCount > 0) ? ((wayPointList.Count>0) ? appBarStates.Waypoint : appBarStates.Shit2) : appBarStates.Init; 

            //    BuildLocalizedApplicationBar();
            //    ApplicationBar.Mode = ApplicationBarMode.Minimized;

            //}

            if (ReadOnly)return;
            
            if (!_searchin)
            {
                if (_pointCount > 0 && !_confirmed)
                {
                    myMap.Layers.Remove(_layer);
                    _pointCount--;
                    states = appBarStates.Map;
                    BuildLocalizedApplicationBar();
                }
            }



        }

        /// <summary>
        /// esta cosa la hice provisional para mostarr mensaje de espera cuando el usuario agrega un punto b o waypoint. para el tiempo en 
        /// que se tarda en haer el reverse query. 
        /// </summary>
        /// <returns></returns>
        public Task ExpensiveTaskAsync()
        {
            return Task.Run(() => ExpensiveTask());
        }

        private void ExpensiveTask()
        {
            // MessageBox.Show("expensive task completed...");
            //for (int i = 0; i < 10000000; i++)
            //{
            //    i = i;
            //}

        }
        async public void DoStuff()
        {
            //PrepareExpensiveTask();
            progress.showProgressIndicator(this, "esperando al servidor");
            await ExpensiveTaskAsync();
            progress.hideProgressIndicator(this);
            //UseResultsOfExpensiveTask();
        }
        /// <summary>
        /// This event adds a new layer with a puonshpin on the holded location with district and street and number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMapControl_OnHold(object sender, GestureEventArgs e)
        {
            //DoStuff();
            if (_flag && !_routeConfirmed)
            {
                AddWayPoint(sender, e);
            }

            //if the destination has not been added yet and it has no confirmation, then a new bpoint is going to be added to the map.
            else if (!_confirmed)
            {
                if (_pointCount < 1)
                {
                    AddBPoint(sender, e);
                    states = appBarStates.Shit;
                    BuildLocalizedApplicationBar();
                    //change the mode of the applicationBar so the user notices there's an action to be performed (confirm the pushpin location)
                    //ApplicationBar.Mode = ApplicationBarMode.Default;
                }
                else
                {
                    MyMapControl_OnCenterChanged(null, null);
                    MyMapControl_OnHold(sender, e);
                }
            }
        }

        #endregion



        #region Canibalizado


        #region cannibaled class fields
        bool canChangeState;
        bool isSearchTerm;
        bool isOrigin;
        bool isConfirmRoute;
        //DispatcherTimer LocationTimer = new DispatcherTimer();
        String timeString;
        public static String dateString;
        Progress progress;
        int intSmoker;
        int intGender;
        RouteResult currentRoute;

        enum appBarStates { Map, Left, Right, Search, Confirm, Shit, Waypoint, Init, Shit2, Readonly };
        appBarStates states;

        //User user = new User();

        BingConnectorcs bingConnector = new BingConnectorcs();

        List<string> autoComplemapingreets;
        Maping maping;
        private bool positionAquired;
        private string aStreetName;
        //ApiConnector apiConnector;
        #endregion

        #region Time & Date Change event
        void timePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            timeString = string.Format("{0:HH:mm}", e.NewDateTime);
        }

        void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            TimeSpan time = ((DateTime)e.NewDateTime).Subtract(DateTime.Now);
            if (time.Days >= 0)
                dateString = string.Format("{0:dd-MM-yyyy}", e.NewDateTime);
            else
                MessageBox.Show("No puedes agendar aventones para el pasado, ¿te crees Marty McFly?", "No te pases de listo", MessageBoxButton.OK);
        }
        #endregion

        #region OnBackKeyPress
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

            if (states == appBarStates.Search)
            {
                SearchTermGrid.Visibility = System.Windows.Visibility.Collapsed;
                ShowLeftPanelAnimation.Begin();
                isSearchTerm = false;
                e.Cancel = true;
            }
            else if (states == appBarStates.Confirm)
            {
                isConfirmRoute = false;
                if (maping.myMapRoute != null)
                    myMap.RemoveRoute(maping.myMapRoute);
                maping.DrawMapLocation();
                ResultsListBox.SelectedIndex = -1;
                ShowRightPanelAnimation.Begin();
                e.Cancel = true;
            }
            else if (states == appBarStates.Readonly)
            {
                //NavigationService.Navigate(new Uri("/Pages/ShowRoutes.xaml", UriKind.Relative));
                //NavigationService.RemoveBackEntry();
                if (TheNewMap.Driver)
                {
                    NavigationService.Navigate(new Uri("/Pages/ShowRoutes.xaml", UriKind.Relative));
                    NavigationService.RemoveBackEntry();
                }
                else
                    NavigationService.GoBack();
            }
            else

            {
                var result = MessageBox.Show("Vas a regresar al menú sin haber realizado cambios","Salir del mapa",MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                    NavigationService.RemoveBackEntry();
                    //base.OnBackKeyPress(e);
                }
                else
                {
                    e.Cancel = true;
                }
                
            }

            
        }


        #endregion

        #region myReverseGeocodeQuery_QueryCompleted_2
        private void myReverseGeocodeQuery_QueryCompleted_2(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    MapAddress address = e.Result[0].Information.Address;
                    String direction = "";

                    if (address.Street.Length > 0)
                    {
                        direction += address.Street;
                    }
                    else
                        direction += "Mi ubicacion";

                    if (address.HouseNumber.Length > 0)
                    {
                        direction += " # " + address.HouseNumber + " ";
                    }

                    if (isOrigin)
                        txtOriginRojo.Text = direction;
                    else
                        txtDestinyRojo.Text = direction;
                }
            }
            else
            {
                MessageBox.Show("No se encontraron resultados", "Error", MessageBoxButton.OK);
            }

            if (states == appBarStates.Search)
            {
                ShowLeftPanelAnimation.Begin();
            }

            progress.hideProgressIndicator(this);
            maping.myReverseGeocodeQuery.Dispose();
        }
        #endregion

        #region Origin and Destiny Tap
        private void txtOrigin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (canChangeState)
            {
                HideLeftPanelAnimation.Begin();
                canChangeState = false;
                isOrigin = true;
                isSearchTerm = true;
            }
        }

        private void txtDestiny_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (canChangeState)
            {
                HideLeftPanelAnimation.Begin();
                canChangeState = false;
                isOrigin = false;
                isSearchTerm = true;
            }
        }
        #endregion


        public GeocodeQuery myGeocodeQuery;
        private void searchTermBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (searchTermBox.Text.Length > 0)
                {
                    //if (APoint!=null)
                    //{
                    //    //maping.locationA = APoint;
                    //    //maping.myCoordinate = APoint.GeoCoordinate;
                    //    //maping.mapLayer = myMap.Layers[0];
                    //}
                    //maping.myCoordinates.Clear();
                    progress.showProgressIndicator(this, "Buscando");
                    //maping.searchForTerm(searchTermBox.Text, this, this);


                    myGeocodeQuery = new GeocodeQuery();
                    myGeocodeQuery.SearchTerm = searchTermBox.Text;
                    myGeocodeQuery.GeoCoordinate = APoint.GeoCoordinate ?? new GeoCoordinate(0, 0);
                    myGeocodeQuery.QueryCompleted += myGeocodeQuery_QueryCompleted;
                    myGeocodeQuery.QueryAsync();



                    myMap.ZoomLevelChanged += myMap_ZoomLevelChanged;
                    if (isOrigin)
                    {
                        txtOriginRojo.Text = searchTermBox.Text;
                    }
                    else
                    {
                        txtDestinyRojo.Text = searchTermBox.Text;
                    }
                    searchTermBox.Text = "";
                    //progress.hideProgressIndicator(this);
                    //ShowLeftPanelAnimation.Begin();
                    //SearchTermGrid.Visibility = System.Windows.Visibility.Collapsed;
                    isSearchTerm = false;
                }
            }
        }

        List<GeoCoordinate> myCoordinates = new List<GeoCoordinate>();
        GeoCoordinate pointGeoCoordinate = new GeoCoordinate();
        void myGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            try
            {

                if (e.Error == null)
                {
                    if (e.Result.Count > 0)
                    {
                        myCoordinates.Clear();
                        myCoordinates.Add(e.Result[0].GeoCoordinate);
                        pointGeoCoordinate = e.Result[0].GeoCoordinate;
                        this.myMap.SetView(myCoordinates[0], 14.0, MapAnimationKind.Parabolic);
                        //TheNewMap.fo//aqui me quede
                        new Progress().hideProgressIndicator(this);
                        SearchTermGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.Focus();

                    }
                    else
                    {
                        MessageBox.Show("No se encontro ningun resultado", "Error", MessageBoxButton.OK);
                        new Progress().hideProgressIndicator(this);
                    }

                    myGeocodeQuery.Dispose();
                }

                if (!isOrigin)
                {
                    DrawMapMarkers();

                    states = appBarStates.Shit;
                    BuildLocalizedApplicationBar();
                }
                else
                {
                    FijarOtroLado(myCoordinates[0]);
                    this.myMap.SetView(myCoordinates[0], 14.0, MapAnimationKind.Parabolic);

                    states = appBarStates.Map;
                    BuildLocalizedApplicationBar();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrio un error, vuelve a intentarlo", "Error", MessageBoxButton.OK);
            }
        }

        DispatcherTimer timer = new DispatcherTimer();
        public void DrawMapMarkers()
        {
            //MapOverlay userLocationOverlay;
            //userLocationOverlay = this.mapLayer.ElementAt(0);
            //userLocationOverlay = this._aPoint.ElementAt(0);
            //this._aPoint.Clear();
            //this._aPoint.Add(userLocationOverlay);
            for (int i = 0; i < myCoordinates.Count; i++)
            {
                //DrawMapMarker(myCoordinates[i], Colors.Blue);
                _searchin = true;
                AddBPoint(myCoordinates[i]);

            }

            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(00, 0, 3);
            //bool enabled = timer.IsEnabled;
            timer.Start();

        }

        void timer_Tick(object sender, object e)
        {
            //function to execute
            timer.Stop();
            _searchin = false;
        }

        void myMap_ZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            if (maping.mapLayer.Count <= 1 || ApplicationBar.IsVisible) return;
            BuildLocalizedApplicationBar();
            myMap.Focus();
            progress.hideProgressIndicator(this);
        }

        private void mapLayer_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (maping.mapLayer.Count > 1)
            {
                BuildLocalizedApplicationBar();
                myMap.Focus();
            }
        }

        #region MapPanel Tap & MapBackFromLeft & MapBackFromRight
        private void MapPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (states == appBarStates.Left && canChangeState)
            {
                canChangeState = false;
                HideLeftPanelAnimation.Begin();

            }
            else if (states == appBarStates.Right && canChangeState)
            {
                HideRightPanelAnimation.Begin();
            }
        }
        #endregion

        #region Animations Completed
        void ShowLeftPanelAnimation_Completed(object sender, EventArgs e)
        {
            states = appBarStates.Left;
            canChangeState = true;
            isSearchTerm = false;
            BuildLocalizedApplicationBar();
            if (_pointCount > 0)
            {
                MessageBox.Show("para continuar presiona en confirmar", "Configura fecha y horario", MessageBoxButton.OK);
            }
        }

        void ShowRightPanelAnimation_Completed(object sender, EventArgs e)
        {
            states = appBarStates.Right;
            canChangeState = true;
            isSearchTerm = false;
            BuildLocalizedApplicationBar();
        }

        void HideLeftPanelAnimation_Completed(object sender, EventArgs e)
        {
            if (isSearchTerm)
            {
                ApplicationBar.IsVisible = false;
                SearchTermGrid.Visibility = System.Windows.Visibility.Visible;
                searchTermBox.Focus();
                states = appBarStates.Search;
            }
            else
            {
                isSearchTerm = false;
                states = appBarStates.Map;
                BuildLocalizedApplicationBar();
            }
            canChangeState = true;
        }

        void HideRightPanelAnimation_Completed(object sender, EventArgs e)
        {
            if (isConfirmRoute)
            {
                states = appBarStates.Confirm;
            }
            else
            {
                states = appBarStates.Map;
            }
            canChangeState = true;
            isSearchTerm = false;
            BuildLocalizedApplicationBar();
        }

        private void ShowLeftPanelFromRightAnimation_Completed(object sender, EventArgs e)
        {
            states = appBarStates.Left;
            canChangeState = true;
            isSearchTerm = false;
            BuildLocalizedApplicationBar();
        }

        private void ShowRightPanelFromLeftAnimation_Completed(object sender, EventArgs e)
        {
            states = appBarStates.Right;
            canChangeState = true;
            isSearchTerm = false;
            BuildLocalizedApplicationBar();
        }
        #endregion

        #region appBar Buttons
        async void appBarSearchRouteButton_Click(object sender, EventArgs e)
        {

            try
            {



                progress.showProgressIndicator(this, "Espera un momento, por favor");
                //SendSearchRequest(); 
                //send request
                //aqui puto jairo
                string origen = txtOriginRojo.Text;
                string destino = txtDestinyRojo.Text;
                string fecha = dateString;// la fecha de inicia obtenida del time picker
                DateTime myDateTime;
                //12.0.0
                //
                myDateTime = DateTime.ParseExact(dateString + " " + timeString, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);//yyyy-MM-dd HH:mm tt   M/d/yyyy hh:mm
                string hora = timeString;

                double lat_destino = BPoint.GeoCoordinate.Latitude;
                double lon_destino = BPoint.GeoCoordinate.Longitude;
                double lat_origen = APoint.GeoCoordinate.Latitude;
                double lon_origen = APoint.GeoCoordinate.Longitude;




                ///DAR AVENTON
                if (TheNewMap.Driver)// si estas creando ruta
                {
                    IEnumerable<GeoCoordinate> myEnumerable;
                    //convierto mis waypoins en un enumerable para pasarlo al metodo de crear ruta.
                    /*if (wayPointList.Count > 0)
                    {
                        List<GeoCoordinate> myList = wayPointList.Select(mapOverlay => mapOverlay.GeoCoordinate).ToList();
                        myEnumerable = myList;
                    }
                    else
                    {
                        var newpoint = new MapOverlay(); 
                        var ubicacion = new GeoCoordinate(20.608390, -103.414512);// voy a agregar iteso si no pone waypoints.
                        newpoint.GeoCoordinate = ubicacion;
                        wayPointList.Add(newpoint);
                        List<GeoCoordinate> myList = wayPointList.Select(mapOverlay => mapOverlay.GeoCoordinate).ToList();
                        myEnumerable = myList;
                    }*/

                    List<GeoCoordinate> myList = new List<GeoCoordinate>(), waypoints = wayPointList.Select(mapOverlay => mapOverlay.GeoCoordinate).ToList();

                    myList.Add(new GeoCoordinate(lat_origen, lon_origen));

                    foreach (var item in waypoints)
                    {
                        myList.Add(item);
                    }

                    myList.Add(new GeoCoordinate(lat_destino, lon_destino));


                    //List<string> listAgain = myEnumerable.ToList();//para convertir atras.
                    //Rutai rutai = new Rutai();
                    var a = await ApiConnector.Instance.RouteCreate(origen, destino, myDateTime, myDateTime.AddHours(1), "1", 2, myList);
                    progress.hideProgressIndicator(this);
                    if (a.estatus == 1)
                    {
                        MessageBox.Show("Ruta agregada exitosamente");
                        NavigationService.Navigate(new Uri("/Pages/ShowRoutes.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show(a.error, "Hubo un error en el sistema", MessageBoxButton.OK);
                    }
                }



                ///BUSCAR AVENTON
                else
                {
                    //buscar ruta
                    var rutas = await ApiConnector.Instance.RouteSearch(origen, destino, fecha, lat_destino, lon_destino, lat_origen, lon_origen, hora);
                    progress.hideProgressIndicator(this);

                    try
                    {
                        ResultsListBox.Items.Clear();
                        if (rutas.rutas.Count > 0)
                        {
                            foreach (var ruta in rutas.rutas)
                            {
                                //pinche trampa sucia! pero funciona y se ve muy bien. repetir en caja de resultados en mapa XD. 
                                var grid = new Grid();
                                grid.Width = 440;
                                grid.Height = 20;
                                ResultsListBox.Items.Add(grid);

                                var resultados = new cajaDeResultados();
                                resultados.NombreDelConductor.Text = ruta.persona_nombre;
                                resultados.DescripcionDeRuta.Text = ruta.texto_origen + "\n" + ruta.texto_destino + "\na las " + ruta.hora_llegada_formato + " el " + ruta.fecha_inicio_formato.Substring(0, 2) + "-" + ruta.fecha_inicio_formato.Substring(2, 2) + "-" + ruta.fecha_inicio_formato.Substring(4);
                                resultados.routeID = ruta.ruta_id;

                                resultados.routeID = ruta.ruta_id;
                                resultados.perfil_id = ruta.perfil_id.ToString();
                                //resultados.aventon_id = ruta.;
                                resultados.texto_origen = ruta.texto_origen;
                                resultados.texto_destino = ruta.texto_destino;

                                resultados.ruta = ruta;

                                //resultados.Text += ruta.persona_nombre + ruta.texto_origen + ruta.texto_destino + ruta.hora_llegada;
                                ResultsListBox.Items.Add(resultados);

                            }
                            ShowRightPanelAnimation.Begin();

                        }
                        else
                        {
                            MessageBox.Show("No hay rutas disponibles, intenta otro horario");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Revisa tus datos e intenta de nuevo. tip: fecha mal", "Información inválida", MessageBoxButton.OK);
                        Debug.WriteLine("algo salio mal>>>" + ex.Message);
                        //throw;
                    }

                }




            }
            catch (Exception ex)
            {
                MessageBox.Show("Revisa tus datos e intenta de nuevo.", "Información inválida", MessageBoxButton.OK);
            }
            new Progress().hideProgressIndicator(this);
            //ResultsListBox.Items.Add(resultados);

        }

        private List<Notificacione> a;

        void appBarShowResults_Click(object sender, EventArgs e)
        {
            if (canChangeState)
            {
                canChangeState = false;
                ShowRightPanelFromLeftAnimation.Begin();
            }
        }
        void appBarShowMapButton_Click(object sender, EventArgs e)
        {
            if (canChangeState)
            {
                canChangeState = false;
                HideLeftPanelAnimation.Begin();
            }
        }
        void appBarShowSearch_Click(object sender, EventArgs e)
        {
            if (canChangeState)
            {
                canChangeState = false;
                ShowLeftPanelFromRightAnimation.Begin();
            }
        }
        void appBarShowMapFromRight_Click(object sender, EventArgs e)
        {
            if (canChangeState)
            {
                canChangeState = false;
                HideRightPanelAnimation.Begin();
            }
        }
        //primer confirmacion, la del punto b colocado correctamente
        void appBarSearchButton_Click(object sender, EventArgs e)
        {
            if (canChangeState)
            {
                canChangeState = false;
                ShowLeftPanelAnimation.Begin();
                //System.Threading.Thread.Sleep(1000);//number is expressed in miliseconds. 
                //MessageBox.Show("para continuar presiona en confirmar","Configura fecha y horario",MessageBoxButton.OK);
            }
        }
        void appBarResultButton_Click(object sender, EventArgs e)
        {
            if (canChangeState)
            {
                canChangeState = false;
                ShowRightPanelAnimation.Begin();
            }
        }
        void appBarSearchLocationButton_Click(object sender, EventArgs e)
        {
            if (isOrigin)
            {
                maping.locationA = maping.mapLayer.ElementAt(maping.mapLayer.Count - 1);
            }
            else
            {
                maping.locationB = maping.mapLayer.ElementAt(maping.mapLayer.Count - 1);
            }

            if (maping.myReverseGeocodeQuery == null || !maping.myReverseGeocodeQuery.IsBusy)
            {
                progress.showProgressIndicator(this, "Procesando");
                maping.myReverseGeocodeQuery = new ReverseGeocodeQuery();
                maping.myReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(maping.pointGeoCoordinate.Latitude, maping.pointGeoCoordinate.Longitude);
                maping.myReverseGeocodeQuery.QueryCompleted += myReverseGeocodeQuery_QueryCompleted_2;
                maping.myReverseGeocodeQuery.QueryAsync();
            }

            SearchTermGrid.Visibility = System.Windows.Visibility.Collapsed;
            searchTermBox.Text = "";
            isSearchTerm = false;



        }

        //PLAY>>>>metodo viejo de jairo. deprecado.
        async void appBarConfirmButton_Click(object sender, EventArgs e)
        {

            //send request 
            string origen = txtOriginRojo.Text;
            string destino = txtDestinyRojo.Text;
            string fecha = dateString;
            double lat_destino = TheNewMap.BPoint.GeoCoordinate.Latitude;
            double lon_destino = TheNewMap.BPoint.GeoCoordinate.Longitude;
            double lat_origen = TheNewMap.APoint.GeoCoordinate.Longitude;
            double lon_origen = TheNewMap.APoint.GeoCoordinate.Longitude;
            string hora = timeString;

            if (TheNewMap.Driver)
            {

            }
            else
            {
                new Progress().showProgressIndicator(this, "espera un momento");
                await ApiConnector.Instance.RouteSearch(origen, destino, fecha, lat_destino, lon_destino, lat_origen, lon_origen, hora);
                new Progress().hideProgressIndicator(this);
            }
        }

        void appBarReturnResultsButton_Click(object sender, EventArgs e)//metodo viejo de emmanuel, no se esta usando ya que jamas entramos al estado confirm. del appbar. 
        {
            isConfirmRoute = false;
            maping.DrawMapLocation();
            if (maping.myMapRoute != null)
                myMap.RemoveRoute(maping.myMapRoute);
            ResultsListBox.SelectedIndex = -1;
            ShowRightPanelAnimation.Begin();
        }
        #endregion

        #region BuildLocalizedApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            if (states == appBarStates.Map)
            {
                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar";
                appBarSearchButton.Click += appBarSearchButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                if (!Driver)
                {
                    ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Images/icons/List-50.png", UriKind.Relative));
                    appBarResultButton.Text = "Resultados";
                    appBarResultButton.Click += appBarResultButton_Click;
                    ApplicationBar.Buttons.Add(appBarResultButton);
                }

                ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                fijarme.Text = "Fijar mi posición como origen";
                fijarme.Click += fijarme_OnClick;
                ApplicationBar.MenuItems.Add(fijarme);

                ApplicationBarMenuItem fijariteso = new ApplicationBarMenuItem();
                fijariteso.Text = "Fijar iteso como origen";
                fijariteso.Click += fijariteso_OnClick;
                ApplicationBar.MenuItems.Add(fijariteso);

                ApplicationBarMenuItem b = new ApplicationBarMenuItem { Text = "¿Cómo se usa?" };
                b.Click += howToUse_Click;
                ApplicationBar.MenuItems.Add(b);

                if (_pointCount <= 0) return;
                ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                changeDestination.Text = "Borrar el destino";
                changeDestination.Click += ChangeDestinationButton_OnClick;
                ApplicationBar.MenuItems.Add(changeDestination);
            }

            else if (states == appBarStates.Shit2)
            {
                //ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/u.png", UriKind.Relative));
                //a.Text = "cómo usar";
                //a.Click += howToUse_Click;
                //ApplicationBar.Buttons.Add(a);

                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchButton);


                if (!Driver)
                {
                    
                    ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                    appBarResultButton.Text = "Resultados";
                    appBarResultButton.Click += appBarResultButton_Click;
                    ApplicationBar.Buttons.Add(appBarResultButton);

                }
                
                ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                fijarme.Text = "Fijar mi posición como origen";
                fijarme.Click += fijarme_OnClick;
                ApplicationBar.MenuItems.Add(fijarme);


                ApplicationBarMenuItem fijariteso = new ApplicationBarMenuItem();
                fijariteso.Text = "Fijar iteso como origen";
                fijariteso.Click += fijariteso_OnClick;
                ApplicationBar.MenuItems.Add(fijariteso);

                ApplicationBarMenuItem a = new ApplicationBarMenuItem { Text = "Ver tips" };
                a.Click += howToUse_Click;
                ApplicationBar.MenuItems.Add(a);


                if (_pointCount <= 0) return;
                ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                changeDestination.Text = "Borrar el destino";
                changeDestination.Click += ChangeDestinationButton_OnClick;
                ApplicationBar.MenuItems.Add(changeDestination);

            }
            else if (states == appBarStates.Readonly)
            {
                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Images/icons/close.png", UriKind.Relative));
                appBarSearchButton.Text = "Cerrar";
                appBarSearchButton.Click += cerrar_click;
                ApplicationBar.Buttons.Add(appBarSearchButton);
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            }

            else if (states == appBarStates.Init)
            {
                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar";
                appBarSearchButton.Click += appBarSearchButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                if (!Driver)
                {
                    ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Images/icons/List-50.png", UriKind.Relative));
                    appBarResultButton.Text = "Resultados";
                    appBarResultButton.Click += appBarResultButton_Click;
                    ApplicationBar.Buttons.Add(appBarResultButton);
                }
                
                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/icons/questionmark.png", UriKind.Relative));
                a.Text = "Ver tips";
                a.Click += howToUse_Click;
                ApplicationBar.Buttons.Add(a);

                ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                fijarme.Text = "Fijar mi posición como origen";
                fijarme.Click += fijarme_OnClick;
                ApplicationBar.MenuItems.Add(fijarme);

                ApplicationBarMenuItem fijariteso = new ApplicationBarMenuItem();
                fijariteso.Text = "Fijar iteso como origen";
                fijariteso.Click += fijariteso_OnClick;
                ApplicationBar.MenuItems.Add(fijariteso);


                ApplicationBarMenuItem b = new ApplicationBarMenuItem { Text = "Ver tips" };
                b.Click += howToUse_Click;
                ApplicationBar.MenuItems.Add(b);


            }

            else if (states == appBarStates.Waypoint)
            {

                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/check.png", UriKind.Relative));
                a.Text = "Confirmar puntos";
                a.Click += Play;
                ApplicationBar.Buttons.Add(a);

                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                appBarSearchButton.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                if (!Driver)
                {
                    ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                    appBarResultButton.Text = "Resultados";
                    appBarResultButton.Click += appBarResultButton_Click;
                    appBarResultButton.IsEnabled = false;
                    ApplicationBar.Buttons.Add(appBarResultButton);
                }

                ApplicationBarMenuItem b = new ApplicationBarMenuItem();
                b.Text = "Volver a empezar";//
                b.Click += startOverWayPoints_OnClick;
                ApplicationBar.MenuItems.Add(b);
            }

                //el de la palomita
            else if (states == appBarStates.Shit)
            {


                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/check.png", UriKind.Relative));
                a.Text = "Confirmar";
                a.Click += Play;
                ApplicationBar.Buttons.Add(a);


                ApplicationBarIconButton b = new ApplicationBarIconButton(new Uri("/Images/icons/cancel.png", UriKind.Relative));
                b.Text = "Cancelar";
                b.Click += Cancel;
                ApplicationBar.Buttons.Add(b);

                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                appBarSearchButton.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                
                if (!Driver)
                {
                    ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                    appBarResultButton.Text = "Resultados";
                    appBarResultButton.Click += appBarResultButton_Click;
                    appBarResultButton.IsEnabled = false;
                    ApplicationBar.Buttons.Add(appBarResultButton);
    
                }


                //ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                //changeDestination.Text = "cambiar el destino";
                //changeDestination.Click += ChangeDestinationButton_OnClick;

                //ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                //appBarSearchButton.Text = "Buscar Ruta";
                //appBarSearchButton.Click += appBarSearchButton_Click;
                //ApplicationBar.Buttons.Add(appBarSearchButton);

                //ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                //appBarResultButton.Text = "Resultados";
                //appBarResultButton.Click += appBarResultButton_Click;
                //ApplicationBar.Buttons.Add(appBarResultButton);

            }
            else if (states == appBarStates.Left)
            {
                ApplicationBarIconButton appBarSearchRouteButton = new ApplicationBarIconButton(new Uri("Assets/check.png", UriKind.Relative));
                appBarSearchRouteButton.Text = "Confirmar";
                appBarSearchRouteButton.Click += appBarSearchRouteButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchRouteButton);

                ApplicationBarIconButton appBarShowMapButton = new ApplicationBarIconButton(new Uri("Images/icons/map.png", UriKind.Relative));
                appBarShowMapButton.Text = "Mapa";
                appBarShowMapButton.Click += appBarShowMapButton_Click;
                ApplicationBar.Buttons.Add(appBarShowMapButton);

                if (!Driver)
                {
                    ApplicationBarIconButton appBarShowResultsButton = new ApplicationBarIconButton(new Uri("Images/icons/List-50.png", UriKind.Relative));
                    appBarShowResultsButton.Text = "Resultados";
                    appBarShowResultsButton.Click += appBarShowResults_Click;
                    ApplicationBar.Buttons.Add(appBarShowResultsButton);
                }

                ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                fijarme.Text = "Fijar mi posición como origen";
                fijarme.Click += fijarme_OnClick;
                ApplicationBar.MenuItems.Add(fijarme);


                ApplicationBarMenuItem fijariteso = new ApplicationBarMenuItem();
                fijariteso.Text = "Fijar iteso como origen";
                fijariteso.Click += fijariteso_OnClick;
                ApplicationBar.MenuItems.Add(fijariteso);


                if (_pointCount <= 0) return;
                ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                changeDestination.Text = "Borrar el destino";
                changeDestination.Click += ChangeDestinationButton_OnClick;
                ApplicationBar.MenuItems.Add(changeDestination);
            }

            else if (states == appBarStates.Right)
            {
                ApplicationBarIconButton appBarShowMapFromRightButton = new ApplicationBarIconButton(new Uri("Images/icons/map.png", UriKind.Relative));
                appBarShowMapFromRightButton.Text = "Mapa";
                appBarShowMapFromRightButton.Click += appBarShowMapFromRight_Click;
                ApplicationBar.Buttons.Add(appBarShowMapFromRightButton);

                ApplicationBarIconButton appBarShowSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarShowSearchButton.Text = "Buscar";
                appBarShowSearchButton.Click += appBarShowSearch_Click;
                ApplicationBar.Buttons.Add(appBarShowSearchButton);
            }

            else if (states == appBarStates.Search)
            {
                ApplicationBarIconButton appBarSearchLocationButton = new ApplicationBarIconButton(new Uri("Assets/save.png", UriKind.Relative));
                appBarSearchLocationButton.Text = "Guardar";
                appBarSearchLocationButton.Click += appBarSearchLocationButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchLocationButton);
            }

            else if (states == appBarStates.Confirm)
            {
                ApplicationBarIconButton appBarConfirmButton = new ApplicationBarIconButton(new Uri("Assets/check.png", UriKind.Relative));
                appBarConfirmButton.Text = "Aceptar";
                appBarConfirmButton.Click += appBarConfirmButton_Click;
                ApplicationBar.Buttons.Add(appBarConfirmButton);

                if (!Driver)
                {
                    ApplicationBarIconButton appBarReturnResultsButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                    appBarReturnResultsButton.Text = "Resultados";
                    appBarReturnResultsButton.Click += appBarReturnResultsButton_Click;
                    appBarReturnResultsButton.IsEnabled = false;
                    ApplicationBar.Buttons.Add(appBarReturnResultsButton);                    
                }

            }

        }

        private void Cancel(object sender, EventArgs e)
        {
            MyMapControl_OnCenterChanged(null,null);
            txtDestinyRojo.Text = "";
        }

        private void cerrar_click(object sender, EventArgs e)
        {
            OnBackKeyPress(null);
        }

        private void fijariteso_OnClick(object sender, EventArgs e)
        {
            FijarIteso();
        }

        private void startOverWayPoints_OnClick(object sender, EventArgs e)
        {
            if (!Driver)
            {
                ChangeDestinationButton_OnClick(null, null);
            }
            else
            {
                wayPointList.Clear();
                foreach (var a in waylayerList)
                {
                    myMap.Layers.Remove(a);
                }
                waylayerList.Clear();

                MessageBox.Show("Enfócate en colocar los puntos intermedios", "Puntos intermedios borrados", MessageBoxButton.OK);
            }

        }

        private void fijarme_OnClick(object sender, EventArgs e)
        {
            FijarPosicionActual();
            //FijarOtroLado();
        }

        private void howToUse_Click(object sender, EventArgs e)
        {
            if (Driver)
            {
                NavigationService.Navigate(new Uri("/Pages/0Tutorials/TutCrearRuta.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Pages/0Tutorials/TutBuscarRuta.xaml", UriKind.Relative));
            }
           
        }


        #endregion

        #region searchTermBox_TextChanged
        private void searchTermBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (searchTermBox.Text.Length > 2)
            {
                try
                {
                    bingConnector.address = searchTermBox.Text;
                    bingConnector.SendGetRequest();
                    bingConnector.responseChanged += bingConnector_responseChanged;
                }
                catch (Exception ex) { }
            }

        }
        #endregion

        #region bingConnector_responseChanged
        void bingConnector_responseChanged(object sender, EventArgs e)
        {

            List<Street> streets = bingConnector.getStreets();
            autoComplemapingreets = new List<string>(streets.Count);
            foreach (Street street in streets)
                autoComplemapingreets.Add(street.name);
            searchTermBox.ItemsSource = autoComplemapingreets;
        }
        #endregion

        #region setLayout
        private void setLayout()
        {
            int phoneWidth = (int)Application.Current.Host.Content.ActualWidth;
            int phoneHeight = (int)Application.Current.Host.Content.ActualHeight;
            int panelLeftWidth = (phoneWidth / 4) * 3;
            int searchGridHeight = phoneHeight / 6;
            int searchImageHeight = phoneHeight / 6;
            int canvasWidth = (phoneWidth * 2) + panelLeftWidth;

            CanvasRootTransform.TranslateX = (double)(panelLeftWidth * -1);

            Binding binding = new Binding();

            binding.Mode = BindingMode.OneTime;
            binding.Source = canvasWidth;
            CanvasRoot.SetBinding(Canvas.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneHeight;
            CanvasRoot.SetBinding(Canvas.HeightProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = canvasWidth;
            RootGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneHeight;
            RootGrid.SetBinding(Grid.HeightProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = searchGridHeight;
            SearchTermGrid.SetBinding(Grid.HeightProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneWidth;
            SearchTermGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = panelLeftWidth;
            LeftPanelGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneWidth;
            CenterPanelGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneWidth;
            RightPanelGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = searchImageHeight;
            SearchImageGrid.SetBinding(Grid.HeightProperty, binding);

            LeftKeyFrameShowBegin.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)(panelLeftWidth * -1));
            LeftKeyFrameHideEnd.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)(panelLeftWidth * -1));
            RightKeyFrameShowBegin.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)(panelLeftWidth * -1));
            RightKeyFrameShowEnd.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)((panelLeftWidth * -1) - phoneWidth));
            RightKeyFrameHideBegin.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)((panelLeftWidth * -1) - phoneWidth));
            RightKeyFrameHideEnd.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)(panelLeftWidth * -1));
            RightKeyFrameShowFromLeftEnd.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)((panelLeftWidth * -1) - phoneWidth));
            LeftKeyFrameShowFromRightBegin.SetValue(EasingDoubleKeyFrame.ValueProperty, (double)((panelLeftWidth * -1) - phoneWidth));
        }
        #endregion

        private void MapGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.myMap.IsEnabled)
            {
                MapOverlay location = this.myMap.Layers[0].ElementAt(this.myMap.Layers[0].Count - 1);
                GeoCoordinate position = myMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(myMap));
                location.GeoCoordinate = position;
            }
        }

        private void MapGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.myMap.IsEnabled)
            {
                Ellipse circle = (Ellipse)this.myMap.Layers[0].ElementAt(this.myMap.Layers[0].Count - 1).Content;
                circle.Fill = new SolidColorBrush(Colors.Blue);
                this.myMap.IsEnabled = true;
            }
        }

        //saendSearchRequest() deleted

        //apiConnector_exceptionChanged(object sender, EventArgs e) eliminado

        private void pikerSmoke_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0].ToString().Equals("Fumador"))
                    intSmoker = 1;
                else if (e.AddedItems[0].ToString().Equals("No Fumador"))
                    intSmoker = 0;
                else
                    intSmoker = -1;
            }
        }

        private void pikerGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0].ToString().Equals("Hombre"))
                    intGender = 1;
                else if (e.AddedItems[0].ToString().Equals("Mujer"))
                    intGender = 0;
                else
                    intGender = -1;
            }
        }

        private Class.Notifications _listOfNotifications;
        public static Notificacione NotificationItem;
        public static cajaDeResultados caja;
        private async void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {//solilcitar aventon, solicitud de aventon, listbox de resultados.

            var index = ResultsListBox.SelectedIndex - 1;

            if (index % 2 != 0 || index < 0)
            {
                return;
            }

            var item = (cajaDeResultados)ResultsListBox.SelectedItem;
            item.myBool = true;
            //caja = item;
            //var item = _listOfNotifications.notificaciones[index / 2];
            //NotificationItem = item;
            //item.routeID

            //new RouteInfo().foo(item);
            caja = item;
            NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/RouteInfo.xaml", UriKind.Relative));//?key=value&key2=value


            //this.NavigationService.Navigate();
            //new Progress().showProgressIndicator(this, "esperate poquito brother, ya vamos");
            //_listOfNotifications = await ApiConnector.Instance.NotificationsGet();
            //new Progress().hideProgressIndicator(this);

            //foreach (var noti in _listOfNotifications.notificaciones)
            //{
            //    if (noti.aventon_id == item.routeID)
            //    {
            //        NotificationItem = noti;
            //        NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/RouteInfo.xaml", UriKind.Relative));//?key=value&key2=value
            //        return;
            //    }
            //}

            //NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/RouteInfo.xaml", UriKind.Relative));//?key=value&key2=value

        }

        //SendLiftRequest(int id) deleted



        #endregion

        //private BingConnectorcs bingConnector = new BingConnectorcs();
        private void TxtOriginRojo_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchTermBox.Text.Length > 2)
            {
                try
                {
                    bingConnector.address = searchTermBox.Text;
                    bingConnector.SendGetRequest();
                    bingConnector.responseChanged += bingConnector_responseChanged;
                }
                catch (Exception ex) { }
            }
        }


    }
}