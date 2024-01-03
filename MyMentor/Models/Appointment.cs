using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyMentor.Models
{
    [Table("Appointment")]
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        public ApplicationUser Student { get; set; }

        public ApplicationUser Mentor { get; set; }

        public Course Course { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy.}")]
        public DateTime Date { get; set; }

        [Column("Start Time")]
        public int StartTime { get; set; }

        public Status Status { get; set; }
    }
    public enum Status
    {
        Pending,
        Confirmed,
        Denied,
        Completed
    }

}
