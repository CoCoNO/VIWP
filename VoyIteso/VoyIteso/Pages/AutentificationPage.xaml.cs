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
using Windows.UI;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using VoyIteso.Resources;
using System.Threading;

namespace VoyIteso.Pages
{
    public partial class Autentification : PhoneApplicationPage
    {

        //ApiConnector apiConnector = new ApiConnector();
        //ApiConnector apiConnector = ApiConnector.instance;
        //User user;
        Progress progress;
        

        //private ProgressIndicator progressIndicator;

        public Autentification()
        {
            InitializeComponent();
            progress = new Progress();

        }

        #region commented
        /*
        #region clientVoyIteso_GetUserNameCompleted
        void clientVoyIteso_GetUserNameCompleted(object sender, ServiceReferenceVoyItesoMovil.GetUserNameCompletedEventArgs e)
        {
            String Info;
            //String name;
            //String gender;
            int index;

            Info = e.Result;
            index = Info.IndexOf(":");
            user.Name = Info.Substring(0, index);

            user.Gender = Info.Substring(index + 1);

            user.setInfo(user.key);
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }
        #endregion
        
        
        #region client_validarUsuarioCompleted
        private void client_validarUsuarioCompleted(object sender, ServiceReferenceAutentification.validarUsuarioCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                HideProgressIndicator();
                String tok;
                tok = e.Result;
                
                int foundChar = tok.IndexOf(":");
                if (foundChar >= 0)
                    tok = tok.Remove(0, foundChar + 2);

                foundChar = tok.IndexOf("\"");
                tok = tok.Remove(foundChar);

                if(tok!="")
                {
                    user.Token = tok;
                    user.setInfo(user.key);
                    if (user.Type == null)
                        NavigationService.Navigate(new Uri("/Pages/TypePAge.xaml", UriKind.Relative));
                    else
                    {
                        if(user.Name == null)
                        {
                            //clientVoyIteso.GetUserNameAsync(user.Token);
                            
                        }
                        else      
                            NavigationService.Navigate(new Uri("/Pages/Home.xaml", UriKind.Relative));
                    }
                    
                    
                }
                
                else
                    MessageBox.Show("Tu Usuario y/o Constraseña son incorrectos", "Error", MessageBoxButton.OK);
            }         

        }
        #endregion
        */
        #endregion

        #region btnSend_Click
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {

            SendRequest();

        }
        #endregion 

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //HideProgressIndicator();
            progress.hideProgressIndicator(this);
            base.OnNavigatedTo(e);
            NavigationService.RemoveBackEntry();
            Microsoft.Phone.Shell.SystemTray.ForegroundColor = System.Windows.Media.Color.FromArgb(255, 110, 207, 243);
#if DEBUG
            txbUser.Text = "ie800001";
            txbPass.Password = "PruebaQA2015";
#endif
        }
        #endregion
        
        #region Focus User/Pass
        private void txbUser_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            color.A = 255;
            color.R = 16;
            color.G = 69;
            color.B = 114;

            txbUser.Background = new SolidColorBrush(color);
            txbUser.BorderBrush = new SolidColorBrush(color);

        }

        private void txbPass_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            color.A = 255;
            color.R = 16;
            color.G = 69;
            color.B = 114;

            txbPass.Background = new SolidColorBrush(color);
            txbPass.BorderBrush = new SolidColorBrush(color);
        }
        #endregion

        #region User & Pass KeyDown
        private void txbPass_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txbUser.Text == string.Empty)
                {

                    txbUser.Focus();
                }
                else
                {
                    if (txbPass.Password != string.Empty)
                    {

                        SendRequest();
                    }

                }
            }
                
        }

        private void txbUser_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                /*if (txbUser.Text != string.Empty && txbPass.Password != string.Empty)
                {
                    //ShowProgressIndicator();
                    //progress.showProgressIndicator(this, "Autentificando");
                    //clientAutentification.validarUsuarioAsync(txbUser.Text, txbPass.Password);
                    SendRequest();
                }
                else*/
                    txbPass.Focus();
            }
        }
        #endregion

        #region SendRequest
        private async Task SendRequest()
        {



            progress.showProgressIndicator(this, "Autentificando");
            try
            {
                await ApiConnector.Instance.LogIn(txbUser.Text, txbPass.Password);
                progress.hideProgressIndicator(this);
                NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
            }
            catch (TimeoutException)
            {
                progress.hideProgressIndicator(this);
                MessageBox.Show("No hay conexión a internet");
            }
            catch (VoyIteso.Class.ApiConnector.BadLoginExeption)
            {
                progress.hideProgressIndicator(this);
                MessageBox.Show("Verifica credenciales e intenta de nuevo");
            }
            

        }
        #endregion

        #region HyperlinkButton_Click
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.Uri = new Uri("https://datospersonales.iteso.mx/");
            wbt.Show();
        }
        #endregion
        
        #region uSELES
        /*
        void apiConnector_exceptionChanged(object sender, EventArgs e)
        {
            /*if (apiConnector.throwException != null || apiConnector.throwException != "")
            {
                Dispatcher.BeginInvoke(() => {
                    MessageBox.Show(apiConnector.throwException, "Error", MessageBoxButton.OK);
                    apiConnector.exceptionChanged -= apiConnector_exceptionChanged;
                    apiConnector.throwException = null;
                    //HideProgressIndicator();
                    progress.hideProgressIndicator(this);
                });
                
            }
        }
        #endregion

        #region apiConnector_responseChanged
        void apiConnector_responseChanged(object sender, EventArgs e)
        {
            //user = apiConnector.getUserFromJson();
            if(user != null)
            {
                user.setInfo(user.key);
                Dispatcher.BeginInvoke(() =>
                {
                    if (user.Type == null)
                        NavigationService.Navigate(new Uri("/Pages/TypePage.xaml", UriKind.Relative));
                    else
                        NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Tu Usuario y/o Constraseña son incorrectos", "Error", MessageBoxButton.OK);
                    //HideProgressIndicator();
                    progress.hideProgressIndicator(this);
                });
            }
            //apiConnector.responseChanged -= apiConnector_responseChanged;
            /*
            if (apiConnector.CheckForStatus())
            {
                user = apiConnector.SetUserJson();
                user.setInfo(user.key);
                Dispatcher.BeginInvoke(() =>
                {
                    if (user.Type == null)
                        NavigationService.Navigate(new Uri("/Pages/TypePage.xaml", UriKind.Relative));
                    else
                        NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                });
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Tu Usuario y/o Constraseña son incorrectos", "Error", MessageBoxButton.OK);
                    //HideProgressIndicator();
                    progress.hideProgressIndicator(this);
                });

            }

            apiConnector.responseChanged -= apiConnector_responseChanged;*/
        //}
        /*
        #region show and hide progressIndicator
        private void ShowProgressIndicator()
        {
            progressIndicator.Text = "Autentificando";
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;
            SystemTray.SetProgressIndicator(this,progressIndicator);
        }

        private void HideProgressIndicator()
        {
            progressIndicator.IsVisible = false;
            SystemTray.SetProgressIndicator(this,progressIndicator);
        }
        #endregion
        */
        #endregion

        

        
        
    }
}