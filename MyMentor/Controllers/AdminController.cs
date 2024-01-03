using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMentor.Data;
using MyMentor.Models;
using MyMentor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MyMentor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MyMentorDbContext _db;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(MyMentorDbContext dbContext, SignInManager<ApplicationUser> manager)
        {
            _db = dbContext;
            _signInManager = manager;
        }

        public ActionResult Index()
        {
            var users = (from u in _db.Users
                        join ur in _db.UserRoles on u.Id equals ur.UserId
                        join r in _db.Roles on ur.RoleId equals r.Id
                        orderby u.JoinDate
                        select new UserRole
                        {
                            User = u,
                            Role = r.Name
                        }).ToList();

            return View(users);
        }

        public ActionResult Profile(string id)
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

        [Route("Admin/Profile/Edit")]
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
                _db.SaveChanges();
                return RedirectToAction("Profile", new { user.Id });
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Courses()
        {
            var courses = _db.Courses.ToList();
            return View(courses);
        }

        public IActionResult Status(int id)
        {
            var target = _db.Courses.Find(id);
            target.Active = !target.Active;
            _db.SaveChanges();
            return RedirectToAction("Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(string courseName)
        {
            var course = new Course()
            {
                Name = courseName,
                Active = true
            };
            _db.Courses.Add(course);
            _db.SaveChanges();

            return RedirectToAction("Courses");
        }

        public IActionResult Appointments()
        {
            var history = _db.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Student)
                .Include(a => a.Course)
                .OrderByDescending(a => a.Date)
                .ToList();

            return View(history);
        }

        public IActionResult Ban(string id)
        {
            var user = _db.Users.Find(id);
            user.Active = !user.Active;
            if (user.Active)
                user.LockoutEnd = DateTimeOffset.MinValue;
            else
                user.LockoutEnd = DateTimeOffset.MaxValue;
            _db.SaveChanges();
            return RedirectToAction("Index");
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