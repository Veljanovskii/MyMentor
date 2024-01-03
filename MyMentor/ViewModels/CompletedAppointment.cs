using MyMentor.Models;

namespace MyMentor.ViewModels
{
    public class CompletedAppointment
    {
        public Appointment Appointment { get; set; }
        public Rating Rating { get; set; }
        public bool FirstTime { get; set; }
    }
}
