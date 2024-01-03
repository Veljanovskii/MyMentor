using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyMentor.Models;
using MyMentor.Data;
using Microsoft.EntityFrameworkCore;
using MyMentor.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MyMentor.Controllers
{
    [Authorize(Roles = "Mentor")]
    public class MentorController : Controller
    {
        private readonly MyMentorDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MentorController(MyMentorDbContext _dbcontext, SignInManager<ApplicationUser> manager)
        {
            _db = _dbcontext;
            _signInManager = manager;
        }

        public IActionResult Index()
        {
            var myAppointments = _db.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.Mentor.UserName == User.Identity.Name)
                .Where(a => a.Status != Status.Denied)
                .Where(a => a.Status != Status.Completed)
                .OrderBy(a => a.Date)
                .ToList();
            
            return View(myAppointments);
        }

        public IActionResult Appointments()
        {
            var history = _db.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.Mentor.UserName == User.Identity.Name)
                .Where(a => a.Status != Status.Pending)
                .Where(a => a.Status != Status.Confirmed)
                .OrderByDescending(a => a.Date)
                .ToList();

            return View(history);
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

        [Route("Mentor/Profile/Edit")]
        public ActionResult Edit()
        {
            var profile = _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            return View(profile);
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
                user.Biography = p.Biography;
                user.PreferredTime = p.PreferredTime;
                _db.SaveChanges();
                return RedirectToAction("Profile", new { user.Id });
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Confirm(int id)
        {
            var target = _db.Appointments.Find(id);
            target.Status = Status.Confirmed;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Deny(int id)
        {
            var target = _db.Appointments.Find(id);
            target.Status = Status.Denied;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Courses()
        {
            var user = _db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var myCoures = _db.Teaches
                .Include(t => t.Course)
                .Include(t => t.Mentor)
                .Where(t => t.Mentor == user && t.Course.Active)
                .Select(t => t.Course)
                .ToList();
            var newCoures = _db.Courses
                .Where(c => c.Active && !myCoures.Contains(c))
                .ToList();
            var courses = new List<List<Course>>();
            courses.Add(myCoures);
            courses.Add(newCoures);
            return View(courses);
        }

        [Route("Mentor/Courses/Modify")]
        public IActionResult Modify(int id)
        {
            var course = _db.Courses.Find(id);
            if (course == null || !course.Active)
            {
                return new NotFoundResult();
            }

            var user = _db.Users
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefault();

            var teaches = _db.Teaches
                .Include(t => t.Course)
                .Include(t => t.Mentor)
                .Where(t => t.Mentor == user && t.Course == course)
                .FirstOrDefault();

            if (teaches == null)
            {
                teaches = new Teaches()
                {
                    Mentor = user,
                    Course = course,
                    Price = 0
                };
            }

            return View(teaches);
        }

        public IActionResult DeleteTeaches(int id)
        {
            var teaches = _db.Teaches.Find(id);
            _db.Teaches.Remove(teaches);
            _db.SaveChanges();
            return RedirectToAction("Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTeaches([FromForm] Teaches cursed)
        {
            var course = _db.Courses.Find(cursed.Id);
            var mentor = _db.Users.Where(m => m.UserName == User.Identity.Name).FirstOrDefault();
            var teaches = _db.Teaches
                .Include(t => t.Mentor)
                .Include(t => t.Course)
                .Where(t => t.Course == course && t.Mentor == mentor)
                .FirstOrDefault();
            teaches.Price = cursed.Price;
            _db.SaveChanges();
            return RedirectToAction("Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTeaches([FromForm] Teaches t)
        {
            var mentor = _db.Users.Find(t.Mentor.Id);
            var course = _db.Courses.Find(t.Course.Id);
            var teaches = new Teaches()
            {
                Mentor = mentor,
                Course = course,
                Price = t.Price
            };
            _db.Teaches.Add(teaches);
            _db.SaveChanges();
            return RedirectToAction("Courses");
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