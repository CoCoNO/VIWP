﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace VoyIteso.Pages
{
    public partial class Calendar : PhoneApplicationPage
    {
        public Calendar()
        {
            InitializeComponent();

            

            MyAppointmentSource myAppointmentSource = new MyAppointmentSource();

            myAppointmentSource.clearAllAppointments();
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
            });
            myAppointmentSource.onDataLoaded();

            // add appointment source to calendar
            myCalendar.AppointmentSource = myAppointmentSource;

            var a = myAppointmentSource.getAppointmentList();
            //myAppointmentSource.Fetch();
            
            appointmentDetails.Text = "";


        }

        private void myCalendar_ItemTap(object sender, CalendarItemTapEventArgs e)
        {
            //appointmentDetails.Text = this.myCalendar.SelectedValue.ToString();
        }

        private void myCalendar_SelectedValueChanged(object sender, ValueChangedEventArgs<object> e)
        {
            DateTime? selectedDate = this.myCalendar.SelectedValue;
            appointmentDetails.Text = selectedDate.ToString() ;
            if (e.NewValue == null)
            {
                
            }
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