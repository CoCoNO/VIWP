﻿#pragma checksum "C:\Users\SergioAdán\Documents\Github\VIWP\VoyIteso\VoyIteso\Pages\ProfilePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2B546C59E0AE5021E6419F117B635E3E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
    
    
    public partial class ProfilePage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Image userImage;
        
        internal System.Windows.Controls.TextBlock userName;
        
        internal System.Windows.Controls.TextBlock userAge;
        
        internal System.Windows.Controls.TextBlock userMajor;
        
        internal Telerik.Windows.Controls.RadRating ratingStars;
        
        internal System.Windows.Controls.TextBlock txtEvaluations;
        
        internal System.Windows.Media.ImageBrush UserPropertiesBackground;
        
        internal System.Windows.Controls.Image musicImage;
        
        internal System.Windows.Controls.Image smokeImage;
        
        internal System.Windows.Controls.Image acImage;
        
        internal System.Windows.Controls.Image talkImage;
        
        internal System.Windows.Controls.TextBlock givenLiftCounttxt;
        
        internal System.Windows.Controls.TextBlock takenLiftCounttxt;
        
        internal System.Windows.Controls.TextBlock routCounttxt;
        
        internal System.Windows.Controls.Grid DescriptionGrid;
        
        internal System.Windows.Controls.TextBox descriptiontxt;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton AppBarEditButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/VoyIteso;component/Pages/ProfilePage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.userImage = ((System.Windows.Controls.Image)(this.FindName("userImage")));
            this.userName = ((System.Windows.Controls.TextBlock)(this.FindName("userName")));
            this.userAge = ((System.Windows.Controls.TextBlock)(this.FindName("userAge")));
            this.userMajor = ((System.Windows.Controls.TextBlock)(this.FindName("userMajor")));
            this.ratingStars = ((Telerik.Windows.Controls.RadRating)(this.FindName("ratingStars")));
            this.txtEvaluations = ((System.Windows.Controls.TextBlock)(this.FindName("txtEvaluations")));
            this.UserPropertiesBackground = ((System.Windows.Media.ImageBrush)(this.FindName("UserPropertiesBackground")));
            this.musicImage = ((System.Windows.Controls.Image)(this.FindName("musicImage")));
            this.smokeImage = ((System.Windows.Controls.Image)(this.FindName("smokeImage")));
            this.acImage = ((System.Windows.Controls.Image)(this.FindName("acImage")));
            this.talkImage = ((System.Windows.Controls.Image)(this.FindName("talkImage")));
            this.givenLiftCounttxt = ((System.Windows.Controls.TextBlock)(this.FindName("givenLiftCounttxt")));
            this.takenLiftCounttxt = ((System.Windows.Controls.TextBlock)(this.FindName("takenLiftCounttxt")));
            this.routCounttxt = ((System.Windows.Controls.TextBlock)(this.FindName("routCounttxt")));
            this.DescriptionGrid = ((System.Windows.Controls.Grid)(this.FindName("DescriptionGrid")));
            this.descriptiontxt = ((System.Windows.Controls.TextBox)(this.FindName("descriptiontxt")));
            this.AppBarEditButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("AppBarEditButton")));
        }
    }
}

