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
using System.Windows.Media.Imaging;

namespace VoyIteso.Pages
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        User user;

        public ProfilePage()
        {
            InitializeComponent();
            user = ApiConnector.instance.ActiveUser;
        }


        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            try
            {
                userImage.Source = user.Avatar;
                userName.Text = user.profile.nombre;
                userAge.Text = user.profile.edad;
                userMajor.Text = user.profile.carrera;

                //userImage.Source = (new BitmapImage(new Uri("/Images/User.jpg", UriKind.Absolute)));

                //if (user.profile.musica != 1)
                //{
                //    musicImage.Opacity = 30;
                //}
                //if (user.profile.fuma != 1)
                //{
                //    smokeImage.Opacity = 30;
                //}
                //if (user.profile.aire != 1)
                //{
                //    acImage.Opacity = 30;
                //}
                //if (user.profile.platicar != 1)
                //{
                //    talkImage.Opacity = 30;
                //}

                givenLiftCounttxt.Text = user.profile.aventones_dados_count.ToString();
                takenLiftCounttxt.Text = user.profile.aventones_recibidos_count.ToString();
                routCounttxt.Text = user.profile.rutas_count.ToString();
                //descriptiontxt.Text = user.profile.
            }
            catch (Exception ex)
            {   
                MessageBox.Show("At: \n" + ex.StackTrace.ToString(), "Error encountered", MessageBoxButton.OK);
            }
           
        }

        #endregion

        
    }
}