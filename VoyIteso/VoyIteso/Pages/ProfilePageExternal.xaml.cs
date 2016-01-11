using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ProfilePageExternal : PhoneApplicationPage
    {

        public static int Perfildelotrowey=0;


        public ProfilePageExternal()
        {
            InitializeComponent();

            BuildAppBar();

        }


        private void BuildAppBar()
        {
            var a = new ApplicationBar();
            var b = new ApplicationBarIconButton(new Uri("/Images/icons/close.png",UriKind.Relative));
            b.Text = "cerrar";
            b.Click += cerrar_clicked;
            a.Buttons.Add(b);
            a.IsVisible = true;
            a.Mode = ApplicationBarMode.Default;
            ApplicationBar = a;
        }

        private void cerrar_clicked(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        #region OnNavigatedTo
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var a = ApiConnector.Instance.GetUserImageById(Perfildelotrowey);
            userImage.Source = a;
            var b = await ApiConnector.Instance.GetUserById(Perfildelotrowey.ToString());

            userName.Text = b.profile.nombre;
            userAge.Text =b.profile.edad;
            userMajor.Text = b.profile.carrera;
            txtEvaluations.Text = b.profile.evaluaciones_count + " " + "evaluaciones";


            var music = (b.profile.musica == 1);
            var smoke = (b.profile.fuma == 1);
            var airconditioner = (b.profile.aire == 1);
            var talk = (b.profile.platicar == 1);

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

            descriptiontxt.Text = b.profile.descripcion;

            givenLiftCounttxt.Text = b.profile.aventones_dados_count.ToString();
            takenLiftCounttxt.Text = b.profile.aventones_recibidos_count.ToString();
            routCounttxt.Text = b.profile.rutas_count.ToString();

            //

            NavigationService.RemoveBackEntry();
        }

        #endregion
         


        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            NavigationService.RemoveBackEntry();
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }
    }
}