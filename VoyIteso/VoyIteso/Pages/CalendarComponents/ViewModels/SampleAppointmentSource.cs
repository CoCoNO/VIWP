
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace VoyIteso.Pages.ViewModels
{
    public class SampleAppointmentSource : AppointmentSource
    {
        public SampleAppointmentSource()
        {

        }

        public override void FetchData(DateTime startDate, DateTime endDate)
        {
            this.AllAppointments.Clear();

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(1),
                Subject = "Appointment 1",
                AdditionalInfo = "Info 1"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddMinutes(30),
                EndDate = DateTime.Now.AddHours(1),
                Subject = "Appointment 2",
                AdditionalInfo = "Info 2"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddMinutes(30),
                EndDate = DateTime.Now.AddHours(1),
                Subject = "Appointment 3",
                AdditionalInfo = "Info 3"

            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddHours(2),
                EndDate = DateTime.Now.AddHours(3),
                Subject = "Appointment 4",
                AdditionalInfo = "Info 4"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(1).AddHours(1),
                Subject = "Check scores",
                AdditionalInfo = "Info"
            });
            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(3),
                Subject = "Long run",
                AdditionalInfo = "10 miles"
            });
            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.Date.AddDays(6),
                EndDate = DateTime.Now.Date.AddDays(7),
                Subject = "Long run",
                AdditionalInfo = "15 miles"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddDays(4),
                EndDate = DateTime.Now.AddDays(4).AddHours(1),
                Subject = "Go to cinema",
                AdditionalInfo = "Choose a movie first"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(-1).AddHours(1),
                Subject = "Wash the car",
                AdditionalInfo = "If necessary"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(7).AddHours(1),
                Subject = "Sail the boat"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(7).AddHours(1),
                Subject = "Feed the fish"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddDays(8),
                EndDate = DateTime.Now.AddDays(8).AddHours(1),
                Subject = "Go to ski"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddMonths(1),
                EndDate = DateTime.Now.AddMonths(1).AddHours(12),
                Subject = "Go fishing"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddMonths(1),
                EndDate = DateTime.Now.AddMonths(1).AddHours(12),
                Subject = "Go to MIX"
            });

            this.AllAppointments.Add(new SampleAppointment()
            {
                StartDate = DateTime.Now.AddMonths(1),
                EndDate = DateTime.Now.AddMonths(1).AddHours(12),
                Subject = "Visit Kauai"
            });

            this.OnDataLoaded();
        }
    }

    public class SampleAppointment : IAppointment
    {
        public DateTime EndDate
        {
            get;
            set;
        }

        public DateTime StartDate
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string AdditionalInfo
        {
            get;
            set;
        }
    }
}
