using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMentor.Models;

namespace MyMentor.Data
{
    public class MyMentorDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyMentorDbContext(DbContextOptions<MyMentorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Teaches> Teaches { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Appointment>().HasOne(s => s.Student);
            builder.Entity<Appointment>().HasOne(s => s.Mentor);
            builder.Entity<Appointment>().HasOne(s => s.Course);

            builder.Entity<Rating>().HasOne(s => s.Student);
            builder.Entity<Rating>().HasOne(s => s.Mentor);

            builder.Entity<Teaches>().HasOne(s => s.Mentor);
            builder.Entity<Teaches>().HasOne(s => s.Course);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
