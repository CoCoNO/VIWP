using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VoyIteso.Pages;

namespace VoyIteso.Class
{
    class Maping
    {
        public GeoCoordinate myCoordinate;
        public ReverseGeocodeQuery myReverseGeocodeQuery;
        public GeocodeQuery myGeocodeQuery;
        public MapLayer mapLayer;
        public MapOverlay locationA;
        public MapOverlay locationB;
        public List<GeoCoordinate> myCoordinates;
        private IsolatedStorageSettings Settings;
        public Map myMap;
        public GeoCoordinate pointGeoCoordinate;
        public Canvas canvas;
        public bool newInstance;
        public RouteQuery myRouteQuery;
        public Route myRoute;
        public MapRoute myMapRoute;

        public Maping(Map map, Canvas canvas)
        {
            Settings = IsolatedStorageSettings.ApplicationSettings;
            this.myMap = map;
            this.mapLayer = new MapLayer();
            myCoordinates = new List<GeoCoordinate>();
            this.canvas = canvas;
            this.newInstance = true;
        }
        
        public void DrawMapLocation()
        {
            if (this.myMap.Layers.Count > 0)
            {
                this.myMap.Layers.Clear();
                this.mapLayer.Clear();
            }
            DrawMapMarker(myCoordinate, Colors.Red);
            this.myMap.Layers.Add(this.mapLayer);
            myCoordinates = new List<GeoCoordinate>();
        }

        public void searchForTerm(String searchTerm, System.Windows.DependencyObject searchPage)
        {
            myGeocodeQuery = new GeocodeQuery();
            myGeocodeQuery.SearchTerm = searchTerm;
            myGeocodeQuery.GeoCoordinate = myCoordinate == null ? new GeoCoordinate(0, 0) : myCoordinate;
            myGeocodeQuery.QueryCompleted += (sender, e) => { myGeocodeQuery_QueryCompleted(sender, e, searchPage); };
            myGeocodeQuery.QueryAsync();
        }

        void myGeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e, System.Windows.DependencyObject searchPage)
        {
            try
            {
                if (e.Error == null)
                {
                    if (e.Result.Count > 0)
                    {
                        myCoordinates.Add(e.Result[0].GeoCoordinate);
                        this.pointGeoCoordinate = e.Result[0].GeoCoordinate;
                        this.myMap.SetView(myCoordinates[0], 13.5, MapAnimationKind.Parabolic);
                        //TheNewMap.fo//aqui me quede
                    }
                    else
                    {
                        MessageBox.Show("No se encontro ningun resultado", "Error", MessageBoxButton.OK);
                        Progress progress = new Progress();
                        progress.hideProgressIndicator(searchPage);
                    }

                    myGeocodeQuery.Dispose();
                }
                DrawMapMarkers();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ocurrio un error, vuelve a intentarlo", "Error", MessageBoxButton.OK);
            }
        }

        public void DrawMapMarkers()
        {
            MapOverlay userLocationOverlay = new MapOverlay();
            userLocationOverlay = this.mapLayer.ElementAt(0);
            this.mapLayer.Clear();
            this.mapLayer.Add(userLocationOverlay);
            for (int i = 0; i < myCoordinates.Count; i++)
            {
                DrawMapMarker(myCoordinates[i], Colors.Blue);
            }
        }

        

        public void DrawMapMarker(GeoCoordinate coordinate, Color color)
        {
            Ellipse Circhegraphic = new Ellipse();
            Circhegraphic.Fill = new SolidColorBrush(color);
            Circhegraphic.Height = 30;
            Circhegraphic.Width = 30;
            Circhegraphic.Tag = "false";
            if (color != Colors.Red)
                Circhegraphic.Hold += Circhegraphic_Hold;

            MapOverlay mapOverlay = new MapOverlay();
            mapOverlay.Content = Circhegraphic;
            mapOverlay.GeoCoordinate = new GeoCoordinate(coordinate.Latitude, coordinate.Longitude);
            mapOverlay.PositionOrigin = new Point(0.5, 0.5);
            if (color == Colors.Red)
                this.locationA = mapOverlay;
            this.mapLayer.Add(mapOverlay);
        }

        private void Circhegraphic_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Ellipse Circhegraphic = (Ellipse)sender;
            Circhegraphic.Fill = new SolidColorBrush(Colors.Purple);
            Circhegraphic.Height = 50;
            Circhegraphic.Width = 50;
            this.canvas.IsHitTestVisible = true;
            this.mapLayer.RemoveAt(this.mapLayer.Count -1);
            this.canvas.Children.Add(Circhegraphic);
            Point p = e.GetPosition(canvas);
            Canvas.SetLeft(Circhegraphic,p.X);
            Canvas.SetTop(Circhegraphic, p.Y);
            canvas.MouseMove += canvas_MouseMove;
            canvas.MouseLeftButtonUp += canvas_MouseLeftButtonUp;
            
        }

        void canvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.canvas.IsHitTestVisible)
            {
                Point p = e.GetPosition(canvas);
                this.canvas.IsHitTestVisible = false;
                this.pointGeoCoordinate = this.myMap.ConvertViewportPointToGeoCoordinate(p);
                DrawMapMarker(pointGeoCoordinate, Colors.Blue);
                this.canvas.Children.RemoveAt(0);
            }
        }

        void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(this.canvas.IsHitTestVisible)
            {
                Ellipse Circhegraphic = (Ellipse)this.canvas.Children[0];
                Point p = e.GetPosition(canvas);
                Canvas.SetLeft(Circhegraphic, p.X);
                Canvas.SetTop(Circhegraphic, p.Y);
            }
            
        }

        public void DrawMapMarkersAB()
        {
            MapOverlay userLocationOverlay = new MapOverlay();
            userLocationOverlay = this.mapLayer.ElementAt(0);
            this.mapLayer.Clear();
            this.mapLayer.Add(userLocationOverlay);
            if (locationA != null)
                this.mapLayer.Insert(1, locationA);
            if (locationB != null)
                this.mapLayer.Insert(2, locationB);

        }

        public void DrawRoute(List<GeoCoordinate> route)
        {
            myRouteQuery = new RouteQuery();
            myRouteQuery.TravelMode = TravelMode.Driving;
            myRouteQuery.Waypoints = route;
            myRouteQuery.QueryCompleted += myRouteQuery_QueryCompleted;
            myRouteQuery.QueryAsync();
        }

        void myRouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                myRoute = e.Result;
                myMapRoute = new MapRoute(myRoute);
                myMap.AddRoute(myMapRoute);
                myMap.SetView(myCoordinate, 16, MapAnimationKind.Parabolic);
                myRouteQuery.Dispose();
            }
            myMap.SetView(myRoute.Legs[0].BoundingBox, MapAnimationKind.Parabolic);
        }

    }
}
