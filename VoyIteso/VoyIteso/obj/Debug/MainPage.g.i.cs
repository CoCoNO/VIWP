﻿#pragma checksum "C:\Users\Usuario\Documents\GitHub\VIWP\VoyIteso\VoyIteso\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F28341F05A1F9EF5C3CF6C317F11FCB2"
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


namespace VoyIteso {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid Root;
        
        internal System.Windows.Controls.TextBlock versionControlSplash;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/VoyIteso;component/MainPage.xaml", System.UriKind.Relative));
            this.Root = ((System.Windows.Controls.Grid)(this.FindName("Root")));
            this.versionControlSplash = ((System.Windows.Controls.TextBlock)(this.FindName("versionControlSplash")));
        }
    }
}

