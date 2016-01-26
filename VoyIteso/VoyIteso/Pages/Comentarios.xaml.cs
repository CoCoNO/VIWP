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

namespace VoyIteso.Pages
{
    public partial class Comentarios : PhoneApplicationPage
    {
        public Comentarios()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await ApiConnector.Instance.CommentSend(DetermineType(), TextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Hubo un error, intenta de nuevo.");
            } 
        }

        string DetermineType()
        {
            var a = "";
            switch (ComboBox.SelectedIndex)
            {
                case 0:
                    a = "Queja";
                    break;
                case 1:
                    a = "Sugerencia";
                    break;
                case 2:
                    a = "";
                    break;

                default:
                    a = "No especificado";
                    break;
            }

            return a;
        }
    }
}