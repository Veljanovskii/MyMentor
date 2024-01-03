using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyMentor.Models
{
    [Table("Teaches")]
    public class Teaches
    {
        [Key]
        public int Id { get; set; }

        public ApplicationUser Mentor { get; set; }

        public Course Course { get; set; }

        public int Price { get; set; }
    }
}
