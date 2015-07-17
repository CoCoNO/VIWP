using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VoyIteso.Class;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        //fields
        User user;
        private bool editionEnabled;
        private bool music;
        private bool smoke;
        private bool airconditioner;
        private bool talk;
        public PhotoChooserTask photoChooserTask;

        //properties
        public byte[] photoStream { get; set; }

        //constructor of class
        public ProfilePage()
        {
            InitializeComponent();
            user = ApiConnector.instance.ActiveUser;
            editionEnabled = false;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
        }

        //methods
        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (editionEnabled)
            {
                if (e.TaskResult == TaskResult.OK)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(e.ChosenPhoto);
                        string photoName = e.OriginalFileName;
                        userImage.Source = bmp;
                        using (var ms = new MemoryStream())
                        {
                            WriteableBitmap btmMap = new WriteableBitmap(bmp);
                            Extensions.SaveJpeg(btmMap, ms, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                            photoStream = ms.ToArray();
                        }
                        //isPhotoChanged = true;//esa bandera la usa emmanuel.
                    });

                }
            }
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

                music = (user.profile.musica == 1);
                smoke = (user.profile.fuma == 1);
                airconditioner = (user.profile.aire == 1);
                talk = (user.profile.platicar == 1);

                if (!music)
                {
                    musicImage.Opacity = 30;
                }
                if (!smoke)
                {
                    smokeImage.Opacity = 30;
                }
                if (!airconditioner)
                {
                    acImage.Opacity = 30;
                }
                if (!talk)
                {
                    talkImage.Opacity = 30;
                }

                givenLiftCounttxt.Text = user.profile.aventones_dados_count.ToString();
                takenLiftCounttxt.Text = user.profile.aventones_recibidos_count.ToString();
                routCounttxt.Text = user.profile.rutas_count.ToString();
                descriptiontxt.Text = user.profile.descripcion.ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("At: \n" + ex.Message, "Error:", MessageBoxButton.OK);
                MessageBox.Show("At: \n" + "algo valio madres y es esto-> \n" + ex.Message, "Error:", MessageBoxButton.OK);
            }
           
        }

        #endregion

        //este metodo es el boton que habilita la edicion o guarda los cambios 
        private void ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            //toggle edition mode
            editionEnabled = !editionEnabled;
            if (!editionEnabled)
            {
                user.profile.descripcion = descriptiontxt.Text;
                descriptiontxt.IsEnabled = false;
                //guardar las modificaciones con el metodo de Jairo. aqui puto
            }
            else
            {
                descriptiontxt.IsEnabled = true;
            }
        }


#region Tap_Events

        private void Music_onTap(object sender, GestureEventArgs e)
        {
            if (editionEnabled)
            
            {
                //musicImage.Opacity = user.profile.musica == 1 ? 30 : 100;
                if (user.profile.musica == 1)
                {
                    musicImage.Opacity = 30;
                    user.profile.musica = 0;//toggle value
                }
                else
                {
                    musicImage.Opacity = 100; 
                    user.profile.musica = 1;//toggle value
                }
            }
        }

        private void Fuma_OnTap(object sender, GestureEventArgs e)
        {
            if (editionEnabled)
            {
                //smokeImage.Opacity = user.profile.fuma == 1 ? 30 : 100;
                if (user.profile.fuma == 1)
                {
                    smokeImage.Opacity = 30;
                    user.profile.fuma = 0;
                }
                else
                {
                    smokeImage.Opacity = 100;
                    user.profile.fuma = 1;
                }
            }
        }

        private void AirConditioner_OnTap(object sender, GestureEventArgs e)
        {
            if (editionEnabled)
            {
                //acImage.Opacity = user.profile.aire == 1 ? 30 : 100;
                if (user.profile.aire == 1)
                {
                    acImage.Opacity = 30;
                    user.profile.aire = 0;
                }
                else
                {
                    acImage.Opacity = 100;
                    user.profile.aire = 1;
                }
            }
        }

        private void Talk_OnTap(object sender, GestureEventArgs e)
        {
            if (editionEnabled)
            {
                //talkImage.Opacity = user.profile.platicar == 1 ? 30 : 100;
                if (user.profile.platicar == 1)
                {
                    talkImage.Opacity = 30;
                    user.profile.platicar = 1;
                }
                else
                {
                    talkImage.Opacity = 100;
                    user.profile.platicar = 0;
                }
            }
        }

#endregion

        private void UserImage_OnTap(object sender, GestureEventArgs e)
        {
            photoChooserTask.Show();
        }

        
    }
}