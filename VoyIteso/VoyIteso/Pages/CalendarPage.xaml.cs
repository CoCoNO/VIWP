using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using VoyIteso.Class;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace VoyIteso.Pages
{
    public partial class Calendar : PhoneApplicationPage
    {
        public Calendar()
        {
            InitializeComponent();

            initAppointments();

            
        }

        

        //<summary>
        //This method gets the appointments and display them in the appointment detail text.
        //</summary>
        private async void initAppointments()
        {
            MyAppointmentSource myAppointmentSource = new MyAppointmentSource();


            //ApiConnector.instance.LoadCurrentMonthLifts()
            myAppointmentSource.clearAllAppointments();
            /*
            myAppointmentSource.addAppointment(new Appointment()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(1),
                Subject = "Today Appointment 1",
                Details = "Some Long Details are coming here",
                Location = "My Home Town"
            });

            myAppointmentSource.addAppointment(new Appointment()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(1),
                Subject = "apponint shit",
                Details = "details of appointshit",
                Location = "Zapopan, Jal. Mex"
            });*/

            Appointment[] apps=await ApiConnector.Instance.LoadCurrentMonthLifts();//a lift is an appointment
            foreach (var appointment in apps)
            {
                myAppointmentSource.addAppointment(appointment);
            }
            myAppointmentSource.onDataLoaded();

            // add appointment source to calendar
            myCalendar.AppointmentSource = myAppointmentSource;

            //var a = myAppointmentSource.getAppointmentList();
            //myAppointmentSource.Fetch();
            
            //foreach (var appointment in a)
            //{
            //    Appointment ap = appointment;
            //    appointmentDetails.Text = "";
            //    appointmentDetails.Text += "-->" + ap.Subject + "\n" + ap.StartDate + "\n\n" + ap.Details;
            //}
        }

        //<summary>
        //Tap event handler.
        //</summary>
        private void myCalendar_ItemTap(object sender, CalendarItemTapEventArgs e)
        {
            appointmentDetails.Text = this.myCalendar.SelectedValue.ToString();
            //myCalendar_SelectedValueChanged(null,null);
            //MessageBox.Show("howdy world");
            //myCalendar_SelectedValueChanged(null,null);
        }

        /// <summary>
        /// este metodo imprime en la pantalla los detalles del día seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myCalendar_SelectedValueChanged(object sender, ValueChangedEventArgs<object> e)
        {
            //DateTime? selectedDate = this.myCalendar.SelectedValue;

            DateTime selectedDate = (DateTime)this.myCalendar.SelectedValue;
            MyAppointmentSource appointmentSource = (MyAppointmentSource)myCalendar.AppointmentSource;
            var list = appointmentSource.getAppointmentList();//List<Appointment>. list of appointments.
            appointmentDetails.Text = "";
            foreach (var item in list)
            {
                if (item.StartDate.Day == selectedDate.Day)
                {
                    appointmentDetails.Text += "-->" + item.Subject + "\n" + item.StartDate + "\n\n" + item.Details; 
                }
            }

            //appointmentDetails.Text = selectedDate.ToString() ;
            if (e.NewValue == null)
            {

            }
        }

        private void AppointmentDetails_OnTap(object sender, GestureEventArgs e)
        {
            //esta es el evento de tap al texto
            //NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/RouteInfo.xaml",UriKind.Relative));
            Debug.WriteLine("Not yet implemented."); 
        }

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;
            ApplicationBar.Opacity = 1.0;
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            ApplicationBarIconButton advancedDetailButton = new ApplicationBarIconButton(new Uri("Assets/check.png", UriKind.Relative));
            advancedDetailButton.Text = "detalles";
            advancedDetailButton.Click += advancedDetailButton_Click;
            ApplicationBar.Buttons.Add(advancedDetailButton);

        }

        private async void advancedDetailButton_Click(object sender, EventArgs e)
        {// esta madre es la del appbar
            
            //var a = await ApiConnector.Instance.NotificationsGet();
            //Debug.WriteLine("lista notifs..."+a.notificaciones.Count.ToString());
            //var b = await ApiConnector.Instance.LoadCurrentMonthLifts();
            //Debug.WriteLine("lista current month lifts"+b.Count().ToString());
            ////NavigationService.Navigate(new Uri("/Pages/NotificationsStuff/RouteInfo.xaml",UriKind.Relative));
        }

    }

    public class MyAppointmentSource : AppointmentSource
    {
        private List<Appointment> _appointmentList;
        public List<Appointment> getAppointmentList()
        {
            return _appointmentList;
        }

        public MyAppointmentSource()
        {
            _appointmentList = new List<Appointment>();
        }


        //<summary> 
        //Adds given appointment.
        //</summary> 
        public void addAppointment(Appointment appointment)
        {
            this.AllAppointments.Add(appointment);
            this._appointmentList.Add(appointment);
        }

        //<summary> 
        //removes given appointment.
        //</summary> 
        public void removeAppointment(Appointment appointment)
        {
            this.AllAppointments.Remove(appointment);
            this._appointmentList.Remove(appointment);
        }

        //<summary> 
        //Clears all appointments.
        //</summary> 
        public void clearAllAppointments()
        {
            this.AllAppointments.Clear();
        }

        //<summary> 
        //Notify that there's new data to display. 
        //</summary> 
        public void onDataLoaded()
        {
            this.OnDataLoaded();
        }

        public override void FetchData(DateTime startDate, DateTime endDate)
        {


            /*
             
             this.AllAppointments.Clear();
            this.AllAppointments.Add(new Appointment()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(1),
                Subject = "Today Appointment 1",
                Details = "Some Long Details are coming here",
                Location = "My Home Town"
            });
            this.AllAppointments.Add(new Appointment()
            {
                StartDate = DateTime.Now.AddMinutes(30),
                EndDate = DateTime.Now.AddHours(1),
                Subject = "Appointment 2",
                Details = "Some Long Details are coming here",
                Location = "Paris"
            });
            this.OnDataLoaded(); // notify that there is new data to display
             
             */

        }
    }

    public class Appointment : IAppointment
    {
        /// <summary> 
        /// Gets the details of the appointment. 
        /// </summary> 
        public string Details
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets the end date and time of the appointment. 
        /// </summary> 
        public DateTime EndDate
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets the location of the appointment. 
        /// </summary> 
        public string Location
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets the start date and time of the appointment. 
        /// </summary> 
        public DateTime StartDate
        {
            get;
            set;
        }

        /// <summary> 
        /// Gets the subject of the appointment. 
        /// </summary> 
        public string Subject
        {
            get;
            set;
        }

        public int LiftID { get; set; }

        /// <summary> 
        /// Gets a string representation of the appointment 
        /// </summary> 
        /// <returns>A string representation of the appointment</returns> 
        public override string ToString()
        {
            return this.Subject;
        }
    }

}