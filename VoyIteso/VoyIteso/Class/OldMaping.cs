using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VoyIteso.Resources;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using VoyIteso;

namespace VoyIteso.Class
{
    public class OldMaping
    {
        //public ProgressIndicator progressIndicator;
        public bool newInstance;
        public bool isLocationAllowed;
        public bool LocationSelected;
        public bool isSelectLocation;
        public bool isRouteSearch;
        public bool isRouteShown;
        public bool routeDone;
        public bool isSearchingTerm;
        public GeoCoordinate myCoordinate;
        public ReverseGeocodeQuery myReverseGeocodeQuery;
        public GeocodeQuery myGeocodeQuery;
        public RouteQuery myRouteQuery;
        public Route myRoute;
        public MapRoute myMapRoute;
        public MapLayer mapLayer;
        public MapOverlay locationOverlay;
        public MapOverlay locationA;
        public MapOverlay locationB;
        public List<GeoCoordinate> myCoordinates = new List<GeoCoordinate>();
        public String Direction;
        public int isOrigin;

        private IsolatedStorageSettings Settings;

        public OldMaping()
        {
            //progressIndicator = new ProgressIndicator();
            newInstance = true;
            isLocationAllowed = false;
            LocationSelected = false;
            isSelectLocation = false;
            isRouteSearch = false;
            routeDone = false;
            isRouteShown = false;
            isSearchingTerm = false;
            Settings = IsolatedStorageSettings.ApplicationSettings;
            isOrigin = 0;
        }

        public void searchForTerm(String searchTerm, System.Windows.DependencyObject searchPage, Map myMap)
        {
            //isSearchOn = true;
            //showProgressIndicator(searchPage);
            myGeocodeQuery = new GeocodeQuery();
            myGeocodeQuery.SearchTerm = searchTerm;
            myGeocodeQuery.GeoCoordinate = myCoordinate == null ? new GeoCoordinate(0, 0) : myCoordinate;
            myGeocodeQuery.QueryCompleted += (sender, e) => { myGeocodeQuery_QueryCompleted(sender, e, searchPage, myMap); };
            myGeocodeQuery.QueryAsync();
        }


        void myGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e, System.Windows.DependencyObject searchPage, Map myMap)
        {
            //hideProgressIndicator(searchPage);

            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    /*
                    for (int i = 0; i < e.Result.Count; i++)
                    {
                        myCoordinates.Add(e.Result[i].GeoCoordinate);
                    }*/
                    myCoordinates.Add(e.Result[0].GeoCoordinate);

                    myMap.SetView(myCoordinates[0], 13.5, MapAnimationKind.Parabolic);
                }
                else
                {
                    MessageBox.Show("No se encontro ningun resultado", "Error", MessageBoxButton.OK);
                }

                myGeocodeQuery.Dispose();
            }
            DrawMapMarkers(myMap);
        }

        

        public void CalculateRoute(List<GeoCoordinate> route, System.Windows.DependencyObject searchPage, Map myMap)
        {
            //showProgressIndicator(searchPage);
            myRouteQuery = new RouteQuery();
            myRouteQuery.TravelMode = TravelMode.Driving;
            myRouteQuery.Waypoints = route;
            myRouteQuery.QueryCompleted += (sender, e) => { myRouteQuery_QueryCompleted(sender, e, searchPage, myMap); };
            myRouteQuery.QueryAsync();
        }

        void myRouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e,System.Windows.DependencyObject searchPage, Map myMap)
        {
            if (!routeDone)
            {
                //hideProgressIndicator(searchPage);
                if (e.Error == null)
                {
                    myRoute = e.Result;
                    myMapRoute = new MapRoute(myRoute);
                    myMap.AddRoute(myMapRoute);
                    routeDone = true;
                    isRouteShown = true;
                    myMap.SetView(myCoordinate, 16, MapAnimationKind.Parabolic);
                    myRouteQuery.Dispose();
                }
                //DrawMapMarkers(myMap);
                myMap.SetView(myRoute.Legs[0].BoundingBox,MapAnimationKind.Parabolic);
                isSearchingTerm = false;
            }
            
        }

        public void DrawMapMarkers(Map myMap)
        {
            myMap.Layers.Clear();
            mapLayer = new MapLayer();

            // Draw marker for current position
            if (myCoordinate != null)
            {
                DrawMapMarker(myCoordinate, Colors.Red, mapLayer, myMap);
            }

            // Draw markers for location(s) / destination(s)
            for (int i = 0; i < myCoordinates.Count; i++)
            {
                DrawMapMarker(myCoordinates[i], Colors.Blue, mapLayer, myMap);
            }

            // Draw markers for possible waypoints when directions are shown.
            // Start and end points are already drawn with different colors.
            /*
            if (isSearchOn)
            {
                if (myRoute.LengthInMeters > 0 && myRoute != null)
                {
                    for (int i = 1; i < myRoute.Legs[0].Maneuvers.Count - 1; i++)
                    {
                        DrawMapMarker(myRoute.Legs[0].Maneuvers[i].StartGeoCoordinate, Colors.Purple, mapLayer);
                    }
                }
            }*/

            myMap.Layers.Add(mapLayer);
        }

        public void DrawMapMarker(GeoCoordinate coordinate, Color color, MapLayer mapLayer, Map myMap)
        {
            Ellipse Circhegraphic = new Ellipse();
            if (!myMap.IsEnabled && color != Colors.Red)
                Circhegraphic.Fill = new SolidColorBrush(Colors.Purple);
            else
                Circhegraphic.Fill = new SolidColorBrush(color);

            Circhegraphic.Height = 30;
            Circhegraphic.Width = 30;
            //Circhegraphic.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            Circhegraphic.Tag = "false";
            Circhegraphic.MouseLeftButtonDown += (sender, e) => { Circhegraphic_MouseLeftButtonDown(sender, e, coordinate, color); };
            if(color != Colors.Red)
                Circhegraphic.Hold += (sender, e) => { Circhegraphic_Hold(sender, e, myMap); };

            locationOverlay = new MapOverlay();
            locationOverlay.Content = Circhegraphic;
            locationOverlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            locationOverlay.PositionOrigin = new Point(0.5, 0.5);
            mapLayer.Add(locationOverlay);
        }

        private void Circhegraphic_Hold(object sender, System.Windows.Input.GestureEventArgs e, Map myMap)
        {
            Ellipse Circhegraphic = (Ellipse)sender;
            Circhegraphic.Fill = new SolidColorBrush(Colors.Purple);
            myMap.IsEnabled = false;
        }


        void Circhegraphic_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e,GeoCoordinate coordinate, Color color)
        {
            Ellipse ellipse = new Ellipse();
            ellipse = (Ellipse)sender;

            if (!isSelectLocation)
            {
                DrawPoligon(coordinate, color);
                isSelectLocation = true;
                ellipse.Tag = "true";
            }
            else
            {
                if (ellipse.Tag.ToString()=="true")
                {
                    ClearSelection(sender);
                    ellipse.Tag = "false";
                }
                else
                {
                    //ClearSelection();
                    DrawPoligon(coordinate, color);
                    ellipse.Tag = "true";
                }
            }

            
        }
        

        public void DrawPoligon(GeoCoordinate coordinate, Color color)
        {
            Grid MyGrid = new Grid();
            MyGrid.RowDefinitions.Add(new RowDefinition());
            MyGrid.RowDefinitions.Add(new RowDefinition());
            MyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            MyGrid.Background = new SolidColorBrush(Colors.Transparent);
            MyGrid.Tag = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);

            //Creating a Rectangle
            Rectangle MyRectangle = new Rectangle();
            MyRectangle.Fill = new SolidColorBrush(color);
            MyRectangle.Height = 50;
            MyRectangle.Width = 150;
            MyRectangle.Margin = new Thickness(0);
            MyRectangle.HorizontalAlignment = HorizontalAlignment.Left;
            MyRectangle.SetValue(Grid.RowProperty, 0);
            MyRectangle.SetValue(Grid.ColumnProperty, 0);

            TextBlock txtDirection = new TextBlock();
            //txtDirection.Text = name;
            txtDirection.FontSize = 20;
            txtDirection.VerticalAlignment = VerticalAlignment.Center;
            txtDirection.HorizontalAlignment = HorizontalAlignment.Left;
            txtDirection.Margin = new Thickness(15, 0, 5, 0);
            txtDirection.Foreground = new SolidColorBrush(Colors.White);
            txtDirection.Width = 120;
            txtDirection.Height = 50;
            txtDirection.TextWrapping = TextWrapping.NoWrap;
            txtDirection.SetValue(Grid.RowProperty, 0);
            txtDirection.SetValue(Grid.ColumnProperty, 0);

            TextBlock txtDirection2 = new TextBlock();
           // txtDirection2.Text = description;
            txtDirection2.FontSize = 15;
            txtDirection2.VerticalAlignment = VerticalAlignment.Center;
            txtDirection2.HorizontalAlignment = HorizontalAlignment.Left;
            txtDirection2.Margin = new Thickness(15, 30, 15, 3);
            txtDirection2.Foreground = new SolidColorBrush(Colors.White);
            txtDirection2.Width = 60;
            txtDirection2.Height = 17;
            txtDirection2.TextWrapping = TextWrapping.NoWrap;
            txtDirection2.SetValue(Grid.RowProperty, 0);
            txtDirection2.SetValue(Grid.ColumnProperty, 0);

            //Adding the Rectangle to the Grid
            MyGrid.Children.Add(MyRectangle);
            MyGrid.Children.Add(txtDirection);
            MyGrid.Children.Add(txtDirection2);

            //Creating a Polygon
            Polygon MyPolygon = new Polygon();
            MyPolygon.Points.Add(new Point(0, 0));
            MyPolygon.Points.Add(new Point(15, 0));
            MyPolygon.Points.Add(new Point(2, 30));
            MyPolygon.Stroke = new SolidColorBrush(color);
            MyPolygon.Fill = new SolidColorBrush(color);
            MyPolygon.SetValue(Grid.RowProperty, 1);
            MyPolygon.SetValue(Grid.ColumnProperty, 1);

            //Adding the Polygon to the Grid
            MyGrid.Children.Add(MyPolygon);

            

            
            if (myReverseGeocodeQuery == null || !myReverseGeocodeQuery.IsBusy)
            {
                myReverseGeocodeQuery = new ReverseGeocodeQuery();
                myReverseGeocodeQuery.GeoCoordinate = new GeoCoordinate(coordinate.Latitude,coordinate.Longitude);
                myReverseGeocodeQuery.QueryCompleted += (sender, e) => { myReverseGeocodeQuery_QueryCompleted(sender, e, txtDirection, txtDirection2); };
                myReverseGeocodeQuery.QueryAsync();
                MyGrid.Tap += MyGrid_Tap;
            }

            MapOverlay overlay = new MapOverlay();
            overlay.Content = MyGrid;
            overlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            overlay.PositionOrigin = new Point(0.0, 1.0);
            mapLayer.Add(overlay);
        }

        void MyGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isSearchingTerm)
            {
                Grid grid = new Grid();
                grid = (Grid)sender;
                TextBlock dir;
                dir = (TextBlock)grid.Children.ElementAt(1);
                Direction = dir.Text.ToString();
                dir = (TextBlock)grid.Children.ElementAt(2);
                Direction += dir.Text.ToString();
                LocationSelected = true;
                ClearSelection(grid);
                isRouteShown = false;
            }
        }

        void myReverseGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e,TextBlock txtDirection, TextBlock txtDirection2)
        {
            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    MapAddress address = e.Result[0].Information.Address;
                    String direction = "";
                    String direction2 = "";

                    if (address.Street.Length > 0)
                    {
                        direction += address.Street;
                    }
                    if (address.HouseNumber.Length > 0)
                    {
                        direction2 += "#" + address.HouseNumber + " ";
                        if (address.Neighborhood.Length > 0)
                            direction2 += address.Neighborhood;
                    }

                    txtDirection.Text = direction;
                    txtDirection2.Text = direction2;
                }
            }
            else
            {
                MessageBox.Show("No se encontraron resultados", "Error", MessageBoxButton.OK);
            }
            myReverseGeocodeQuery.Dispose();
        }

        #region Load and Save Settings
        public void LoadSettings()
        {
            if (Settings.Contains("isLocationAllowed"))
            {
                isLocationAllowed = (bool)Settings["isLocationAllowed"];
            }
            else
            {
                isLocationAllowed = true;
            }
        }

        public void SaveSettings()
        {
            if (Settings.Contains("isLocationAllowed"))
            {
                if ((bool)Settings["isLocationAllowed"] != isLocationAllowed)
                {
                    Settings["isLocationAllowed"] = isLocationAllowed;
                }
            }
            else
            {
                Settings.Add("isLocationAllowed", isLocationAllowed);
            }
        }
        #endregion

        #region ClearSelection
        //to clean the grid info
        public void ClearSelection(object sender)
        {
            
            Ellipse ellipse = new Ellipse();
            ellipse = (Ellipse)sender;

            Grid grid;

            for(int i=0;i<mapLayer.Count;i++)
            {
                if (mapLayer.ElementAt(i).Content.GetType() == typeof(Ellipse) && mapLayer.ElementAt(i).Content.Equals(ellipse))
                {
                    MapOverlay overlay = new MapOverlay();
                    overlay = mapLayer.ElementAt(i);

                    for (int j = 0; j < mapLayer.Count; j++)
                    {
                        if (mapLayer.ElementAt(j).Content.GetType() == typeof(Grid))
                        {
                            grid = mapLayer.ElementAt(j).Content as Grid;
                            if (overlay.GeoCoordinate.Equals((GeoCoordinate)grid.Tag))
                            {
                                mapLayer.RemoveAt(j);
                            }
                        }
                        
                    }
                        
                }

            }

            if (mapLayer.Count > 1)
            {
                
                for (int i = 0; i < mapLayer.Count; i++)
                {
                    if (mapLayer.ElementAt(i).Content.GetType() == typeof(Grid))
                    {
                        isSelectLocation = true;

                        break;
                    }
                    if(i==mapLayer.Count-1)
                        isSelectLocation = false;
                }
            }

        }

        public void ClearSelection(Grid grid)
        {
            ///limpia todos los puntos menos el current y la busqueda recien realizada
            mapLayer.RemoveAt(mapLayer.Count-1);
            for (int i = 0; i < mapLayer.Count; i++)
            {
                if (mapLayer.ElementAt(i).Content.GetType() == typeof(Ellipse))
                {
                    if (mapLayer.ElementAt(i).GeoCoordinate == (GeoCoordinate)grid.Tag)
                    {
                        MapOverlay overleyLocation = new MapOverlay();
                        overleyLocation = mapLayer.ElementAt(0);

                        if (isOrigin == 1)
                        {
                            locationA = new MapOverlay();
                            locationA = mapLayer.ElementAt(i) as MapOverlay;
                        }
                        else if (isOrigin == 2)
                        {
                            locationB = new MapOverlay();
                            locationB = (MapOverlay)mapLayer.ElementAt(i) as MapOverlay;
                        }

                        mapLayer.Clear();
                        mapLayer.Add(overleyLocation);
                        if (locationA != null)
                        {
                            //DrawMapMarker(locationA.GeoCoordinate, Colors.Blue, mapLayer);
                        }
                        if (locationB != null)
                        {
                            //DrawMapMarker(locationB.GeoCoordinate,Colors.Blue,mapLayer);
                        }
                    }
                }
            }
            isSelectLocation = false;
        }
        #endregion


        
    }
}
