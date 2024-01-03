using MyMentor.Models;

namespace MyMentor.ViewModels
{
    public class MentorTeaches
    {
        public ApplicationUser Mentor { get; set; }
        public int Price { get; set; }
        public int CourseId { get; set; }
    }
}
