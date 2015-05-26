﻿#pragma checksum "C:\Users\Usuario\Documents\GitHub\VIWP\VoyIteso\VoyIteso\Pages\SearchRoutePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CF9FD74866FAA23E51E65B28BA19E66E"
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
    
    
    public partial class SearchRoutePage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Media.Animation.Storyboard ShowLeftPanelAnimation;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame LeftKeyFrameShowBegin;
        
        internal System.Windows.Media.Animation.Storyboard HideLeftPanelAnimation;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame LeftKeyFrameHideEnd;
        
        internal System.Windows.Media.Animation.Storyboard ShowRightPanelAnimation;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame RightKeyFrameShowBegin;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame RightKeyFrameShowEnd;
        
        internal System.Windows.Media.Animation.Storyboard HideRightPanelAnimation;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame RightKeyFrameHideBegin;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame RightKeyFrameHideEnd;
        
        internal System.Windows.Media.Animation.Storyboard ShowRightPanelFromLeftAnimation;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame RightKeyFrameShowFromLeftEnd;
        
        internal System.Windows.Media.Animation.Storyboard ShowLeftPanelFromRightAnimation;
        
        internal System.Windows.Media.Animation.EasingDoubleKeyFrame LeftKeyFrameShowFromRightBegin;
        
        internal System.Windows.Controls.Canvas CanvasRoot;
        
        internal System.Windows.Media.CompositeTransform CanvasRootTransform;
        
        internal System.Windows.Controls.Grid RootGrid;
        
        internal System.Windows.Controls.Grid LeftPanelGrid;
        
        internal System.Windows.Controls.Grid TopMarginGrid;
        
        internal System.Windows.Controls.Grid OriginSetGrid;
        
        internal System.Windows.Controls.TextBlock lblOrigin;
        
        internal System.Windows.Controls.TextBlock txtOrigin;
        
        internal System.Windows.Controls.Grid DestinySetGrid;
        
        internal System.Windows.Controls.TextBlock lblDestiny;
        
        internal System.Windows.Controls.TextBlock txtDestiny;
        
        internal Microsoft.Phone.Controls.DatePicker datePicker;
        
        internal Microsoft.Phone.Controls.TimePicker timePicker;
        
        internal System.Windows.Controls.Grid SmokerPickerGrid;
        
        internal Microsoft.Phone.Controls.ListPicker pikerSmoke;
        
        internal System.Windows.Controls.Grid GenderPickerGrid;
        
        internal Microsoft.Phone.Controls.ListPicker pikerGender;
        
        internal System.Windows.Controls.Grid SearchImageGrid;
        
        internal System.Windows.Controls.Grid CenterPanelGrid;
        
        internal System.Windows.Controls.Grid MapGrid;
        
        internal Microsoft.Phone.Maps.Controls.Map myMap;
        
        internal System.Windows.Controls.Canvas InvisibleCanvas;
        
        internal System.Windows.Controls.Grid SearchTermGrid;
        
        internal System.Windows.Controls.TextBlock lblSearchTerm;
        
        internal Microsoft.Phone.Controls.AutoCompleteBox searchTermBox;
        
        internal System.Windows.Controls.Grid RightPanelGrid;
        
        internal System.Windows.Controls.ListBox ResultsListBox;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/VoyIteso;component/Pages/SearchRoutePage.xaml", System.UriKind.Relative));
            this.ShowLeftPanelAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ShowLeftPanelAnimation")));
            this.LeftKeyFrameShowBegin = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("LeftKeyFrameShowBegin")));
            this.HideLeftPanelAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("HideLeftPanelAnimation")));
            this.LeftKeyFrameHideEnd = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("LeftKeyFrameHideEnd")));
            this.ShowRightPanelAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ShowRightPanelAnimation")));
            this.RightKeyFrameShowBegin = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("RightKeyFrameShowBegin")));
            this.RightKeyFrameShowEnd = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("RightKeyFrameShowEnd")));
            this.HideRightPanelAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("HideRightPanelAnimation")));
            this.RightKeyFrameHideBegin = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("RightKeyFrameHideBegin")));
            this.RightKeyFrameHideEnd = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("RightKeyFrameHideEnd")));
            this.ShowRightPanelFromLeftAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ShowRightPanelFromLeftAnimation")));
            this.RightKeyFrameShowFromLeftEnd = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("RightKeyFrameShowFromLeftEnd")));
            this.ShowLeftPanelFromRightAnimation = ((System.Windows.Media.Animation.Storyboard)(this.FindName("ShowLeftPanelFromRightAnimation")));
            this.LeftKeyFrameShowFromRightBegin = ((System.Windows.Media.Animation.EasingDoubleKeyFrame)(this.FindName("LeftKeyFrameShowFromRightBegin")));
            this.CanvasRoot = ((System.Windows.Controls.Canvas)(this.FindName("CanvasRoot")));
            this.CanvasRootTransform = ((System.Windows.Media.CompositeTransform)(this.FindName("CanvasRootTransform")));
            this.RootGrid = ((System.Windows.Controls.Grid)(this.FindName("RootGrid")));
            this.LeftPanelGrid = ((System.Windows.Controls.Grid)(this.FindName("LeftPanelGrid")));
            this.TopMarginGrid = ((System.Windows.Controls.Grid)(this.FindName("TopMarginGrid")));
            this.OriginSetGrid = ((System.Windows.Controls.Grid)(this.FindName("OriginSetGrid")));
            this.lblOrigin = ((System.Windows.Controls.TextBlock)(this.FindName("lblOrigin")));
            this.txtOrigin = ((System.Windows.Controls.TextBlock)(this.FindName("txtOrigin")));
            this.DestinySetGrid = ((System.Windows.Controls.Grid)(this.FindName("DestinySetGrid")));
            this.lblDestiny = ((System.Windows.Controls.TextBlock)(this.FindName("lblDestiny")));
            this.txtDestiny = ((System.Windows.Controls.TextBlock)(this.FindName("txtDestiny")));
            this.datePicker = ((Microsoft.Phone.Controls.DatePicker)(this.FindName("datePicker")));
            this.timePicker = ((Microsoft.Phone.Controls.TimePicker)(this.FindName("timePicker")));
            this.SmokerPickerGrid = ((System.Windows.Controls.Grid)(this.FindName("SmokerPickerGrid")));
            this.pikerSmoke = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("pikerSmoke")));
            this.GenderPickerGrid = ((System.Windows.Controls.Grid)(this.FindName("GenderPickerGrid")));
            this.pikerGender = ((Microsoft.Phone.Controls.ListPicker)(this.FindName("pikerGender")));
            this.SearchImageGrid = ((System.Windows.Controls.Grid)(this.FindName("SearchImageGrid")));
            this.CenterPanelGrid = ((System.Windows.Controls.Grid)(this.FindName("CenterPanelGrid")));
            this.MapGrid = ((System.Windows.Controls.Grid)(this.FindName("MapGrid")));
            this.myMap = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("myMap")));
            this.InvisibleCanvas = ((System.Windows.Controls.Canvas)(this.FindName("InvisibleCanvas")));
            this.SearchTermGrid = ((System.Windows.Controls.Grid)(this.FindName("SearchTermGrid")));
            this.lblSearchTerm = ((System.Windows.Controls.TextBlock)(this.FindName("lblSearchTerm")));
            this.searchTermBox = ((Microsoft.Phone.Controls.AutoCompleteBox)(this.FindName("searchTermBox")));
            this.RightPanelGrid = ((System.Windows.Controls.Grid)(this.FindName("RightPanelGrid")));
            this.ResultsListBox = ((System.Windows.Controls.ListBox)(this.FindName("ResultsListBox")));
        }
    }
}

