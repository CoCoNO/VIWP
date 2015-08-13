using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace VoyIteso.Pages.Chat2
{


    public partial class ChatView : UserControl
    {
        ObservableCollection<object> messages = new ObservableCollection<object>();
        ObservableCollection<string> dummyMessages = new ObservableCollection<string>()
        {
            "Hey you! I'm Dummy. Tell me what's on your mind!",
            "Great talkers are little doers. Are you a great talker?",
            "Huh?",
            "One ring to rule them all. One ring to bind them, and in the darkness bind them!",
            "Wow!",
            "I'm not speaking to you ever again!",
            "Thor is here!",
            "The force flows strong in you!",
            "虎穴に入らずんば虎子を得ず ^^",
            "Hodor!",
            "Talk to the hand!",
            "You shall not pass!",
            "Moo! I'm a goat!",
            "Sup' dawg?",
            "I'll be back!"
        };

        IEnumerator<string> dummyMessagesEnumerator;
        DispatcherTimer timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(2),
        };
        DispatcherTimer startTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        public ChatView()
        {
            InitializeComponent();

            this.dummyMessagesEnumerator = new InfiniteEnumerator(this.dummyMessages);
            this.timer.Tick += this.OnTimerTick;
            this.startTimer.Tick += this.OnStartTimerTick;

            this.conversationView.ItemsSource = this.messages;
            this.conversationView.CreateMessage = (string text) => new CustomMessage(text, DateTime.Now, ConversationViewMessageType.Outgoing);

            this.dummyMessagesEnumerator.MoveNext();
            this.messages.Add(this.CreateIncomingMessage(this.dummyMessagesEnumerator.Current));
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
            this.messages.Add(this.CreateIncomingMessage(this.dummyMessagesEnumerator.Current));
            this.timer.Stop();
            VibrateController.Default.Start(TimeSpan.FromSeconds(0.2));
        }

        private void OnSendingMessage(object sender, ConversationViewMessageEventArgs e)
        {
            if (string.IsNullOrEmpty((e.Message as CustomMessage).Text))
            {
                return;
            }

            this.messages.Add(e.Message);

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








    //------------
     
}
