﻿#pragma checksum "C:\Users\Usuario\Documents\GitHub\VIWP\VoyIteso\VoyIteso\Pages\HomePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C979DDCF7BFFC1ED4CCA56A69D4D4AEF"
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
using Telerik.Windows.Controls;


namespace VoyIteso.Pages {
    
    
    public partial class HomePage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid Root;
        
        internal System.Windows.Controls.TextBlock txtUserName;
        
        internal Telerik.Windows.Controls.RadCycleHubTile notificationsTile;
        
        internal Telerik.Windows.Controls.RadCustomHubTile calendarTile;
        
        internal System.Windows.Controls.TextBlock txtDayString;
        
        internal System.Windows.Controls.TextBlock txtDayNumber;
        
        internal Telerik.Windows.Controls.RadSlideHubTile profileTile;
        
        internal System.Windows.Controls.Image profileImage;
        
        internal Telerik.Windows.Controls.RadSlideHubTile searchOfferMapTile;
        
        internal System.Windows.Controls.Image TestImage;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/VoyIteso;component/Pages/HomePage.xaml", System.UriKind.Relative));
            this.Root = ((System.Windows.Controls.Grid)(this.FindName("Root")));
            this.txtUserName = ((System.Windows.Controls.TextBlock)(this.FindName("txtUserName")));
            this.notificationsTile = ((Telerik.Windows.Controls.RadCycleHubTile)(this.FindName("notificationsTile")));
            this.calendarTile = ((Telerik.Windows.Controls.RadCustomHubTile)(this.FindName("calendarTile")));
            this.txtDayString = ((System.Windows.Controls.TextBlock)(this.FindName("txtDayString")));
            this.txtDayNumber = ((System.Windows.Controls.TextBlock)(this.FindName("txtDayNumber")));
            this.profileTile = ((Telerik.Windows.Controls.RadSlideHubTile)(this.FindName("profileTile")));
            this.profileImage = ((System.Windows.Controls.Image)(this.FindName("profileImage")));
            this.searchOfferMapTile = ((Telerik.Windows.Controls.RadSlideHubTile)(this.FindName("searchOfferMapTile")));
            this.TestImage = ((System.Windows.Controls.Image)(this.FindName("TestImage")));
        }
    }
}

