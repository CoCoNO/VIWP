using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RestSharp.Extensions;
using Telerik.Windows.Controls;
using VoyIteso.Class;

namespace VoyIteso.Pages.ChatStuff
{

    public partial class ChatView : UserControl
    {
        public ObservableCollection<object> messages = new ObservableCollection<object>();
        public ObservableCollection<string> dummyMessages = new ObservableCollection<string>();
        //public static ObservableCollection<string> dummyMessages = new ObservableCollection<string>()
        //{
        //    "Hey you! I'm Dummy. Tell me what's on your mind!",
        //    "Great talkers are little doers. Are you a great talker?",
        //    "Huh?",
        //    "One ring to rule them all. One ring to bind them, and in the darkness bind them!",
        //    "Wow! how stupid you are",
        //    "I'm not speaking to you ever again!",
        //    "Chinga tu madre wey!",
        //    "The force flows strong in you!",
        //    "虎穴に入らずんば虎子を得ず ^^",
        //    "Hodor!",
        //    "Talk to the hand!",
        //    "You shall not pass!",
        //    "Moo! I'm a goat!",
        //    "Sup' dawg?",
        //    "I'll be back!"
        //};

        IEnumerator<string> dummyMessagesEnumerator;
        DispatcherTimer timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        DispatcherTimer startTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        public ChatView()//(ObservableCollection<object> myMessages, ObservableCollection<string> secondPartyMessages)
        {
            InitializeComponent();

            //UserAvatar = new BitmapImage();// ApiConnector.Instance.ActiveUser.Avatar;
            //this.messages = myMessages;
            //this.dummyMessages = secondPartyMessages;

            //foo();
            //this.dummyMessagesEnumerator = new InfiniteEnumerator(dummyMessages);
            //this.timer.Tick += this.OnTimerTick;
            //this.startTimer.Tick += this.OnStartTimerTick;

            //this.conversationView.ItemsSource = messages;
            //this.conversationView.CreateMessage = (string text) => new CustomMessage(text, DateTime.Now, ConversationViewMessageType.Outgoing);

            //this.dummyMessagesEnumerator.MoveNext();
            //messages.Add(this.CreateIncomingMessage(this.dummyMessagesEnumerator.Current)); 
            
        }

        public async void foo()
        {
            //this.dummyMessagesEnumerator = new InfiniteEnumerator(dummyMessages);
            //this.dummyMessagesEnumerator = new 

            this.conversationView.ItemsSource = messages;
            this.conversationView.CreateMessage = (string text) => new CustomMessage(text, DateTime.Now, ConversationViewMessageType.Outgoing);

            //this.dummyMessagesEnumerator.MoveNext();

            //foreach (var message in dummyMessages)
            //{
            //    messages.Add(this.CreateIncomingMessage(this.dummyMessagesEnumerator.Current));
            //    this.dummyMessagesEnumerator.MoveNext();
            //}

            Mensajes listaDeMensajes = null;

            var key = ChatLayout.key;

            new Progress().showProgressIndicator(this, "cargando mensajes");
            listaDeMensajes = await ApiConnector.Instance.LiftMessagesGet(Convert.ToInt32(key));//aqui se obtiene la lista de mensajes del aventon seleccionado.
            new Progress().hideProgressIndicator(this);

            foreach (var mensaje in listaDeMensajes.mensajes)
            {
                var custommessa = new CustomMessage("",DateTime.Now,ConversationViewMessageType.Incoming);
                /////si el mensaje pertenece al usuario/////
                if (mensaje.perfil_id.ToString().Equals(ApiConnector.Instance.ActiveUser.profileID))//si el mensaje pertenece al usuario. 
                {
                    custommessa = new CustomMessage(mensaje.texto, DateTime.Now, ConversationViewMessageType.Outgoing);
                    messages.Add(custommessa);
                }
                /////sino el mensaje pertenece a la segunda persona./////
                else
                {
                    custommessa = new CustomMessage(mensaje.texto, DateTime.Now, ConversationViewMessageType.Incoming);
                    messages.Add(custommessa);
                }

            }


        }


        private CustomMessage CreateIncomingMessage(string text)
        {
            return new CustomMessage(text, DateTime.Now, ConversationViewMessageType.Incoming);
        }

        private void OnStartTimerTick(object sender, EventArgs e)
        {
            this.startTimer.Stop();
            this.typingTextBlock.Text = "El dummy está escribiendo";
            this.timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this.typingTextBlock.Text = "";
            this.dummyMessagesEnumerator.MoveNext();
            messages.Add(this.CreateIncomingMessage(this.dummyMessagesEnumerator.Current));
            this.timer.Stop();
            VibrateController.Default.Start(TimeSpan.FromSeconds(0.2));
        }

        private void OnSendingMessage(object sender, ConversationViewMessageEventArgs e)
        {
            if (string.IsNullOrEmpty((e.Message as CustomMessage).Text))
            {
                return;
            }

            messages.Add(e.Message);
            //aqui va a mandar el mensaje puto jairo.

            if (this.timer.IsEnabled || this.startTimer.IsEnabled)
            {
                return;
            }

            this.startTimer.Start();
        }
    }

    public class InfiniteEnumerator : IEnumerator<string>
    {
        private IEnumerator<string> enumerator;
        private IEnumerable<string> collection;

        public InfiniteEnumerator(IEnumerable<string> collection)
        {
            this.enumerator = collection.GetEnumerator();
            this.collection = collection;
        }

        public string Current
        {
            get
            {
                return this.enumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.enumerator.Current;
            }
        }

        public void Dispose()
        {
            this.enumerator.Dispose();
        }

        public bool MoveNext()
        {
            if (!this.enumerator.MoveNext())
            {
                this.enumerator = this.collection.GetEnumerator();
                this.enumerator.MoveNext();
            }

            return true;
        }

        public void Reset()
        {
            this.enumerator.Reset();
        }

        string IEnumerator<string>.Current
        {
            get
            {
                return this.enumerator.Current;
            }
        }
    }

    public class CustomMessage : ConversationViewMessage
    {
        public CustomMessage(string text, DateTime timeStamp, ConversationViewMessageType type)
            : base(text, timeStamp, type)
        {
        }

        public string FormattedTimeStamp
        {
            get
            {
                return this.TimeStamp.ToShortTimeString();
            }
        }
    }

}
