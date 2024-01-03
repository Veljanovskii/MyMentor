using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyMentor.Models
{
    [Table("Rating")]
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        public ApplicationUser Student { get; set; }

        public ApplicationUser Mentor { get; set; }

        public int Score { get; set; }

        [StringLength(100)]
        public string Comment { get; set; }
    }
}
