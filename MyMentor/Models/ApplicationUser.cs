using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyMentor.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Please enter first name")]
        [Column("First Name")]
        [StringLength(32)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]
        [Column("Last Name")]
        [StringLength(32)]
        public string LastName { get; set; }

        public string Birthdate { get; set; }

        public string Address { get; set; }

        public string Number { get; set; }

        [Column("Preferred Time")]
        public string PreferredTime { get; set; }

        [Column("Join Date")]
        public DateTime JoinDate { get; set; }

        public double Rating { get; set; }

        public string Education { get; set; }

        public string Biography { get; set; }

        [Column("Profile Picture")]
        public string ProfilePicture { get; set; }

        public bool Active { get; set; }
    }
}
