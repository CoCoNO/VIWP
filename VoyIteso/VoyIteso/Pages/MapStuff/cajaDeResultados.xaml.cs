using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Class;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages.MapStuff
{
    public partial class cajaDeResultados : UserControl
    {
        public int routeID ;
        //string perfil_id;
        public string perfil_id { get; set; }
        public string aventon_id { get; set; }
        public string texto_origen { get; set; }
        public string texto_destino { get; set; }
        public bool myBool = false;

        public cajaDeResultados()
        {
            InitializeComponent();
        }

        
    }
}
