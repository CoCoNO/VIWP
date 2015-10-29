﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
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
        }






        //<summary>
        //This method gets the appointments from the server and load them to the calendar.
        //</summary>
        private async void InitAppointments()
        {
            MyAppointmentSource myAppointmentSource = new MyAppointmentSource();
            myAppointmentSource.clearAllAppointments();

            //Appointment[] apps = await ApiConnector.Instance.LoadCurrentMonthLifts();//a lift is an appointment
            Appointment[] apps = HomePage.apps;
            //await ApiConnector.Instance.LoadCurrentMonthLifts();
            //await ApiConnector.Instance.LoadCurrentMonthLifts();
                ////a lift is an appointment
            
            foreach (var appointment in apps)
            {
                myAppointmentSource.addAppointment(appointment);
                Days.Add(appointment.StartDate);
            }
            myAppointmentSource.onDataLoaded();

            // add appointment source to calendar
            MyCalendar.AppointmentSource = myAppointmentSource;
            
        }


        public void foo()
        {
            //Appointment[] apps = HomePage.apps;//a lift is an appointment
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

                
                //AppointmentDetails.Text = "";
                ListaDeApointments.Items.Clear();

                foreach (var item in list)
                {
                    //build a new listboxitem.
                    var listItem = new ListBoxItem();
                    listItem.Content = "";
                    listItem.FontSize = 13;

                    string exampleString = item.Details;
                    string[] words = exampleString.Split('*');
                    if (item.StartDate.Day == selectedDate.Day)
                    {
                        listItem.Content += words[0];
                        listItem.Content += "\n" + words[1];
                        listItem.Content += "\n" + words[2];
                        //AppointmentDetails.Text += words[0];
                        //AppointmentDetails.Text += "\n" + words[1];
                        //AppointmentDetails.Text += "\n" + words[2];
                        listItem.Tap += item_onClicked;
                    }
                    ListaDeApointments.Items.Add(listItem);
                }
            }

            //appointmentDetails.Text = selectedDate.ToString() ;
            //if (e.NewValue == null)
            //{
            //    return;
            //}

        }

        private void item_onClicked(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
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

        private void MyCalendar_OnDisplayDateChanged(object sender, ValueChangedEventArgs<object> e)
        {
            Debug.WriteLine("date changed");
            //InitAppointments();
        }


        private void MyCalendar_OnDisplayDateChanging(object sender, ValueChangingEventArgs<object> e)
        {
            InitAppointments();
        }
    }
}
