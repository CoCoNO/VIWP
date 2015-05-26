using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Resources;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Windows.Devices.Geolocation;
using System.Device.Location;
using VoyIteso.Class;
using System.Windows.Threading;
using System.Windows.Input;

namespace VoyIteso.Pages
{
    public partial class OfferRoutePage : PhoneApplicationPage
    {

        //Map
        OldMaping searchMap = new OldMaping();
        bool canChangeState;
        bool fullTransition;
        bool isSearchTerm;
        int isOrigin;
        DispatcherTimer LocationTimer = new DispatcherTimer();
        DateTime DATE = new DateTime();
        DateTime TIME = new DateTime();
        String timeStringMin;
        String timeStringMax;
        String dateString;


        //Control Settings rute
        public double fullwidth = Application.Current.Host.Content.ActualWidth;
        public double halfwidth = Application.Current.Host.Content.ActualWidth / 2;
        public double fullandhalf = Application.Current.Host.Content.ActualWidth + Application.Current.Host.Content.ActualWidth / 2;

        //AppBar 
        enum appBarStates { Map, Left, Right};
        appBarStates states;

        //User
        User user = new User();

        //Web Service
        ServiceReferenceVoyItesoMovil.VoyItesoMovilClient clientVoyIteso = new ServiceReferenceVoyItesoMovil.VoyItesoMovilClient();

        public OfferRoutePage()
        {
            InitializeComponent();

            MapPanel.Margin = new Thickness(-fullwidth, 0, 0, 0);

            states = appBarStates.Map;

            // Sample code to localize the ApplicationBar
            BuildLocalizedApplicationBar();
            canChangeState = true;
            fullTransition = false;
            isSearchTerm = false;
            isOrigin = 0;

            //User
            user.getInfo(user.key);

            //Timer
            LocationTimer.Interval = TimeSpan.FromSeconds(1);
            LocationTimer.Tick += LocationTimer_Tick;

            //Data
            datePicker.ValueChanged += datePicker_ValueChanged;
            timePicker.ValueChanged += timePicker_ValueChanged;

            //Animation Events
            ResultPanelShow.Completed += ResultPanelShow_Completed;
            SearchPanelShow.Completed += SearchPanelShow_Completed;
            MapBackFromRight.Completed += MapBackFromRight_Completed;
            MapBackFromLeft.Completed += MapBackFromLeft_Completed;
            
        }

        #region Time & Date Change event
        void timePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            TIME = (DateTime)e.NewDateTime;
            TimeSpan interval = new TimeSpan(0, 15, 0);
            DateTime Min = TIME.Subtract(interval);
            DateTime Max = TIME.Add(interval);
            timeStringMin = Min.ToString("HH:mm:ss") + "'";
            timeStringMax = Max.ToString("HH:mm:ss") + "'";

        }

        void datePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DATE = (DateTime)e.NewDateTime;
            dateString = "'" + DATE.Day + "/" + DATE.Month + "/" + DATE.Year;
        }
        #endregion

        #region LocationTimer_Tick
        void LocationTimer_Tick(object sender, EventArgs e)
        {
            refreshLocation();
            LocationTimer.Stop();
        }
        #endregion

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (searchMap.newInstance)
            {
                searchMap.newInstance = false;

                searchMap.LoadSettings();
                if (searchMap.isLocationAllowed)
                {
                    LocationTimer.Start();
                }

                NavigationService.RemoveBackEntry();
            }

        }
        #endregion

        #region serachTermBox_LostFocus
        private void searchTermBox_LostFocus(object sender, RoutedEventArgs e)
        {            
            SearchTermPanel.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBar.IsVisible = true;
        }
        #endregion

        #region show and hide progressIndicator
        private void showProgressIndicator()
        {
            //searchMap.showProgressIndicator(this);

        }

        private void hideProgressIndicator()
        {
            //searchMap.hideProgressIndicator(this);
        }
        #endregion

        #region getCurrentLocation
        public async void getCurrentLocation()
        {
            //searchMap.showProgressIndicator(this);
            Geolocator geoLocator = new Geolocator();
            geoLocator.DesiredAccuracy = PositionAccuracy.High;

            try
            {

                Geoposition currentPosition = await geoLocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));

                Dispatcher.BeginInvoke(() =>
                {
                    searchMap.myCoordinate = new GeoCoordinate(currentPosition.Coordinate.Latitude, currentPosition.Coordinate.Longitude);
                    searchMap.DrawMapMarkers(myMap);
                    myMap.SetView(searchMap.myCoordinate, 10, MapAnimationKind.Parabolic);
      
                    if (searchMap.myReverseGeocodeQuery == null || !searchMap.myReverseGeocodeQuery.IsBusy)
                    {
                        searchMap.myReverseGeocodeQuery = new ReverseGeocodeQuery();
                        searchMap.myReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(currentPosition.Coordinate.Latitude, currentPosition.Coordinate.Longitude);
                        searchMap.myReverseGeocodeQuery.QueryCompleted += myReverseGeocodeQuery_QueryCompleted_2;
                        searchMap.myReverseGeocodeQuery.QueryAsync();
                    }
                });

            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo encontrar su posicion", "Error", MessageBoxButton.OK);
            }

            //searchMap.hideProgressIndicator(this);

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

                    txtOrigin.Text = direction;
                }
            }
            else
            {
                MessageBox.Show("No se encontraron resultados", "Error", MessageBoxButton.OK);
            }
            searchMap.myReverseGeocodeQuery.Dispose();
        }
        #endregion

        #region Origin and Destiny Tap
        private void txtOrigin_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (canChangeState)
            {
                AnimationBackFromLeft();
            }
            /*
            SearchTermPanel.Visibility = System.Windows.Visibility.Visible;
            searchTermBox.Focus();
            
            searchMap.isSearchingTerm = true;
            isSearchTerm = true;
            */
            isOrigin = 1;
            searchMap.isSearchingTerm = true;
            isSearchTerm = true;
        }

        private void txtDestiny_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (canChangeState)
            {
                AnimationBackFromLeft();
            }
            /*
            SearchTermPanel.Visibility = System.Windows.Visibility.Visible;
            searchTermBox.Focus();
            
            */
            isOrigin = 2;
            searchMap.isSearchingTerm = true;
            isSearchTerm = true;
            
        }
        #endregion

        #region searchTermBox_KeyDown
        private void searchTermBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (searchTermBox.Text.Length > 0)
                {
                    if (searchMap.myMapRoute != null)
                    {
                        myMap.RemoveRoute(searchMap.myMapRoute);
                    }
                    searchMap.myCoordinates.Clear();
                    searchMap.searchForTerm(searchTermBox.Text, this, myMap);
                    searchMap.DrawMapMarkers(myMap);
                    searchMap.isOrigin = isOrigin;

                }
                isSearchTerm = false;
                SearchTermPanel.Visibility = System.Windows.Visibility.Collapsed;
                searchTermBox.Text = "";
                ApplicationBar.IsVisible = true;
            }

        }
        #endregion

        #region Refresh
        public void refreshLocation()
        {
            if (searchMap.isLocationAllowed)
            {
                getCurrentLocation();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Deseas permitir el localizar tu posicion?", "Advertencia", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    searchMap.isLocationAllowed = true;
                    searchMap.SaveSettings();
                    getCurrentLocation();
                }
            }
        }

        void appBarRefreshButton_Click(object sender, EventArgs e)
        {
            refreshLocation();
        }
        #endregion

        #region MapPanel Tap & MapBackFromLeft & MapBackFromRight
        private void MapPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (states == appBarStates.Left && canChangeState)
            {
                AnimationBackFromLeft();
            }
            else if (states == appBarStates.Right && canChangeState)
            {
                AnimtionBackFromRight();
            }

            else if (states == appBarStates.Map && searchMap.isSelectLocation)
            {
                searchMap.mapLayer.CollectionChanged += mapLayer_CollectionChanged;
                searchMap.routeDone = false;
            }
        }


        void MapBackFromLeft_Completed(object sender, EventArgs e)
        {

            states = appBarStates.Map;
            canChangeState = true;
            if (fullTransition)
            {
                AnimationResultShow();
                fullTransition = false;
            }
            else
                BuildLocalizedApplicationBar();

            if (isSearchTerm)
            {
                ApplicationBar.IsVisible = false;
                SearchTermPanel.Visibility = System.Windows.Visibility.Visible;
                searchTermBox.Focus();
            }

        }

        void MapBackFromRight_Completed(object sender, EventArgs e)
        {
            states = appBarStates.Map;
            canChangeState = true;

            if (fullTransition)
            {
                AnimationSearchShow();
                fullTransition = false;
            }
            else
                BuildLocalizedApplicationBar();
        }

        #endregion

        #region mapLayer_CollectionChanged
        void mapLayer_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (searchMap.LocationSelected)
            {
                AnimationSearchShow();
                if (isOrigin == 1)
                {
                    if (searchMap.Direction.Length > 0)
                        txtOrigin.Text = searchMap.Direction;
                    else
                        txtOrigin.Text = "Origen";

                }
                else if (isOrigin == 2)
                {
                    if (searchMap.Direction.Length > 0)
                        txtDestiny.Text = searchMap.Direction;
                    else
                        txtDestiny.Text = "Destino";
                }
                isOrigin = 0;
                searchMap.LocationSelected = false;
            }
        }
        #endregion

        #region SearchPanel
        void appBarSearchButton_Click(object sender, EventArgs e)
        {
            if (states == appBarStates.Map && canChangeState)
            {
                AnimationSearchShow();
            }

        }

        void SearchPanelShow_Completed(object sender, EventArgs e)
        {
            // ApplicationBar.IsVisible = false;
            states = appBarStates.Left;
            canChangeState = true;
            checkRoute();
            BuildLocalizedApplicationBar();
        }
        #endregion

        #region ResultPanel
        void appBarResultButton_Click(object sender, EventArgs e)
        {
            AnimationResultShow();
        }

        void ResultPanelShow_Completed(object sender, EventArgs e)
        {
            states = appBarStates.Right;
            canChangeState = true;
            BuildLocalizedApplicationBar();
        }
        #endregion

        #region Animation
        public void AnimationBackFromLeft()
        {
            if (MapPanel.Margin.Left == -fullwidth)
                MapPanel.Margin = new Thickness(-halfwidth, 0, 0, 0);

            canChangeState = false;

            MapBackFromLeft.Begin();
            //MapBackFromLeft.Completed += MapBackFromLeft_Completed;
        }

        void AnimtionBackFromRight()
        {
            if (MapPanel.Margin.Left == -fullwidth)
                MapPanel.Margin = new Thickness(-fullandhalf, 0, 0, 0);

            canChangeState = false;

            MapBackFromRight.Begin();
            //MapBackFromRight.Completed += MapBackFromRight_Completed;
            
        }

        void AnimationSearchShow()
        {
            if (MapPanel.Margin.Left == -halfwidth)
                MapPanel.Margin = new Thickness(-fullwidth, 0, 0, 0);

            canChangeState = false;

            SearchPanelShow.Begin();
            //SearchPanelShow.Completed += SearchPanelShow_Completed;
            
        }

        void AnimationResultShow()
        {
            if (states == appBarStates.Map && canChangeState)
            {
                if (MapPanel.Margin.Left == -fullandhalf)
                    MapPanel.Margin = new Thickness(-fullwidth, 0, 0, 0);

                canChangeState = false;

                ResultPanelShow.Begin();
                //ResultPanelShow.Completed += ResultPanelShow_Completed;
                
            }
        }
        #endregion

        #region checkRoute
        ///method for cheking the validation to print route
        private void checkRoute()
        {
            if (txtOrigin.Text != "Direccion" && txtDestiny.Text != "Direccion" && !searchMap.isRouteShown)
            {

                searchMap.isRouteSearch = true;
                List<GeoCoordinate> PointAB = new List<GeoCoordinate>();
                if (searchMap.locationA != null)
                    PointAB.Add(searchMap.locationA.GeoCoordinate);
                else
                    PointAB.Add(searchMap.myCoordinate);

                PointAB.Add(searchMap.locationB.GeoCoordinate);
                searchMap.CalculateRoute(PointAB, this, myMap);
            }
            else
                searchMap.isRouteSearch = false;

        }
        #endregion


        #region appBarButtons
        void appBarSearchRouteButton_Click(object sender, EventArgs e)
        {
            //run the search algorithm y take the results from the web service
            showProgressIndicator();
            //getResultsOfSearch();
        }

        void appBarShowResults_Click(object sender, EventArgs e)
        {
            fullTransition = true;
            AnimationBackFromLeft();
            states = appBarStates.Right;
            //BuildLocalizedApplicationBar();
        }

        void appBarShowMapButton_Click(object sender, EventArgs e)
        {
            AnimationBackFromLeft();
            states = appBarStates.Map;
            //BuildLocalizedApplicationBar();
        }
        void appBarShowSearch_Click(object sender, EventArgs e)
        {
            fullTransition = true;
            AnimtionBackFromRight();
            states = appBarStates.Left;
            //BuildLocalizedApplicationBar();
        }

        void appBarShowMapFromRight_Click(object sender, EventArgs e)
        {
            AnimtionBackFromRight();
            states = appBarStates.Map;
            //BuildLocalizedApplicationBar();
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
                
                ApplicationBarIconButton appBarSearchButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
                appBarSearchButton.Text = "Buscar Ruta";
                appBarSearchButton.Click += appBarSearchButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchButton);

                ApplicationBarIconButton appBarResultButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
                appBarResultButton.Text = "Resultados";
                appBarResultButton.Click += appBarResultButton_Click;
                ApplicationBar.Buttons.Add(appBarResultButton);

                ApplicationBarIconButton appBarRefreshButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
                appBarRefreshButton.Text = "Actualizar Posicion";
                appBarRefreshButton.Click += appBarRefreshButton_Click;
                ApplicationBar.Buttons.Add(appBarRefreshButton);
            }
            else if (states == appBarStates.Left)
            {
                ApplicationBarIconButton appBarSearchRouteButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
                appBarSearchRouteButton.Text = "Buscar";
                appBarSearchRouteButton.Click += appBarSearchRouteButton_Click;
                ApplicationBar.Buttons.Add(appBarSearchRouteButton);

                ApplicationBarIconButton appBarShowMapButton = new ApplicationBarIconButton(new Uri("Assets/AppBar/appbar.add.rest.png",UriKind.Relative));
                appBarShowMapButton.Text = "Mostrar Mapa";
                appBarShowMapButton.Click += appBarShowMapButton_Click;
                ApplicationBar.Buttons.Add(appBarShowMapButton);

                ApplicationBarIconButton appBarShowResultsButton = new ApplicationBarIconButton(new Uri("Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
                appBarShowResultsButton.Text = "Mostrar resultados";
                appBarShowResultsButton.Click += appBarShowResults_Click;
                ApplicationBar.Buttons.Add(appBarShowResultsButton);
            }

            else if (states == appBarStates.Right)
            {
                ApplicationBarIconButton appBarShowMapFromRightButton = new ApplicationBarIconButton(new Uri("Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
                appBarShowMapFromRightButton.Text = "Mostrar Mapa";
                appBarShowMapFromRightButton.Click += appBarShowMapFromRight_Click;
                ApplicationBar.Buttons.Add(appBarShowMapFromRightButton);

                ApplicationBarIconButton appBarShowSearchButton = new ApplicationBarIconButton(new Uri("Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
                appBarShowSearchButton.Text = "Realizar Busqueda";
                appBarShowSearchButton.Click += appBarShowSearch_Click;
                ApplicationBar.Buttons.Add(appBarShowSearchButton);
            }

            //    // Create a new menu item with the localized string from AppResources.
            //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
            //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        }

        

        #endregion
    }
}