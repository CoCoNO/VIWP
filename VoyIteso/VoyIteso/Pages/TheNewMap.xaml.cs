using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
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
using VoyIteso.Resources;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;


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
        public static bool Driver = false;
        public static bool Passenger = false;



        #endregion

        public TheNewMap()
        {
            InitializeComponent();

            _aconfirmed = false;
            ResetValues();
            //Touch.FrameReported += Touch_FrameReported;  
            _pushPinUsuario.Source = new BitmapImage(new Uri("/Images/u.png", UriKind.Relative));

            Loaded += SearchView_Loaded;
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "acc0d8e8-cffc-4bcb-9d28-06444a2fc7d8";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "0FvJj6wXx2HVKh7g-6hRGw";
        
#region cannibaled constructor
            states = appBarStates.Map;

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

            //maping = new Maping(myMap, InvisibleCanvas);

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

        private async void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            FijarIteso();
            MessageBox.Show("presiona y manten en el lugar en el que quieres colocar el destino", "¿a dónde vas?", MessageBoxButton.OK);
            //este se utiliza para que cuando se regrese el usuario no le salga la ventana de seleccionar entre aventon o dar ride, el usuario va a regresar directamente al menu principal.
            NavigationService.RemoveBackEntry();
            //FijarPosicionActual(); 
            //el progreso debe parar ya que ya se tiene una ubicacion para el usuario.
            ConfirmedChanged += MyConfirmedChanged;

            //
            Microsoft.Phone.Shell.SystemTray.ForegroundColor = System.Windows.Media.Color.FromArgb(255, 0, 66, 112);
            RouteResult.ResultHeight = (int)(Application.Current.Host.Content.ActualHeight / 4);
            RouteResult.ResultWidth = (int)((Application.Current.Host.Content.ActualWidth / 4) * 3);

            BuildLocalizedApplicationBar();
            //myMap.Focus();
            progress.hideProgressIndicator(this);
        }

        private async void FijarIteso()
        {
            GeoCoordinate migeoCoordenada = new GeoCoordinate(20.608390, -103.414512);
            dibujaru(migeoCoordenada);
            myMap.Center = migeoCoordenada;
            myMap.ZoomLevel = 13;
            //ApplicationBar.IsVisible = true;
            _aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            MessageBox.Show("por defecto, el origen está fijado en el ITESO, pero puedes fijar tu posición");
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


            var capausuario = new MapLayer { imgusuarioenelmapa };
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
            //_aconfirmed = false;
            BPoint = null;
            _pointCount = 0;
            //ApplicationBar.Mode = ApplicationBarMode.Minimized;//there is no app bar build yet.
        }

        /// <summary>
        /// es disparado cuando hay punto a y b fijados. (origen y destino)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void MyConfirmedChanged(object sender, EventArgs eventArgs)
        {
            if (Driver)
            {
                MessageBox.Show("para facilitar el cálculo de la ruta agrega algunos puntos intermedios y luego presiona en confirmar. Puedes omitir este paso presionando confirmar.", "agrega puntos intermedios", MessageBoxButton.OKCancel);
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
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }

        private async void FijarPosicionActual()
        {
            Geolocator migeolocalizador = new Geolocator();
            Geoposition migeoposicion = await migeolocalizador.GetGeopositionAsync();
            Geocoordinate migeocoordenada = migeoposicion.Coordinate;
            GeoCoordinate migeoCoordenada = convertidirGeocoordinate(migeocoordenada);
            dibujaru(migeoCoordenada);
            myMap.Center = migeoCoordenada;
            myMap.ZoomLevel = 13;
            ApplicationBar.IsVisible = true;
            _aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            MessageBox.Show("origen está fijado en tu posición");
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
        String dateString;
        Progress progress;
        int intSmoker;
        int intGender;
        RouteResult currentRoute;

        enum appBarStates { Map, Left, Right, Search, Confirm };
        appBarStates states;

        //User user = new User();

        BingConnectorcs bingConnector = new BingConnectorcs();

        List<string> autoComplemapingreets;
        Maping maping;
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
                MessageBox.Show("Ingresar un dia que sea apartir de hoy en adelante", "Error", MessageBoxButton.OK);
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
            else
                base.OnBackKeyPress(e);

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
                        txtOrigin.Text = direction;
                    else
                        txtDestiny.Text = direction;
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
         
        private void searchTermBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (searchTermBox.Text.Length > 0)
                {
                    maping.myCoordinates.Clear();
                    progress.showProgressIndicator(this, "Buscando");
                    //maping.searchForTerm(searchTermBox.Text, this);
                    //myMap.ZoomLevelChanged += myMap_ZoomLevelChanged;

                }
            }
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
        void appBarSearchRouteButton_Click(object sender, EventArgs e)
        {
            progress.showProgressIndicator(this, "Calculando");
            //SendSearchRequest(); 
        }

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

        void appBarSearchButton_Click(object sender, EventArgs e)
        {
            if (canChangeState)
            {
                canChangeState = false;
                ShowLeftPanelAnimation.Begin();
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

        void appBarConfirmButton_Click(object sender, EventArgs e)
        {
            //SendLiftRequest(currentRoute.route_id);   commented shit

        }

        void appBarReturnResultsButton_Click(object sender, EventArgs e)
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
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarResultButton.Text = "Resultados";
                appBarResultButton.Click += appBarResultButton_Click;
                ApplicationBar.Buttons.Add(appBarResultButton);

            }
            else if (states == appBarStates.Left)
            {
                ApplicationBarIconButton appBarSearchRouteButton = new ApplicationBarIconButton(new Uri("Assets/check.png", UriKind.Relative));
                appBarSearchRouteButton.Text = "Buscar";
                appBarSearchRouteButton.Click += appBarSearchRouteButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchRouteButton);

                ApplicationBarIconButton appBarShowMapButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarShowMapButton.Text = "Mostrar Mapa";
                appBarShowMapButton.Click += appBarShowMapButton_Click;
                ApplicationBar.Buttons.Add(appBarShowMapButton);

                ApplicationBarIconButton appBarShowResultsButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarShowResultsButton.Text = "Mostrar resultados";
                appBarShowResultsButton.Click += appBarShowResults_Click;
                ApplicationBar.Buttons.Add(appBarShowResultsButton);
            }

            else if (states == appBarStates.Right)
            {
                ApplicationBarIconButton appBarShowMapFromRightButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarShowMapFromRightButton.Text = "Mostrar Mapa";
                appBarShowMapFromRightButton.Click += appBarShowMapFromRight_Click;
                ApplicationBar.Buttons.Add(appBarShowMapFromRightButton);

                ApplicationBarIconButton appBarShowSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarShowSearchButton.Text = "Realizar Busqueda";
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

                ApplicationBarIconButton appBarReturnResultsButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarReturnResultsButton.Text = "Mostrar resultados";
                appBarReturnResultsButton.Click += appBarReturnResultsButton_Click;
                ApplicationBar.Buttons.Add(appBarReturnResultsButton);
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

        private void ResultsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //progress.showProgressIndicator(this, "Calculando");
            currentRoute = (RouteResult)ResultsListBox.SelectedItem;
            if (currentRoute != null)
            {
                List<GeoCoordinate> ABpoints = new List<GeoCoordinate>();
                ABpoints.Add(currentRoute.origin_coordinate);
                ABpoints.Add(currentRoute.destiny_coordinate);
                maping.myCoordinates = ABpoints;
                maping.DrawMapMarkers();
                maping.DrawRoute(ABpoints);
                isConfirmRoute = true;
                HideRightPanelAnimation.Begin();
            }
            //delete routes from the beginin
            //create a new state on the serch called confirm route with just one button on appbar
            //do testing

        }

        //SendLiftRequest(int id) deleted
        
        

#endregion


    }
}