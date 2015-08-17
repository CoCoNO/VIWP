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
using VoyIteso.Pages.Chat2;

namespace VoyIteso.Pages
{
    public partial class ChatLayout : PhoneApplicationPage
    {
        private ChatView chat;
        ObservableCollection<object> messages = new ObservableCollection<object>();
        ObservableCollection<string> dummyMessages = new ObservableCollection<string>();

        public ChatLayout()
        {
            InitializeComponent();
            chat = new ChatView();
            ContentPanel.Children.Add(chat);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Mensajes listaDeMensajes = null;
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("key"))//the key is the Notificacion.aventon_id.ToString()
            {
                var val = NavigationContext.QueryString["key"];
                listaDeMensajes = await ApiConnector.Instance.LiftMessagesGet(Convert.ToInt32(val)); 
            }
            else
            {
                return;
            }
            foreach (var mensaje in listaDeMensajes.mensajes)
            {
                if (mensaje.perfil_id.ToString().Equals(ApiConnector.Instance.ActiveUser.profileID))//si el mensaje pertenece al usuario. 
                {
                    ChatView.messages.Add(mensaje.texto);
                }
                else//sino pertenece a la segunda persona. 
                {
                    //ChatView.dummyMessages.Add(mensaje.texto);

                }

            }
        }
    }
}