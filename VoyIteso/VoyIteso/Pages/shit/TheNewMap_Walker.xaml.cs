using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
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
using Telerik.Windows.Controls;
using VoyIteso.Resources;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

/*la busqueda de ruta exige:
        *
         * 
         * puntos a y b
         * fecha de salida: hora, dia, mes, ano.// que se me hace una pendejada, la hora era suficiente, pero asi lo diseno comunicacion social (hate them!)
         * nombre del a y del b // otra pendejada mas ya que no son mas que para mostrar un texto.
         * fumador. por si quieres ir con alguien que fuma o no fuma.
         * genero. por si quieres ir con una mujer o un hombre. En la primera version voy a poner estas ultimas dos por defecto ya que no quiero q voy iteso caiga por su mal diseno. 
         * 
         * 
         */
//

namespace VoyIteso.Pages
{
    public partial class TheNewMap_Walker : PhoneApplicationPage
    {

        #region fields

        //banderas para manejar los eventos.
        private bool a = false;//el punto a (origen)
        private bool b = false;//el punto b (destino)
        private bool rutaConfirmada = false;// cuando a y b entonces rutaConfirmada.
        private bool horaDeSalida = false;//
        private bool fechaDeSalida = false;
        //el temporizador que va a estar contando el tiempo para checar mis banderas cada x tiempo. x = un segundo.
        readonly DispatcherTimer _timer = new DispatcherTimer();
        readonly Image _pushPinUsuario = new Image();
        //readonly ProgressIndicator _progress = new ProgressIndicator();
        private MapLayer _layer;

        private bool _confirmed;
        private bool _aconfirmed;
        private bool _routeConfirmed;// esta pendejada sirve para que hacer que no se pueda mover una vez q estan fijados los puntos a y b.
        private int _pointCount;

        public static MapOverlay BPoint { private set; get; }
        public static MapOverlay APoint { get; private set; }
        private MapLayer _aPoint;
        private List<MapOverlay> wayPointList = new List<MapOverlay>();
        private List<MapLayer> waylayerList = new List<MapLayer>();
        public static bool Driver = false;


        #endregion

        public TheNewMap_Walker()
        {
            InitializeComponent();

            //desmadre del timer.
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += TimerOnTick;
            _timer.Start();

            ResetValues();
            Driver = true;//borrar despues. 
            _pushPinUsuario.Source = new BitmapImage(new Uri("/Images/u.png", UriKind.Relative));

            Loaded += SearchView_Loaded;
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "acc0d8e8-cffc-4bcb-9d28-06444a2fc7d8";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "0FvJj6wXx2HVKh7g-6hRGw";

            #region cannibaled constructor
            //iniciaremos preguntando la fecha y la hora asi que nos enfocamos en el comienzo en el panel izquierdo del menu hamburguesa.
            
            canChangeState = true;
            isSearchTerm = false;
            isOrigin = true;
            isConfirmRoute = false;

            datePicker.ValueChanged += datePicker_ValueChanged;
            timePicker.ValueChanged += timePicker_ValueChanged;

            progress = new Progress();

            BuildLocalizedApplicationBar();

            setLayout();

            ShowLeftPanelAnimation.Completed += ShowLeftPanelAnimation_Completed;
            ShowLeftPanelFromRightAnimation.Completed += ShowLeftPanelFromRightAnimation_Completed;
            ShowRightPanelAnimation.Completed += ShowRightPanelAnimation_Completed;
            ShowRightPanelFromLeftAnimation.Completed += ShowRightPanelFromLeftAnimation_Completed;
            HideLeftPanelAnimation.Completed += HideLeftPanelAnimation_Completed;
            HideRightPanelAnimation.Completed += HideRightPanelAnimation_Completed; 
              
            #endregion
            
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            //timer.Stop();
            //well here we gonna have to code the function call. the function that will check the event flags. 
            Debug.WriteLine("timing....");
            CheckEventFlag();

        }

        #region methods

        private async void SearchView_Loaded(object sender, RoutedEventArgs e)// este es el metodo que se dispara en cuanto la interfaz grafica se ha cargado y mostrado exitosamente en el telefono.
        {

            //este se utiliza para que cuando se regrese el usuario no le salga la ventana de seleccionar entre aventon o dar ride, el usuario va a regresar directamente al menu principal.
            NavigationService.RemoveBackEntry();
            //el progreso debe parar ya que ya se tiene una ubicacion para el usuario.
            ConfirmedChanged += MyConfirmedChanged;

            Microsoft.Phone.Shell.SystemTray.ForegroundColor = System.Windows.Media.Color.FromArgb(255, 0, 66, 112);
            RouteResult.ResultHeight = (int)(Application.Current.Host.Content.ActualHeight / 4);
            RouteResult.ResultWidth = (int)((Application.Current.Host.Content.ActualWidth / 4) * 3);
            BuildLocalizedApplicationBar();
            ApplicationBar.IsVisible = false;

            if (!a)
            {
                ObtenerPuntoAyb();
            }
            
        }

        private void ObtenerPuntoAyb()
        {
            var dialogResult = MessageBox.Show("tu punto de partida se fijará en el iteso, si cancelas se fija tu posición", "¿vienes del ITESO?", MessageBoxButton.OKCancel);
            if (dialogResult == MessageBoxResult.OK)
            {
                FijarIteso();
                a = true;
                progress.showProgressIndicator(this, "fijando el iteso");
                MessageBox.Show("presiona y manten en el mapa para colocar el punto a donde vas", "¿a dónde vas?", MessageBoxButton.OK);
            }
            else
            {
                FijarPosicionActual();
                txtOrigin.Text = direccionOrigen;
                a = true;

                progress.showProgressIndicator(this, "fijando tu posición");
                //hacer que se fije el punto b en el iteso  ya que es obvio que debe ir al iteso. para eso esta pensada la app. 
                AddItesoPoint(null, null);
                txtOrigin.Text = direccionDestino;
                b = true;
            }

            progress.hideProgressIndicator(this);
        }

        private void CheckEventFlag()
        {
        
            if (a)
            {
                if (b)
                {
                    if (fechaDeSalida && horaDeSalida)
                    {

                        cargarAResultados();
                        
                        _f = true;
                        if (states!=appBarStates.Foo)
                        {
                            if (_f)
                            {
                                MessageBox.Show("revisa que tanto el punto de origen como de partida estén correctos y confirma.","Atención",MessageBoxButton.OK);
                                ShowRightPanelAnimation.Begin();
                                states = appBarStates.Foo;
                                BuildLocalizedApplicationBar();
                                _f = false;
                            }
                            

                        }
                        if (rutaConfirmada)
                        {
                            MessageBox.Show("ruta confirmada, mandar parametros");
                            //aqui puto jairo
                        }
                        
                    }
                    else
                    {
                        
                        if (!horaDeSalida&&fechaDeSalida)
                        {
                            askForTheTime();
                        }
                        else if (!fechaDeSalida)
                        {
                            askForDate();
                        }
                        
                        
                    }
                }
            }
        }

        private void cargarAResultados()
        {

            var a = new ListBoxItem();
            a.Content = new TextBlock().Text = direccionOrigen;
            ResultsListBox.Items.Add(a);
            var b = new ListBoxItem();
            a.Content = new TextBlock().Text = direccionDestino;
            ResultsListBox.Items.Add(b);
            

        }

        private void askForDate()
        {
            if (states!=appBarStates.Left)
            {
                ShowLeftPanelAnimation.Begin();
                MessageBox.Show("especifica la fecha");
            }
            
        }

        private void askForTheTime()
        {
            if (states != appBarStates.Left)
            {
                ShowLeftPanelAnimation.Begin();
                
            }
            if (_f)
            {
                MessageBox.Show("especifica la hora");
                _f = false;
            }
            
            ; 
        }

        private async void FijarIteso()//si el brother escoge que viene del iteso
        {
            GeoCoordinate migeoCoordenada = new GeoCoordinate(20.608390, -103.414512);
            dibujaru(migeoCoordenada);
            myMap.Center = migeoCoordenada;
            myMap.ZoomLevel = 13;
            //ApplicationBar.IsVisible = true;
            //_aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            //MessageBox.Show("por defecto, el origen está fijado en el ITESO, pero puedes fijar tu posición");
        }

        private async void FijarPosicionActual()// si el brother escoge que no viene del iteso
        {
            Geolocator migeolocalizador = new Geolocator();
            Geoposition migeoposicion = await migeolocalizador.GetGeopositionAsync();
            Geocoordinate migeocoordenada = migeoposicion.Coordinate;
            GeoCoordinate migeoCoordenada = convertidirGeocoordinate(migeocoordenada);
            dibujaru(migeoCoordenada);
            myMap.Center = migeoCoordenada;
            myMap.ZoomLevel = 13;

            //reverse code query aqui.
            //get properties of the address from the tapped location. we are repeating ourselves, there is a need to refactor code later.
            var query = new ReverseGeocodeQuery { GeoCoordinate = new GeoCoordinate(migeoCoordenada.Latitude, migeoCoordenada.Longitude) };
            query.QueryCompleted += (s, ev) =>
            {
                if (ev.Error == null && ev.Result.Count > 0)
                {
                    var locations = ev.Result as List<MapLocation>;
                    // do what you want with the locations...
                    foreach (var newLocation in locations)
                    {
                        //feed.Text += newLocation.Information.Address.Street.ToString();
                        
                            //newLocation.Information.Address.Street.ToString();
                            //newLocation.Information.Description.ToString();
                            //newLocation.Information.Address.BuildingName.ToString();
                            direccionOrigen = newLocation.Information.Address.Street.ToString() + " " + newLocation.Information.Address.HouseNumber + "\n" +
                            newLocation.Information.Address.District.ToString();


                    }

                }
            };
            query.QueryAsync();


//ApplicationBar.IsVisible = true;
            //_aconfirmed = true;
            //cargarlista();//cargar la lista de pushpins sugeridos.
            //MessageBox.Show("origen está fijado en tu posición");
        }

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

        private void ResetValues()//this method returns all flags to false;
        {
            a = false;
            b = false;
            rutaConfirmada = false;
            horaDeSalida = false;
            fechaDeSalida = false;

        }

        /// <summary>
        /// es disparado cuando hay punto a y b fijados. (origen y destino)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void MyConfirmedChanged(object sender, EventArgs eventArgs)// hay que eliminar esta mierda q es inecesaria
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

                        direccionDestino = newLocation.Information.Address.Street.ToString() + " " + newLocation.Information.Address.HouseNumber + "\n" +
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

        /// <summary>
        /// Adds the destination point to the map. and does take the value of the local mapOverlay and takes it to the main context, it also gets the temp layer and takes it to the main context, changes the appbarmode to default.
        /// the mapOverlay object contains the ubicacion and the map icon
        /// </summary>
        private async void AddItesoPoint(object sender, GestureEventArgs e)
        {
            //_bPointAdded = true;
            //Find geocoordinate on tapped location.
            var newLayer = new MapLayer();
            var mapIcon = new MapIcon();//my Own map icon. (custon map icon).
            var bpoint = new MapOverlay();
            var ubicacion = new GeoCoordinate(20.608499, -103.414630);//el iteso 20.608499, -103.414630
            bpoint.GeoCoordinate = ubicacion;
            bpoint.Content = mapIcon;
            newLayer.Add(bpoint);
            //add pushpin layer to map layers.
            myMap.Layers.Add(newLayer);
            _confirmed = true;//esta bandera es la que hace que cuando se mueve el centro del mapa se borre el bpoint debido a q no ha sido confirmado.

            //get properties of the address from the tapped location. 
            var query = new ReverseGeocodeQuery { GeoCoordinate = new GeoCoordinate(20.608499, -103.414630) };
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

                        direccionDestino = newLocation.Information.Address.Street.ToString() + " " + newLocation.Information.Address.HouseNumber + "\n" +
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
            appBarSearchButton_Click(null, null);
        }

        /// <summary>
        /// este es el boton de confirmar, 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            //SearchNowButton_OnClick(null, null);
            //la flag es la que indica que ya existen tanto punto A (origen) como punto B (destino).
            if (_flag)
            {
                if (Driver)
                {
                    if (wayPointList.Count > 0)
                    {
                        var result = MessageBox.Show("vas fijar los puntos intermedios a tu ruta, si cancelas tendrás que volver a agregar todos los puntos", "confirmar operación", MessageBoxButton.OKCancel);
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
                    NavigateToSecundaryPage();
                }

            }
            //aqui es cuando el usuario le pica a confirmar el destino.
            else
            {
                _confirmed = true;
                var mapIcon = BPoint.Content;
                MessageBox.Show("destino fijado");//falta ponerle en donde 
                //
                //MyMapControl.Layers.Add(layer);   
                SearchNowButton_OnClick(null, null);
            }
            ApplicationBar.Mode = ApplicationBarMode.Minimized;
        }

        private void ChangeDestinationButton_OnClick(object sender, EventArgs e)
        {
            ResetValues();
            MyMapControl_OnCenterChanged(null, null);
            myMap.Layers.Remove(_layer);
            MessageBox.Show("Coloca un nuevo destino", "destino borrado", MessageBoxButton.OK);
        }

        private void FindMeButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                myMap.Layers.Remove(_aPoint);
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
                _flag = false;
            }
        }

        private void MyMapControl_OnCenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            states = appBarStates.Map;
            BuildLocalizedApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            if (!_confirmed)
            {
                myMap.Layers.Remove(_layer);
            }

        }

        /// <summary>
        /// This event adds a new layer with a puonshpin on the holded location with district and street and number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMapControl_OnHold(object sender, GestureEventArgs e)
        {
            

            //if the destination has not been added yet and it has no confirmation, then a new bpoint is going to be added to the map.
             if (!_confirmed)
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
        String dateString;
        Progress progress;
        int intSmoker;
        int intGender;
        RouteResult currentRoute;

        public enum appBarStates { Map, Left, Right, Search, Confirm, Shit, Foo };
        appBarStates states;

        //User user = new User();

        BingConnectorcs bingConnector = new BingConnectorcs();

        List<string> autoComplemapingreets;
        Maping maping;
        private string direccionDestino;
        private string direccionOrigen;
        private bool _f = true;
        //ApiConnector apiConnector;
        #endregion

        #region Time & Date Change event
        void timePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            timeString = string.Format("{0:HH:mm}", e.NewDateTime);
            horaDeSalida = true;
        }

        void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            TimeSpan time = ((DateTime)e.NewDateTime).Subtract(DateTime.Now);
            if (time.Days >= 0)
                dateString = string.Format("{0:dd-MM-yyyy}", e.NewDateTime);
            else
                MessageBox.Show("No puedes pedir aventones en el pasado", "¡No te pases de listo!", MessageBoxButton.OK);
            fechaDeSalida = true;
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

            else if (states == appBarStates.Foo)
            {
                ApplicationBarIconButton confirmarRuta = new ApplicationBarIconButton(new Uri("Assets/feature.check.png", UriKind.Relative));
                confirmarRuta.Text = "confirmar ruta";
                confirmarRuta.Click += confirmarRuta_Click;
                ApplicationBar.Buttons.Add(confirmarRuta);

                ApplicationBarMenuItem comenzarDeNuevo = new ApplicationBarMenuItem();
                comenzarDeNuevo.Text = "comenzar todo de nuevo";
                comenzarDeNuevo.Click += comenzarDeNuevo_onClick;
                ApplicationBar.MenuItems.Add(comenzarDeNuevo);

            }
            else if (states == appBarStates.Shit)
            {
                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("/Images/appbar_button1.png", UriKind.Relative));
                a.Text = "Confirmar";
                a.Click += ApplicationBarIconButton_OnClick;
                ApplicationBar.Buttons.Add(a);

                ApplicationBarMenuItem changeDestination = new ApplicationBarMenuItem();
                changeDestination.Text = "cambiar el destino";
                changeDestination.Click += ChangeDestinationButton_OnClick;

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

        private void comenzarDeNuevo_onClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SelectTypePage.xaml", UriKind.Relative))
                ;
        }

        private void confirmarRuta_Click(object sender, EventArgs e)
        {
            rutaConfirmada = true;
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
