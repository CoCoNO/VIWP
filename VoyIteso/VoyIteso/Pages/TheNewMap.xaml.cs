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


        #endregion

        //public TheNewMap()
        //{
        //}

        public void foo()
        {
        }


        public TheNewMap()
        {
            InitializeComponent();

            if (Driver)
            {
                GenderPickerGrid.Children.Remove(pikerGender);
                SmokerPickerGrid.Children.Remove(pikerSmoke);
            }

            _aconfirmed = false;
            ResetValues();
            //Touch.FrameReported += Touch_FrameReported;  
            _pushPinUsuario.Source = new BitmapImage(new Uri("/Images/u.png", UriKind.Relative));

            Loaded += SearchView_Loaded;
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "acc0d8e8-cffc-4bcb-9d28-06444a2fc7d8";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "0FvJj6wXx2HVKh7g-6hRGw";

            //lo q estaba en el metodo view_loaded
            states = appBarStates.Init;
            BuildLocalizedApplicationBar();

            //this is somehow stupid
            if (!positionAquired)
            {
                //FijarPosicionActual();
                FijarIteso();
            }
            ////////////////////

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

        private void Salute()
        {
            MessageBox.Show("coloca el destino presionando y manteniendo", "¿a dónde vas?", MessageBoxButton.OK);
        }
        
        private async void SearchView_Loaded(object sender, RoutedEventArgs e)//loaded method
        {
            ////this is somehow stupid
            //if (!positionAquired)
            //{
            //    //FijarPosicionActual();
            //    FijarIteso();
            //    Salute();
            //}

            
            //este se utiliza para que cuando se regrese el usuario no le salga la ventana de seleccionar entre aventon o dar ride, el usuario va a regresar directamente al menu principal.
            NavigationService.RemoveBackEntry();  
            ConfirmedChanged += MyConfirmedChanged;

            Microsoft.Phone.Shell.SystemTray.ForegroundColor = System.Windows.Media.Color.FromArgb(255, 0, 66, 112);
            //Microsoft.Phone.Shell.SystemTray.ForegroundColor = Colors.LightGray;
            Microsoft.Phone.Shell.SystemTray.BackgroundColor = Colors.White;
            RouteResult.ResultHeight = (int)(Application.Current.Host.Content.ActualHeight / 4);
            RouteResult.ResultWidth = (int)((Application.Current.Host.Content.ActualWidth / 4) * 3);

            //states = appBarStates.Init;
            //BuildLocalizedApplicationBar();
            //myMap.Focus();
            //progress.hideProgressIndicator(this);
            //ResultsListBox.Items.Add(new cajaDeResultados());
        }

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
            _paramsConfirmed=false;
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
            
            states = appBarStates.Waypoint;
            BuildLocalizedApplicationBar();
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
            ApplicationBar.Mode = ApplicationBarMode.Default;
        }

        private async void FijarPosicionActual()
        {
            progress.showProgressIndicator(this, "calculando tu ubicación...");
            Geolocator migeolocalizador = new Geolocator();
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
            //NavigationService.Navigate(new Uri("/Pages/SecondaryMapPage.xaml", UriKind.Relative));

            //en lugar de navegar a la pag secundaria que habia hecho vamos a abrir el panel izquierdo del menu hamburguesa.
            appBarSearchButton_Click(null,null);
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
                    if (wayPointList.Count > 0)
                    {
                        var result = MessageBox.Show("vas fijar los puntos intermedios a tu ruta, si cancelas tendrás que volver a agregar los puntos intermedios", "confirmar operación", MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.OK)
                        {//issue... acomodar los parametros aqui para el apiconector. fin
                            //navegar aqui para pedir los datos que faltan como la hora, la fecha y la chingada.
                            NavigateToSecundaryPage();
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
                    ShowLeftPanelAnimation.Begin();
                    //NavigateToSecundaryPage();
                }

            }
            //aqui es cuando el usuario le pica a confirmar el destino.
            else
            {
                _confirmed = true;
                var mapIcon = (MapIcon) BPoint.Content;//destino
                //var mapIcon2 = (MapIcon) APoint.Content;//origen
                MessageBox.Show("destino fijado");
                if (aStreetName==null)
                {
                    txtOriginRojo.Text = "escribe una dirección";
                    txtDestinyRojo.Text = "escribe una dirección";
                }
                else
                {
                    txtOriginRojo.Text = aStreetName;
                    if (mapIcon.StreetName==null)
                    {
                        txtDestinyRojo.Text = "no encontramos la dirección, escríbela";
                    }else
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
        }




        private void ChangeDestinationButton_OnClick(object sender, EventArgs e)
        {
            
            ResetValues();
            if (Driver)
            {
                startOverWayPoints_OnClick(null,null);
            }
            var a = states;
            MyMapControl_OnCenterChanged(null, null);
            myMap.Layers.Remove(_layer);
            MessageBox.Show("Coloca un nuevo destino para continuar", "destino borrado", MessageBoxButton.OK);
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

        private void MyMapControl_OnCenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            if (Driver)
            {//wayPointList.Count>0
                states = (_pointCount > 0) ? ((wayPointList.Count>0) ? appBarStates.Waypoint : appBarStates.Shit2) : appBarStates.Init; 

                BuildLocalizedApplicationBar();
                ApplicationBar.Mode = ApplicationBarMode.Minimized;

            }
            
            if (_pointCount > 0 && !_confirmed)
            {
                myMap.Layers.Remove(_layer);
                _pointCount--;
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
            progress.showProgressIndicator(this,"esperando");
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

        enum appBarStates { Map, Left, Right, Search, Confirm, Shit, Waypoint, Init, Shit2 };
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
            else
                NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                NavigationService.RemoveBackEntry();
            //base.OnBackKeyPress(e);

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
         
        private void searchTermBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (searchTermBox.Text.Length > 0)
                {
                    //maping.myCoordinates.Clear();
                    //progress.showProgressIndicator(this, "Buscando");
                    //maping.searchForTerm(searchTermBox.Text, this);
                    //myMap.ZoomLevelChanged += myMap_ZoomLevelChanged;
                    if (isOrigin)
                    {
                       txtOriginRojo.Text = searchTermBox.Text; 
                    }
                    else
                    {
                        txtDestinyRojo.Text = searchTermBox.Text;
                    }
                    searchTermBox.Text = "";
                    ShowLeftPanelAnimation.Begin();
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
            if (_pointCount>0)
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
                if (a.estatus==1)
                {
                    MessageBox.Show("ruta agregada exitosamente");
                    NavigationService.Navigate(new Uri("/Pages/ShowRoutes.xaml",UriKind.Relative));
                }
                else
                {
                    MessageBox.Show(a.error,"Hubo un error en el sistema",MessageBoxButton.OK);
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
                            resultados.DescripcionDeRuta.Text = ruta.texto_origen + "\n" + ruta.texto_destino + "\na las " + ruta.hora_llegada_formato + " el " + ruta.fecha_inicio_formato.Substring(0, 2) + "-" + ruta.fecha_inicio_formato.Substring(2,2) + "-" + ruta.fecha_inicio_formato.Substring(4);
                            resultados.routeID = ruta.ruta_id;

                            resultados.routeID = ruta.ruta_id;
                            resultados.perfil_id = ruta.perfil_id.ToString();
                            //resultados.aventon_id = ruta.;
                            resultados.texto_origen = ruta.texto_origen;
                            resultados.texto_destino = ruta.texto_destino;

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
                    MessageBox.Show("Revisa tus datos e intenta de nuevo. tip: fecha mal", "Información inválida",MessageBoxButton.OK);
                    Debug.WriteLine("algo salio mal>>>" + ex.Message);
                    //throw;
                }
                
            }
            

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
        async void  appBarConfirmButton_Click(object sender, EventArgs e)
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
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarResultButton.Text = "Resultados";
                appBarResultButton.Click += appBarResultButton_Click;
                ApplicationBar.Buttons.Add(appBarResultButton);

                ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                fijarme.Text = "fijar mi posición";
                fijarme.Click += fijarme_OnClick;
                ApplicationBar.MenuItems.Add(fijarme);

                ApplicationBarMenuItem b = new ApplicationBarMenuItem { Text = "¿cómo usar?" };
                b.Click += howToUse_Click;
                ApplicationBar.MenuItems.Add(b);

                if (_pointCount <= 0) return;
                ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                changeDestination.Text = "cambiar el destino";
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

                ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarResultButton.Text = "Resultados";
                appBarResultButton.Click += appBarResultButton_Click;
                ApplicationBar.Buttons.Add(appBarResultButton);

                ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                fijarme.Text = "fijar mi posición";
                fijarme.Click += fijarme_OnClick;
                ApplicationBar.MenuItems.Add(fijarme);

                ApplicationBarMenuItem a = new ApplicationBarMenuItem {Text = "¿cómo usar?"};
                a.Click += howToUse_Click;
                ApplicationBar.MenuItems.Add(a);

                if (_pointCount <= 0) return;
                ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                changeDestination.Text = "cambiar el destino";
                changeDestination.Click += ChangeDestinationButton_OnClick;
                ApplicationBar.MenuItems.Add(changeDestination);

            }

            else if (states == appBarStates.Init)
            {
                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarResultButton.Text = "Resultados";
                appBarResultButton.Click += appBarResultButton_Click;
                ApplicationBar.Buttons.Add(appBarResultButton);

                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/u.png", UriKind.Relative));
                a.Text = "cómo usar";
                a.Click += howToUse_Click;
                ApplicationBar.Buttons.Add(a);

                ApplicationBarMenuItem b = new ApplicationBarMenuItem { Text = "¿cómo usar?" };
                b.Click += howToUse_Click;
                ApplicationBar.MenuItems.Add(b);

                ApplicationBarMenuItem fijarme = new ApplicationBarMenuItem();
                fijarme.Text = "fijar mi posición";
                fijarme.Click += fijarme_OnClick;
                ApplicationBar.MenuItems.Add(fijarme);
            }

            else if (states == appBarStates.Waypoint)
            {

                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/check.png", UriKind.Relative));
                a.Text = "confirmar puntos";
                a.Click += Play;
                ApplicationBar.Buttons.Add(a);

                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                appBarSearchButton.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarResultButton.Text = "Resultados";
                appBarResultButton.Click += appBarResultButton_Click;
                appBarResultButton.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarResultButton);


                ApplicationBarMenuItem b = new ApplicationBarMenuItem();
                b.Text = "volver a empezar";//
                b.Click += startOverWayPoints_OnClick;
                ApplicationBar.MenuItems.Add(b);
            }

            else if (states == appBarStates.Shit)
            {


                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/check.png", UriKind.Relative));
                a.Text = "confirmar";
                a.Click += Play;
                ApplicationBar.Buttons.Add(a);

                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("Assets/feature.search.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                appBarSearchButton.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarResultButton.Text = "Resultados";
                appBarResultButton.Click += appBarResultButton_Click;
                appBarResultButton.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarResultButton);


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
                appBarSearchRouteButton.Text = "confirmar";
                appBarSearchRouteButton.Click += appBarSearchRouteButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchRouteButton);

                ApplicationBarIconButton appBarShowMapButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarShowMapButton.Text = "mapa";
                appBarShowMapButton.Click += appBarShowMapButton_Click;
                ApplicationBar.Buttons.Add(appBarShowMapButton);

                ApplicationBarIconButton appBarShowResultsButton = new ApplicationBarIconButton(new Uri("Assets/next.png", UriKind.Relative));
                appBarShowResultsButton.Text = "resultados";
                appBarShowResultsButton.Click += appBarShowResults_Click;
                ApplicationBar.Buttons.Add(appBarShowResultsButton);

                if (_pointCount <= 0) return;
                ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                changeDestination.Text = "cambiar el destino";
                changeDestination.Click += ChangeDestinationButton_OnClick;
                ApplicationBar.MenuItems.Add(changeDestination);
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
                appBarReturnResultsButton.IsEnabled = false;
                ApplicationBar.Buttons.Add(appBarReturnResultsButton);
            }

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
        }

        private void howToUse_Click(object sender, EventArgs e)
        {
            MessageBox.Show("que aqui vaya un tutorial, Fer, ahi te encargo", "Tutorial", MessageBoxButton.OK);
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

            var item = (cajaDeResultados) ResultsListBox.SelectedItem;
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


    }
}