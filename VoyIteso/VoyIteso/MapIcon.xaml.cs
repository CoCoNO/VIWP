using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace VoyIteso
{
    public partial class MapIcon : UserControl
    {
        /// <summary>
        /// name of street or av
        /// </summary>
        public string StreetName { get; set; }
        /// <summary>
        /// street number
        /// </summary>
        public string StreetNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Colony { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Reference { get; set; }


        /// <summary>
        /// Constructor of class
        /// </summary>
        public MapIcon()
        {
            InitializeComponent();
        }
    }
}