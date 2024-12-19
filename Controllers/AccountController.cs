using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DAL;
using MovieTicketBookingSystem.Models;

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
                    TempData["SuccessMessage"] = "Account successfully created! You can now log in.";
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
        public IActionResult Signin(User model)
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
                TempData["Username"] = model.Username; // Pass data to the next request
                return RedirectToAction("Dashboard", "Admin");
            }

            // User login
            var validUser = DatabaseHelper.LoginUser(model.Username, model.Password);
            if (validUser != null)
            {
                HttpContext.Session.SetString("Username", validUser.FirstName);
                HttpContext.Session.SetString("Role", "User");
                TempData["Username"] = validUser.FirstName; // Pass data to the next request
                //return View(validUser.FirstName);
                return RedirectToAction("Dashboard", "User", validUser.FirstName);
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
