using MyMentor.Models;
using System.Collections.Generic;

namespace MyMentor.ViewModels
{
    public class ProfileComments
    {
        public ApplicationUser Profile { get; set; }
        public List<Rating> Comments { get; set; }
    }
}
