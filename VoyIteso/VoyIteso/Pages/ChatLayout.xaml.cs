﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Pages.Chat2;

namespace VoyIteso.Pages
{
    public partial class ChatLayout : PhoneApplicationPage
    {
        public ChatLayout()
        {
            InitializeComponent();
            ContentPanel.Children.Add(new ChatView());
        }
        
        
    }
}