using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DAL;
using MovieTicketBookingSystem.Models;
using CacheManager.Core;

namespace MovieTicketBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        // GET: SignUp
        public IActionResult SignUp()
        {
            return View();
        }
        // POST: SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(User model)
        {
            if (ModelState.IsValid)
            {
                bool isRegistered = DatabaseHelper.SignUpUser(
                    model.FirstName,
                    model.LastName,
                    model.DateOfBirth,
                    model.Gender,
                    model.PhoneNumber,
                    model.Email,
                    model.Address,
                    model.State,
                    model.City,
                    model.Username,
                    model.Password
                );
                if (isRegistered)
                {
                    return RedirectToAction("Signin", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                }
            }
            return View(model);
        }
        // GET: Signup
        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }
        // POST: Signin
        [HttpPost]
        public IActionResult Signin(User model, [FromServices] ICacheManager<string> cache)
        {
            if (ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid input. Please try again.");
                return View(model);
            }
            // Admin login
            if (model.Username == "admin" && model.Password == "admin")
            {
                HttpContext.Session.SetString("Username", model.Username);
                HttpContext.Session.SetString("Role", "Admin");
                TempData["Username"] = model.Username;
                return RedirectToAction("Dashboard", "Admin");
            }
            // User login
            var validUser = DatabaseHelper.LoginUser(model.Username, model.Password);
            if (validUser != null)
            {
                HttpContext.Session.SetString("Role", "User");
                HttpContext.Session.SetInt32("UserId", validUser.UserID);
                // Save UserId to CacheFactory
                cache.Put("UserId", validUser.UserID.ToString());
                TempData["UserID"] = validUser.UserID;
                TempData["Username"] = validUser.FirstName;
                return RedirectToAction("Dashboard", "User");
            }
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }
        // POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Signin", "Account");
        }
    }
}
