using System;
using System.Linq;
using System.Collections.Generic;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;


namespace WeddingPlanner.Controllers     //be sure to use your own project's namespace!
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]     //Http Method and the route
        public IActionResult Index() //When in doubt, use IActionResult
        {
            return View("Index");//or whatever you want to return
        }

        [HttpPost("/RegisterNewUser")]
        public IActionResult RegisterNewUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");

                    // You may consider returning to the View at this point
                    return RedirectToAction("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToAction("Dashboard", new { id = user.UserId });
            }
            return View("Index");
        }
        [HttpPost("LoginUser")]
        public IActionResult LoginUser(Login login)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == login.LoginEmail);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<Login>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(login, userInDb.Password, login.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View("Index");
                }
                else
                {
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                    return RedirectToAction("Dashboard", new { id = userInDb.UserId });
                }
            }
            return View("Index");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            DashboardWrapper LoggedInThing = new DashboardWrapper();
            LoggedInThing.LoggedInUser = _context.Users.FirstOrDefault(LoggedInUser => LoggedInUser.UserId == (int)UserId);
            LoggedInThing.AllWeddings = _context.Weddings
                .Include(g => g.GuestList)
                .Include(g => g.WeddingPlanner)
                .ToList();
            return View("Dashboard", LoggedInThing);
        }
        [HttpGet("weddings/new")]
        public IActionResult NewWedding()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            return View("NewWedding");
        }
        [HttpPost("weddings/new")]
        public IActionResult CreateWedding(Wedding form)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                Wedding HappiestDayEver = new Wedding();
                HappiestDayEver = form;
                HappiestDayEver.WeddingPlanner = _context.Users
                    .FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserId"));
                _context.Weddings.Add(HappiestDayEver);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
        [HttpGet("weddings/{id}")]
        public IActionResult OneWedding(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            Wedding HappiestDayEver = _context.Weddings
                .Include(g => g.GuestList)
                .ThenInclude(p => p.Attendee)
                .FirstOrDefault(w => w.WeddingId == id);
            return View("OneWedding", HappiestDayEver);
        }
        [HttpGet("weddings/{id}/delete")]
        public IActionResult DeleteWedding(int id)
        {
            Wedding WhoCheated = _context.Weddings
                .FirstOrDefault(w => w.WeddingId == id);
            _context.Weddings.Remove(WhoCheated);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet("weddings/{id}/AddGuest")]
        public IActionResult AddGuest(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                Guest Uninvited = new Guest();
                Uninvited.UserId = (int)HttpContext.Session.GetInt32("UserId");
                Uninvited.WeddingId = id;
                _context.Guests.Add(Uninvited);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
        [HttpPost("weddings/{id}/RemoveGuest")]
        public IActionResult RemoveGuest(int id)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if (UserId == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                Guest Uninvited = _context.Guests
                    .FirstOrDefault(g => g.WeddingId == id && g.UserId == (int)UserId);
                _context.Guests.Remove(Uninvited);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}