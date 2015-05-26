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
using VoyIteso.Resources;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using Telerik.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Windows.Threading;

namespace VoyIteso.Pages
{
    public partial class ProfilePage : PhoneApplicationPage
    {
        User user = new User();

        ApiConnector apiConnector = ApiConnector.instance;

        private int smoke, ac, music, pet, talk;

        enum ProfileStates { UserProfile, EditProfile, OtherUserProfile};

        bool isPhotoChanged;

        ProfileStates profileState;

        TextBlock descriptionTextBlock;

        TextBox descriptionTextBox;

        Progress progress = new Progress();

        RadMoveYAnimation moveAnimationStart;

        RadMoveYAnimation moveAnimationEnd;

        PhotoChooserTask photoChooserTask;

        DispatcherTimer AnimationTimer = new DispatcherTimer();

        String otherUserId;

        byte[] photoStream;

        string photoName;

        public ProfilePage()
        {
            InitializeComponent();
        }

        void moveAnimationStart_Ended(object sender, AnimationEndedEventArgs e)
        {
            AnimationTimer.Start();
        }

        void moveAnimationEnd_Ended(object sender, AnimationEndedEventArgs e)
        {
            FeedBackImage.Opacity = 0;
            ApplicationBar.IsVisible = true;
        }

        void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (moveAnimationEnd != null)
            {
                RadAnimationManager.Play(this.FeedBackImage, moveAnimationEnd);
            }
            AnimationTimer.Stop();
        }

        void apiConnector_exceptionChanged(object sender, EventArgs e)
        {
            if (apiConnector.throwException != null)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(apiConnector.throwException, "Error", MessageBoxButton.OK);
                    progress.hideProgressIndicator(this);
                });

            }
        }

        private void apiConnector_responseChanged(object sender, EventArgs e)
        {
            if (profileState == ProfileStates.UserProfile)
            {
                Perfil userProfile = apiConnector.getProfile();
                Dispatcher.BeginInvoke(() =>
                {
                    if (userProfile != null)
                    {
                        txtProfileName.Text = userProfile.nombre;
                        txtProfileMajor.Text = "·" + userProfile.carrera;
                        txtDescription.Text = userProfile.descripcion;
                        txtProfileAge.Text = "·" + userProfile.edad + " años";
                        ratingStars.Value = (double)userProfile.rating;
                        music = userProfile.musica;
                        smoke = userProfile.fuma;
                        ac = userProfile.aire;
                        txtEvaluations.Text = userProfile.evaluaciones_count + " evaluaciones";
                        txtGivenLiftsNumber.Text = userProfile.aventones_dados_count.ToString();
                        txtAskedLiftsNumber.Text = userProfile.aventones_recibidos_count.ToString();
                        txtRoutsNumber.Text = userProfile.rutas_count.ToString();
                        talk = userProfile.platicar;
                        if (music == 0)
                            MusicPropertyImage.Opacity = 0.3;
                        if (smoke == 0)
                            SmokePropertyImage.Opacity = 0.3;
                        if (ac == 0)
                            ACPropertyImage.Opacity = 0.3;
                        if (talk == 0)
                            TalkPropertyImage.Opacity = 0.3;
                    }
                    else
                        MessageBox.Show("Ocurrio un error en el servidor, vuelve a intentarlo", "Error", MessageBoxButton.OK);
                });
            }
            else if(profileState == ProfileStates.EditProfile)
            {
                if (apiConnector.jsonResponse != null)
                {
                    Dispatcher.BeginInvoke(() => {
                        if (apiConnector.getEditProfileRequestResult())
                        {
                            if (isPhotoChanged)
                            {
                                apiConnector.UpdateUrl(AppResources.ApiEditUserPhoto);
                                apiConnector.SendPostImageRequest(photoStream, user.Token, photoName);
                                isPhotoChanged = false;
                            }
                            else
                            {
                                ApplicationBar.IsVisible = false;
                                if (moveAnimationStart != null)
                                {
                                    FeedBackImage.Opacity = 1;
                                    RadAnimationManager.Play(this.FeedBackImage, moveAnimationStart);
                                }
                                descriptionTextBlock.Text = descriptionTextBox.Text;
                                DescriptionGrid.Children.RemoveAt(DescriptionGrid.Children.Count - 1);
                                DescriptionGrid.Children.Add(descriptionTextBlock);
                                profileState = ProfileStates.UserProfile;
                                CameraImage.Opacity = 0;
                                BuildLocalizedApplicationBar();
                                progress.hideProgressIndicator(this);
                            }

                        }
                        else
                        {
                            MessageBox.Show("Ocurrio un error en el servidor, vuelve a intentarlo", "Error", MessageBoxButton.OK);
                            profileImage.ImageSource = (new BitmapImage(new Uri(user.imageUrl, UriKind.Absolute)));
                        }
                            
                        
                    });
                    
                }
            }

        }

        

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext != null && NavigationContext.QueryString.ContainsKey("otherUserId"))
            {
                profileState = ProfileStates.OtherUserProfile;
            }
            else
            {
                profileState = ProfileStates.UserProfile;
            }

            if (profileState == ProfileStates.UserProfile)
            {
                setLayout();
                apiConnector.responseChanged += apiConnector_responseChanged;
                apiConnector.exceptionChanged += apiConnector_exceptionChanged;
                user.getInfo(user.key);
                setProfileUrl();
                apiConnector.SendGetRequest();
                if (user.setImageUrl())
                {
                    profileImage.ImageSource = (new BitmapImage(new Uri(string.Format(user.imageUrl + "?Refresh=true&random={0}",Guid.NewGuid()), UriKind.Absolute)));
                }
                BuildLocalizedApplicationBar();
                setAnimation();
                descriptionTextBlock = (TextBlock)DescriptionGrid.Children.ElementAt(DescriptionGrid.Children.Count - 1);
                descriptionTextBox = new TextBox();
                descriptionTextBox.Name = descriptionTextBlock.Name;
                descriptionTextBox.FontFamily = descriptionTextBlock.FontFamily;
                descriptionTextBox.FontSize = descriptionTextBlock.FontSize;
                descriptionTextBox.GotFocus += descriptionTextBox_GotFocus;
                descriptionTextBox.KeyDown += descriptionTextBox_KeyDown;
                descriptionTextBox.LostFocus += descriptionTextBox_LostFocus;
                photoChooserTask = new PhotoChooserTask();
                photoChooserTask.Completed += photoChooserTask_Completed;
                isPhotoChanged = false;
            }
            else if(profileState == ProfileStates.OtherUserProfile)
            {
                setLayout();
                apiConnector.responseChanged += apiConnector_responseChanged;
                apiConnector.exceptionChanged += apiConnector_exceptionChanged;
                string imageUrl = AppResources.ApiBaseUrl + AppResources.ApiGetProfileImage;
                string otherUserId = NavigationContext.QueryString["otherUserId"];
                imageUrl = String.Format(imageUrl, otherUserId);
                profileImage.ImageSource = (new BitmapImage(new Uri(string.Format(imageUrl, UriKind.Absolute))));
            }
            
        }

        #endregion

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (profileState == ProfileStates.UserProfile || profileState == ProfileStates.OtherUserProfile)
            {
                base.OnBackKeyPress(e);
            }
            else
            {
                profileState = ProfileStates.UserProfile;
                DescriptionGrid.Children.RemoveAt(DescriptionGrid.Children.Count - 1);
                DescriptionGrid.Children.Add(descriptionTextBlock);
                CameraImage.Opacity = 0;
                BuildLocalizedApplicationBar();
                e.Cancel = true;
            }
        }

        void descriptionTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = false;
        }

        void descriptionTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = true;
        }

        void descriptionTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                ratingStars.Focus();
            }
        }

        #region user properties buttons
        private void MusicPropertyImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (profileState == ProfileStates.EditProfile)
            {
                if (music == 1)
                {
                    MusicPropertyImage.Opacity = 0.3;
                    music = 0;
                }
                else
                {
                    MusicPropertyImage.Opacity = 1;
                    music = 1;
                }
            }
        }

        private void SmokePropertyImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (profileState == ProfileStates.EditProfile)
            {
                if (smoke == 1)
                {
                    SmokePropertyImage.Opacity = 0.3;
                    smoke = 0;
                }
                else
                {
                    SmokePropertyImage.Opacity = 1;
                    smoke = 1;
                }
            }
        }

        private void ACPropertyImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (profileState == ProfileStates.EditProfile)
            {
                if (ac == 1)
                {
                    ACPropertyImage.Opacity = 0.3;
                    ac = 0;
                }
                else
                {
                    ACPropertyImage.Opacity = 1;
                    ac = 1;
                }
            }
        }

        private void TalkPropertyImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (profileState == ProfileStates.EditProfile)
            {
                if (talk == 1)
                {
                    TalkPropertyImage.Opacity = 0.3;
                    talk = 0;
                }
                else
                {
                    TalkPropertyImage.Opacity = 1;
                    talk = 1;
                }
            }
        }
        #endregion

        private void setProfileUrl()
        {
            String url = String.Format(AppResources.ApiGetProfile, user.profileID);
            apiConnector.UpdateUrl(url);
            apiConnector.setParametersUrl(AppResources.ApiSecurityToken + user.Token);
            apiConnector.setUrlParametersUrl();
        }

        #region AppBarItems & Buttons
        void appBarEditProfileItem_Click(object sender, EventArgs e)
        {
            profileState = ProfileStates.EditProfile;
            CameraImage.Opacity = 1;
            descriptionTextBox.Text = descriptionTextBlock.Text;
            DescriptionGrid.Children.RemoveAt(DescriptionGrid.Children.Count - 1);
            Grid.SetRow(descriptionTextBox, 3);
            Grid.SetColumn(descriptionTextBox, 1);
            DescriptionGrid.Children.Add(descriptionTextBox);
            BuildLocalizedApplicationBar();
        }

        void appBarSaveProfileButton_Click(object sender, EventArgs e)
        {
            apiConnector.UpdateUrl(AppResources.ApiEditProfile);
            apiConnector.setParametersUrl(
                "descripcion=" + descriptionTextBox.Text +
                "&otrasCostumbres= " +
                "&fuma=" + smoke +
                "&aire=" + ac +
                "&musica=" + music +
                "&mascota=0" +
                "&platicar=" + talk +
                "&security_token=" + user.Token
                );
            progress.showProgressIndicator(this, "Guardando");
            apiConnector.SendPostRequest();
            
            
            
        }
        #endregion

        #region BuildLocalizedApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.BackgroundColor = Color.FromArgb(255, 0, 66, 112);

            if (profileState == ProfileStates.UserProfile)
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;

                ApplicationBarMenuItem appBarEditProfileItem = new ApplicationBarMenuItem("Editar");
                appBarEditProfileItem.Click += appBarEditProfileItem_Click;
                ApplicationBar.MenuItems.Add(appBarEditProfileItem);
            }
            else if(profileState == ProfileStates.EditProfile)
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;

                ApplicationBarIconButton appBarSaveProfileButton = new ApplicationBarIconButton(new Uri("Assets/save.png", UriKind.Relative));
                appBarSaveProfileButton.Text = "Guardar PErfil";
                appBarSaveProfileButton.Click += appBarSaveProfileButton_Click;
                ApplicationBar.Buttons.Add(appBarSaveProfileButton);
            }

        }
        #endregion

        #region setLayout
        private void setLayout ()
        {
            int phoneWidth = (int)Application.Current.Host.Content.ActualWidth;
            int phoneHeight = (int)Application.Current.Host.Content.ActualHeight;
            int topMarginHeight = 30;

            int profileHeight = phoneHeight - topMarginHeight;
            int userPropertiesWidth = phoneWidth - 48;
            int propertieImageSize = ((userPropertiesWidth / 6) - 24);
            int generalInfoHeight = ((profileHeight / 30) * 14);
            int userPropertiesHeight = ((profileHeight / 30) * 5);
            int liftInfoHeight = ((profileHeight / 30) * 3);
            int descriptionHeight = ((profileHeight / 30) * 8);
            Binding binding = new Binding();

            binding.Mode = BindingMode.OneTime;
            binding.Source = generalInfoHeight;
            GeneralInfoGrid.SetBinding(Grid.HeightProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneWidth;
            GeneralInfoGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = userPropertiesHeight;
            UserPropertiesGrid.SetBinding(Grid.HeightProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneWidth;
            UserPropertiesGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = userPropertiesHeight;
            UserPropertiesGrid.SetBinding(Grid.HeightProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = liftInfoHeight;
            LiftDataGrid.SetBinding(Grid.HeightProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneWidth;
            LiftDataGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = phoneWidth;
            DescriptionGrid.SetBinding(Grid.WidthProperty, binding);

            binding = new Binding();
            binding.Mode = BindingMode.OneTime;
            binding.Source = descriptionHeight;
            DescriptionGrid.SetBinding(Grid.HeightProperty, binding);

            FeedBackImage.Width = phoneWidth;
            FeedBackImage.Height = phoneHeight;
            
        }
        #endregion

        #region setAnimation
        private void setAnimation()
        {
            moveAnimationStart = new RadMoveYAnimation();
            moveAnimationStart = this.Resources["feedbackAnimationStart"] as RadMoveYAnimation;
            moveAnimationStart.Duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
            moveAnimationStart.Ended += moveAnimationStart_Ended;
            moveAnimationStart.StartY = (int)Application.Current.Host.Content.ActualHeight * -1;
            moveAnimationStart.EndY = 0;

            moveAnimationEnd = new RadMoveYAnimation();
            moveAnimationEnd = this.Resources["feedbackAnimationend"] as RadMoveYAnimation;
            moveAnimationEnd.Duration = new Duration(new TimeSpan(0, 0, 0, 1, 0));
            moveAnimationEnd.Ended += moveAnimationEnd_Ended;
            moveAnimationEnd.StartY = 0;
            moveAnimationEnd.EndY = (int)Application.Current.Host.Content.ActualHeight * -1;
            AnimationTimer.Interval = TimeSpan.FromSeconds(2);
            AnimationTimer.Tick += AnimationTimer_Tick;
        }
        #endregion

        private void TopMarginGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(profileState == ProfileStates.EditProfile)
                photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, Microsoft.Phone.Tasks.PhotoResult e)
        {
            if (profileState == ProfileStates.EditProfile)
            {
                if (e.TaskResult == TaskResult.OK)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(e.ChosenPhoto);
                        photoName = e.OriginalFileName;
                        profileImage.ImageSource = bmp;
                        using (var ms = new MemoryStream())
                        {
                            WriteableBitmap btmMap = new WriteableBitmap(bmp);
                            Extensions.SaveJpeg(btmMap, ms, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                            photoStream = ms.ToArray();   
                        }
                        isPhotoChanged = true;
                    });

                }
            }
        }


    }
}