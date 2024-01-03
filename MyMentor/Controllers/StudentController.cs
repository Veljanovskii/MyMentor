using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyMentor.Data;
using MyMentor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyMentor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MyMentor.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly MyMentorDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public StudentController(MyMentorDbContext db, SignInManager<ApplicationUser> manager)
        {
            _db = db;
            _signInManager = manager;
        }

        public IActionResult Index()
        {
            var courses = _db.Courses.Select(a => a).Where(a => a.Active).ToList();
            return View(courses);
        }

        public IActionResult Course(int id)
        {
            var mentors = (from t in _db.Teaches
                           from m in _db.Users
                           where t.Mentor == m
                           && t.Course.Id == id
                           && m.Active
                           orderby m.Rating descending
                           select new MentorTeaches()
                           {
                               Mentor = m,
                               Price = t.Price,
                               CourseId = id
                           }).ToList();
            return View(mentors);
        }

        public IActionResult Appointments()
        {
            var history = _db.Appointments
                .Include(a => a.Student)
                .Include(a => a.Mentor)
                .Include(a => a.Course)
                .Where(a => a.Student.UserName == User.Identity.Name)
                .OrderByDescending(a => a.Date)
                .ToList();

            return View(history);
        }

        [Route("Student/Appointment/{id}")]
        public IActionResult Appointment(int id)
        {
            var app = _db.Appointments
                .Include(s => s.Student)
                .Include(s => s.Mentor)
                .Include(s => s.Course)
                .Where(s => s.Id == id).FirstOrDefault();

            var review = _db.Ratings
                .Include(r => r.Student)
                .Include(r => r.Mentor)
                .Where(r => r.Student == app.Student && r.Mentor == app.Mentor)
                .FirstOrDefault();

            var viewModel = new CompletedAppointment()
            {
                Appointment = app,
                Rating = new Rating(),
                FirstTime = review == null
            };

            return View(viewModel);
        }

        [Route("Student/Course/New")]
        public IActionResult New(int id, string m)
        {
            var mentor = _db.Users.Find(m);
            var course = _db.Courses.Find(id);
            var student = _db.Users.Where(s => s.UserName == User.Identity.Name).FirstOrDefault();

            var app = new Appointment()
            {
                Course = course,
                Mentor = mentor,
                Student = student,
                Date = DateTime.Today.AddDays(1)
            };

            return View(app);
        }

        public IActionResult Profile(string id)
        {
            var profile = _db.Users.Find(id);
            var comments = _db.Ratings
                .Include(r => r.Mentor)
                .Include(r => r.Student)
                .Where(r => r.Mentor == profile)
                .ToList();
            var profileWithComments = new ProfileComments()
            {
                Profile = profile,
                Comments = comments
            };

            return View(profileWithComments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save([FromForm] ApplicationUser p)
        {
            try
            {
                var user = _db.Users.Find(p.Id);
                user.Birthdate = p.Birthdate;
                user.Number = p.Number;
                user.Education = p.Education;
                user.Address = p.Address;
                _db.SaveChanges();
                return RedirectToAction("Profile", new { user.Id });
            }
            catch
            {
                return View();
            }
        }

        [Route("Student/Profile/Edit")]
        public ActionResult Edit()
        {
            var profile = _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Make([FromForm] Appointment p)
        {
            var mentor = _db.Users.Find(p.Mentor.Id);
            var student = _db.Users.Find(p.Student.Id);
            var course = _db.Courses.Find(p.Course.Id);
            var app = new Appointment()
            {
                Course = course,
                Mentor = mentor,
                Student = student,
                Date = p.Date,
                StartTime = p.StartTime,
                Status = Status.Pending
            };
            _db.Appointments.Add(app);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rate([FromForm] CompletedAppointment ca)
        {
            var app = _db.Appointments
                .Include(app => app.Mentor)
                .Include(app => app.Student)
                .Where(app => app.Id == ca.Appointment.Id)
                .FirstOrDefault();

            var rating = new Rating()
            {
                Student = app.Student,
                Mentor = app.Mentor,
                Score = ca.Rating.Score,
                Comment = ca.Rating.Comment
            };

            var count = _db.Ratings.Include(r => r.Mentor).Where(r => r.Mentor == app.Mentor).Count();
            var mentor = rating.Mentor;
            mentor.Rating = (mentor.Rating * count + rating.Score) / (count + 1);

            _db.Ratings.Add(rating);
            _db.SaveChanges();

            return RedirectToAction("Appointments");
        }

        public IActionResult Cancel(int id)
        {
            var app = _db.Appointments.Find(id);
            app.Status = Status.Denied;
            _db.SaveChanges();

            return RedirectToAction("Appointments");
        }

        public IActionResult Settings()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(IFormFile file)
        {
            var user = _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                var homePath = "wwwroot\\img\\pfp\\";

                var split = file.FileName.Split(".");
                var extension = split[^1];
                var fileName = DateTime.Now.Ticks - DateTime.Parse("12.09.2020. 08:24:20").Ticks;
                var newName = fileName + "." + extension;

                var path = homePath + newName;
                var stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
                var old = user.ProfilePicture;
                user.ProfilePicture = newName;
                _db.SaveChanges();
                if (old != "default.jpg")
                {
                    var info = new FileInfo(homePath + old);
                    info.Delete();
                }
            }

            return RedirectToAction("Profile", new { user.Id });
        }

        public async Task<IActionResult> SelfBan()
        {
            var user = _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            user.Active = false;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            _db.SaveChanges();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}