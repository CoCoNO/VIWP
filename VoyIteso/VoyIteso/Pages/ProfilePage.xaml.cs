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
using Windows.Storage;
using Microsoft.Phone.Tasks;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VoyIteso.Pages
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        //fields
        //User user;
        private bool _editionEnabled;

        private bool EditionEnabled
        {
            get { return _editionEnabled; }
            set
            {
                _editionEnabled = value;
                editionChanged();
                
                
            }
        }

        private bool saveNewData=true;
        private bool music;
        private bool smoke;
        private bool airconditioner;
        private bool talk;
        public PhotoChooserTask photoChooserTask;
        private bool imageChanged;
        private byte[] newImage;
        private string newImagePath;
        Progress progress = new Progress();

        //properties
        public byte[] photoStream { get; set; }

        //constructor of class
        public ProfilePage()
        {
            InitializeComponent();
            //user = ApiConnector.Instance.ActiveUser;
            _editionEnabled = false;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            userImage.Tap += BtnChooseImage_Click;
            UserDataChanged();
            queryUserData();
        }

        //methods
        private void editionChanged()
        {
            if (!_editionEnabled)
            {
                SystemTray.BackgroundColor = Color.FromArgb(255, 0, 0, 0);
                ApiConnector.Instance.ActiveUser.profile.descripcion = descriptiontxt.Text;
                descriptiontxt.IsEnabled = false;
                //guardar las modificaciones con el metodo de Jairo. aqui puto

                UpdateUserData();

                //((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "modificar";

                ApplicationBar = new ApplicationBar();
                ApplicationBar.Mode = ApplicationBarMode.Default;
                ApplicationBar.Opacity = 1.0;
                ApplicationBar.IsMenuEnabled = true;
                ApplicationBar.IsVisible = true;

                ApplicationBarIconButton a = new ApplicationBarIconButton(new Uri("Images/icons/edit.png", UriKind.Relative));
                a.Text = "Modificar";
                a.Click += ApplicationBarIconButton_OnClick;
                ApplicationBar.Buttons.Add(a);

            }
            else
            {
                SystemTray.BackgroundColor = Color.FromArgb(255, 0, 255, 0);
                descriptiontxt.IsEnabled = true;
               // ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = "Guardar";


                ApplicationBar = new ApplicationBar();
                ApplicationBar.Mode = ApplicationBarMode.Default;
                ApplicationBar.Opacity = 1.0;
                ApplicationBar.IsMenuEnabled = true;
                ApplicationBar.IsVisible = true;

                ApplicationBarIconButton guardar = new ApplicationBarIconButton(new Uri("Images/icons/save.png", UriKind.Relative));
                guardar.Text = "Guardar";
                guardar.Click += ApplicationBarIconButton_OnClick;
                ApplicationBar.Buttons.Add(guardar);


            }
        }

        BitmapImage ObjBmpImage = new BitmapImage();
        private void BtnChooseImage_Click(object sender, RoutedEventArgs e)
        {
            if (_editionEnabled)
            {
                PhotoChooserTask photoChooserTask;
                photoChooserTask = new PhotoChooserTask();
                photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
                photoChooserTask.ShowCamera = true;
                photoChooserTask.Show();
            }
            
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Code to display the photo on the page in an image control named myImage. 
                //isImageUpload = true;
                imageChanged = true;
                ObjBmpImage.SetSource(e.ChosenPhoto);
                userImage.Source = ObjBmpImage;
                var b = ImageToArray(ObjBmpImage);
                ApiConnector.Instance.ActiveUser.Avatar = ObjBmpImage;
                newImage = b;
                newImagePath = e.OriginalFileName;
                //ApiConnector.Instance.UploadImage(b,e.OriginalFileName);
            }
        }
        public byte[] ImageToArray(BitmapImage image)
        {
            WriteableBitmap wbmp = new WriteableBitmap(image);
            MemoryStream ms = new MemoryStream();

            wbmp.SaveJpeg(ms, wbmp.PixelWidth, wbmp.PixelHeight, 0, 100);
            return ms.ToArray();

        } 





        /*private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (editionEnabled)
            {
                if (e.TaskResult == TaskResult.OK)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        /*BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(e.ChosenPhoto);
                        string photoName = e.OriginalFileName;
                        userImage.Source = bmp;
                         * 
                         * 
                         * 
                         


                        // initialize the result photo stream
                        var photoStream = new MemoryStream();
                        // Save the stream result (copying the resulting stream)
                        e.ChosenPhoto.CopyTo(photoStream);
                        // save the original file name
                        var fileName = e.OriginalFileName;


                        e.
                        ApiConnector.Instance.UploadImage(e.ChosenPhoto, e.OriginalFileName);
                            //e.OriginalFileName.Substring(.LastIndexOf("\\") + 1));
                        //e.ChosenPhoto.ReadAsync()
                        /*using (var ms = new MemoryStream())
                        {
                            WriteableBitmap btmMap = new WriteableBitmap(bmp);
                            Extensions.SaveJpeg(btmMap, ms, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                            photoStream = ms.ToArray();
                            
                        }
                        //isPhotoChanged = true;//esa bandera la usa emmanuel.
                    });

                }
            }
        }*/

        private void UserDataChanged()
        {
            try
            {
                if (ApiConnector.Instance.ActiveUser.Avatar!=null)
                {
                    userImage.Source = ApiConnector.Instance.ActiveUser.Avatar;
                }
                else
                {
                    userImage.Source = new BitmapImage(new Uri("/Images/SMOKE.png", UriKind.Relative));
                }
                
                userName.Text = ApiConnector.Instance.ActiveUser.profile.nombre;
                userAge.Text = ApiConnector.Instance.ActiveUser.profile.edad;
                userMajor.Text = ApiConnector.Instance.ActiveUser.profile.carrera;
                txtEvaluations.Text = ApiConnector.Instance.ActiveUser.profile.evaluaciones_count.ToString() +
                                      " Evaluaciones";
                //userImage.Source = (new BitmapImage(new Uri("/Images/User.jpg", UriKind.Absolute)));

                music = (ApiConnector.Instance.ActiveUser.profile.musica == 1);
                smoke = (ApiConnector.Instance.ActiveUser.profile.fuma == 1);
                airconditioner = (ApiConnector.Instance.ActiveUser.profile.aire == 1);
                talk = (ApiConnector.Instance.ActiveUser.profile.platicar == 1);

                if (!music)
                {
                    musicImage.Opacity = .30;
                }
                if (!smoke)
                {
                    smokeImage.Opacity = .30;
                }
                if (!airconditioner)
                {
                    acImage.Opacity = .30;
                }
                if (!talk)
                {
                    talkImage.Opacity = .30;
                }

                givenLiftCounttxt.Text = ApiConnector.Instance.ActiveUser.profile.aventones_dados_count.ToString();
                takenLiftCounttxt.Text = ApiConnector.Instance.ActiveUser.profile.aventones_recibidos_count.ToString();
                routCounttxt.Text = ApiConnector.Instance.ActiveUser.profile.rutas_count.ToString();
                descriptiontxt.Text = ApiConnector.Instance.ActiveUser.profile.descripcion.ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("At: \n" + ex.Message, "Error:", MessageBoxButton.OK);
                MessageBox.Show("At: \n" + "algo valio madres y es esto-> \n" + ex.Message, "Error:", MessageBoxButton.OK);
            }
        }

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            


        }

        #endregion

        private async void queryUserData()
        {
            await ApiConnector.Instance.UpdateCurrentUserData();
            //UserDataChanged();
            UserDataChanged();
        }

        private async void UpdateUserData()
        {
            if (saveNewData)
            {
                progress.showProgressIndicator(this, "Guardando");
                await ApiConnector.Instance.SaveUserDataToCloud();

                if (imageChanged)
                {
                    await ApiConnector.Instance.UploadImage(newImage, newImagePath);
                }
                //await ApiConnector.Instance.UpdateCurrentUserData();
                //UserDataChanged();
                progress.hideProgressIndicator(this);
            }
            saveNewData = true;

        }
        //este metodo es el boton que habilita la edicion o guarda los cambios 
        private void  ApplicationBarIconButton_OnClick(object sender, EventArgs e)
        {
            //toggle edition mode
            EditionEnabled = !EditionEnabled;
            
        }


#region Tap_Events

        private void Music_onTap(object sender, GestureEventArgs e)
        {
            if (_editionEnabled)
            
            {
                //musicImage.Opacity = user.profile.musica == 1 ? 30 : 100;
                if (ApiConnector.Instance.ActiveUser.profile.musica == 1)
                {
                    musicImage.Opacity = .30;
                    ApiConnector.Instance.ActiveUser.profile.musica = 0;//toggle value
                }
                else
                {
                    musicImage.Opacity = 100;
                    ApiConnector.Instance.ActiveUser.profile.musica = 1;//toggle value
                }
            }
        }

        private void Fuma_OnTap(object sender, GestureEventArgs e)
        {
            if (_editionEnabled)
            {
                //smokeImage.Opacity = user.profile.fuma == 1 ? 30 : 100;
                if (ApiConnector.Instance.ActiveUser.profile.fuma == 1)
                {
                    smokeImage.Opacity = .30;
                    ApiConnector.Instance.ActiveUser.profile.fuma = 0;
                }
                else
                {
                    smokeImage.Opacity = 100;
                    ApiConnector.Instance.ActiveUser.profile.fuma = 1;
                }
            }
        }

        private void AirConditioner_OnTap(object sender, GestureEventArgs e)
        {
            if (_editionEnabled)
            {
                //acImage.Opacity = user.profile.aire == 1 ? 30 : 100;
                if (ApiConnector.Instance.ActiveUser.profile.aire == 1)
                {
                    acImage.Opacity = .30;
                    ApiConnector.Instance.ActiveUser.profile.aire = 0;
                }
                else
                {
                    acImage.Opacity = 100;
                    ApiConnector.Instance.ActiveUser.profile.aire = 1;
                }
            }
        }

        private void Talk_OnTap(object sender, GestureEventArgs e)
        {
            if (_editionEnabled)
            {
                //talkImage.Opacity = user.profile.platicar == 1 ? 30 : 100;
                if (ApiConnector.Instance.ActiveUser.profile.platicar == 1)
                {
                    talkImage.Opacity = .30;
                    ApiConnector.Instance.ActiveUser.profile.platicar = 0;
                }
                else
                {
                    talkImage.Opacity = 100;
                    ApiConnector.Instance.ActiveUser.profile.platicar = 1;
                }
            }
        }

#endregion

        private void UserImage_OnTap(object sender, GestureEventArgs e)
        {
            if (_editionEnabled)
            {
                photoChooserTask.Show();
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_editionEnabled)
            {
                saveNewData = false;
                queryUserData();
                ApiConnector.Instance.ActiveUser.UserDataChanged += (o, args) =>
                {
                    if (ApiConnector.Instance.ActiveUser.Avatar != null)
                    {
                        userImage.Source = ApiConnector.Instance.ActiveUser.Avatar;
                    }
                };
                ApiConnector.Instance.UpdateCurrentProfileImage();
                e.Cancel = true;
                EditionEnabled = false;
            }
        }

        
    }
}