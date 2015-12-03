using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using VoyIteso.Class;
using VoyIteso.Pages.ChatStuff;

namespace VoyIteso.Pages
{
    public partial class ChatLayout : PhoneApplicationPage
    {
        private ChatView myChat;
        ObservableCollection<object> messages = new ObservableCollection<object>();
        ObservableCollection<string> dummyMessages = new ObservableCollection<string>();
        public static string key;

        public ChatLayout()
        {
            InitializeComponent();
            myChat = new ChatView();
            //LoadMessages();
            //ContentPanel.Children.Add(myChat);
        }

        /// <summary>
        /// Load messages into ChatView
        /// </summary>
        private async void LoadMessages()
        {
            //Mensajes listaDeMensajes = null;
            //if (NavigationContext.QueryString.ContainsKey("key"))//the key is the Notificacion.aventon_id.ToString()
            //{
            //    var val = NavigationContext.QueryString["key"];
            //    listaDeMensajes = await ApiConnector.Instance.LiftMessagesGet(Convert.ToInt32(val));//aqui se obtiene la lista de mensajes del aventon seleccionado.
            //}
            //else
            //{
            //    return;
            //}
            //foreach (var mensaje in listaDeMensajes.mensajes)
            //{
            //    /////si el mensaje pertenece al usuario/////
            //    if (mensaje.perfil_id.ToString().Equals(ApiConnector.Instance.ActiveUser.profileID))//si el mensaje pertenece al usuario. 
            //    {
            //        myChat.messages.Add(mensaje.texto);
            //        //ChatView.messages.Add(mensaje.texto);
            //    }
            //    /////sino el mensaje pertenece a la segunda persona./////
            //    else
            //    {
            //        myChat.dummyMessages.Add(mensaje.texto);
            //        //ChatView.dummyMessages.Add(mensaje.texto);
            //    }

            //}
            //myChat.foo();
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Mensajes listaDeMensajes = null;
            //Se debe utilizar el key en el onNavigated Ya que el key no existe antes de que haya navegado a este layout
            if (NavigationContext.QueryString.ContainsKey("key"))//the key is the Notificacion.aventon_id.ToString()
            {
                var val = NavigationContext.QueryString["key"];
                key = val;
                //listaDeMensajes = await ApiConnector.Instance.LiftMessagesGet(Convert.ToInt32(val));//aqui se obtiene la lista de mensajes del aventon seleccionado.
            }
            else
            {
                MessageBox.Show("Hubo un error");
                return;
            }
            //foreach (var mensaje in listaDeMensajes.mensajes)
            //{
            //    /////si el mensaje pertenece al usuario/////
            //    if (mensaje.perfil_id.ToString().Equals(ApiConnector.Instance.ActiveUser.profileID))//si el mensaje pertenece al usuario. 
            //    {
            //        myChat.messages.Add(mensaje.texto);
            //        //ChatView.messages.Add(mensaje.texto);
            //    }
            //    /////sino el mensaje pertenece a la segunda persona./////
            //    else
            //    {
            //        myChat.dummyMessages.Add(mensaje.texto);
            //        //ChatView.dummyMessages.Add(mensaje.texto);
            //    }

            //}
            myChat.foo();

            ContentPanel.Children.Add(myChat);
        }
    }
}