using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using VoyIteso.Class;

namespace VoyIteso.Pages.CalendarComponents
{
    public partial class SpecialDays : UserControl
    {


        public static List<DateTime> Days = new List<DateTime>(); 




        /// <summary>
        /// Constructor of class
        /// </summary>
        public SpecialDays()
        {
            InitializeComponent();
            InitAppointments();
            //myCalendar_SelectedValueChanged(null,null);
            //UpdateLayout();
            DateTime fechaFin = new DateTime(2015, 9, 1);
            var fechaInicio = new DateTime(2015, 8, 1);
            while (fechaInicio > fechaFin)
            {
                // do something with target.Month and target.Year
                fechaFin = fechaFin.AddDays(1);
                Days.Add(fechaFin);
            }

            foreach (var d in Days)
            {
                MyCalendar.SelectedValue = d.Date;
            }
        }






        //<summary>
        //This method gets the appointments from the server and load them to the calendar.
        //</summary>
        private async void InitAppointments()
        {
            MyAppointmentSource myAppointmentSource = new MyAppointmentSource();
            myAppointmentSource.clearAllAppointments();

            //Appointment[] apps = await ApiConnector.Instance.LoadCurrentMonthLifts();//a lift is an appointment
            Appointment[] apps = HomePage.apps;//a lift is an appointment
            
            foreach (var appointment in apps)
            {
                myAppointmentSource.addAppointment(appointment);
                Days.Add(appointment.StartDate);
            }
            myAppointmentSource.onDataLoaded();

            // add appointment source to calendar
            MyCalendar.AppointmentSource = myAppointmentSource;
            
        }


        


        /// <summary>
        /// este metodo imprime en la pantalla los detalles del día seleccionado.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myCalendar_SelectedValueChanged(object sender, ValueChangedEventArgs<object> e)
        {
            //DateTime? selectedDate = this.MyCalendar.SelectedValue;
            if (e==null)
            {
                return;
            }
            
            var selectedValue = this.MyCalendar.SelectedValue;
            if (selectedValue != null)
            {
                DateTime selectedDate = (DateTime)selectedValue;
                MyAppointmentSource appointmentSource = (MyAppointmentSource)MyCalendar.AppointmentSource;
                var list = appointmentSource.getAppointmentList();//List<Appointment>. list of appointments.
                AppointmentDetails.Text = "";
                AppointmentDetails2.Text = "";
                AppointmentDetails3.Text = "";

                foreach (var item in list)
                {
                    string exampleString = item.Details;
                    string[] words = exampleString.Split('*');
                    if (item.StartDate.Day == selectedDate.Day)
                    {
                        AppointmentDetails.Text += words[0];
                        AppointmentDetails2.Text += words[1];
                        AppointmentDetails3.Text += words[2];
                    }
                }
            }

            //appointmentDetails.Text = selectedDate.ToString() ;
            //if (e.NewValue == null)
            //{
            //    return;
            //}

        }









        /*
         
         * 
         * private async void initAppointments()
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
        /*
            Appointment[] apps = await ApiConnector.Instance.LoadCurrentMonthLifts();//a lift is an appointment
            foreach (var appointment in apps)
            {
                myAppointmentSource.addAppointment(appointment);
            }
            myAppointmentSource.onDataLoaded();

            // add appointment source to calendar
            MyCalendar.AppointmentSource = myAppointmentSource;

            //var a = myAppointmentSource.getAppointmentList();
            //myAppointmentSource.Fetch();

            //foreach (var appointment in a)
            //{
            //    Appointment ap = appointment;
            //    appointmentDetails.Text = "";
            //    appointmentDetails.Text += "-->" + ap.Subject + "\n" + ap.StartDate + "\n\n" + ap.Details;
            //}
        }
         
         */

    }
}
