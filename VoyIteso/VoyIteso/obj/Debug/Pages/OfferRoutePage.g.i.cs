﻿#pragma checksum "C:\Users\Usuario\Documents\Visual Studio 2013\Projects\Nueva carpeta\VoyIteso\VoyIteso\Pages\OfferRoutePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "20531F98EAEF2338089079F5C0FB8FB0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace VoyIteso.Pages {
    
    
    public partial class OfferRoutePage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Media.Animation.Storyboard SearchPanelShow;
        
        internal System.Windows.Media.Animation.Storyboard MapBackFromLeft;
        
        internal System.Windows.Media.Animation.Storyboard ResultPanelShow;
        
        internal System.Windows.Media.Animation.Storyboard MapBackFromRight;
        
        internal System.Windows.Controls.StackPanel Root;
        
        internal System.Windows.Controls.StackPanel LeftPanel;
        
        internal System.Windows.Controls.StackPanel OriginPanel;
        
        internal System.Windows.Controls.TextBlock lblOrigin;
        
        internal System.Windows.Controls.TextBlock txtOrigin;
        
        internal System.Windows.Controls.StackPanel DestinyPanel;
        
        internal System.Windows.Controls.TextBlock lblDestiny;
        
        internal System.Windows.Controls.TextBlock txtDestiny;
        
        internal System.Windows.Controls.StackPanel DatePanel;
        
        internal Microsoft.Phone.Controls.DatePicker datePicker;
        
        internal System.Windows.Controls.StackPanel TimePanel;
        
        internal Microsoft.Phone.Controls.TimePicker timePicker;
        
        internal System.Windows.Controls.StackPanel Travel;
        
        internal System.Windows.Controls.RadioButton btnOneWayTrip;
        
        internal System.Windows.Controls.RadioButton btnTwoWayTrip;
        
        internal System.Windows.Controls.StackPanel SeatsLeft;
        
        internal Microsoft.Phone.Controls.ListPicker pikerSeats;
        
        internal System.Windows.Controls.StackPanel RightPanel;
        
        internal System.Windows.Controls.ListBox list;
        
        internal System.Windows.Controls.Grid MapPanel;
        
        internal Microsoft.Phone.Maps.Controls.Map myMap;
        
        internal System.Windows.Controls.StackPanel SearchTermPanel;
        
        internal System.Windows.Controls.TextBlock lblSearchTerm;
        
        internal System.Windows.Controls.TextBox searchTermBox;
        
        internal System.Windows.Controls.Grid ImagePanel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/VoyIteso;component/Pages/OfferRoutePage.xaml", System.UriKind.Relative));
            this.SearchPanelShow = ((System.Windows.Media.Animation.Storyboard)(this.FindName("SearchPanelShow")));
            this.MapBackFromLeft = ((System.Windows.Media.Animation.Storyboard)(this.FindName("MapBackFromLeft")));
            this.ResultPanelShow = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ResultPanelShow")));
            this.MapBackFromRight = ((System.Windows.Media.Animation.Storyboard)(this.FindName("MapBackFromRight")));
            this.Root = ((System.Windows.Controls.StackPanel)(this.FindName("Root")));
            this.LeftPanel = ((System.Windows.Controls.StackPanel)(this.FindName("LeftPanel")));
            this.OriginPanel = ((System.Windows.Controls.StackPanel)(this.FindName("OriginPanel")));
            this.lblOrigin = ((System.Windows.Controls.TextBlock)(this.FindName("lblOrigin")));
            this.txtOrigin = ((System.Windows.Controls.TextBlock)(this.FindName("txtOrigin")));
            this.DestinyPanel = ((System.Windows.Controls.StackPanel)(this.FindName("DestinyPanel")));
            this.lblDestiny = ((System.Windows.Controls.TextBlock)(this.FindName("lblDestiny")));
            this.txtDestiny = ((System.Windows.Controls.TextBlock)(this.FindName("txtDestiny")));
            this.DatePanel = ((System.Windows.Controls.StackPanel)(this.FindName("DatePanel")));
            this.datePicker = ((Microsoft.Phone.Controls.DatePicker)(this.FindName("datePicker")));
            this.TimePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TimePanel")));
            this.timePicker = ((Microsoft.Phone.Controls.TimePicker)(this.FindName("timePicker")));
            this.Travel = ((System.Windows.Controls.StackPanel)(this.FindName("Travel")));
            this.btnOneWayTrip = ((System.Windows.Controls.RadioButton)(this.FindName("btnOneWayTrip")));
            this.btnTwoWayTrip = ((System.Windows.Controls.RadioButton)(this.FindName("btnTwoWayTrip")));
            this.SeatsLeft = ((System.Windows.Controls.StackPanel)(this.FindName("SeatsLeft")));
            this.pikerSeats = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("pikerSeats")));
            this.RightPanel = ((System.Windows.Controls.StackPanel)(this.FindName("RightPanel")));
            this.list = ((System.Windows.Controls.ListBox)(this.FindName("list")));
            this.MapPanel = ((System.Windows.Controls.Grid)(this.FindName("MapPanel")));
            this.myMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("myMap")));
            this.SearchTermPanel = ((System.Windows.Controls.StackPanel)(this.FindName("SearchTermPanel")));
            this.lblSearchTerm = ((System.Windows.Controls.TextBlock)(this.FindName("lblSearchTerm")));
            this.searchTermBox = ((System.Windows.Controls.TextBox)(this.FindName("searchTermBox")));
            this.ImagePanel = ((System.Windows.Controls.Grid)(this.FindName("ImagePanel")));
        }
    }
}

