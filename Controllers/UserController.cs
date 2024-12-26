using System.Data;
using CacheManager.Core;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DAL;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseHelper dbHelper;
        private readonly ICacheManager<string> cache;
        // Inject DatabaseHelper and CacheManager via constructor
        public UserController(DatabaseHelper dbHelper, ICacheManager<string> cache)
        {
            this.dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }
        // GET: User Dashboard
        public IActionResult Dashboard()
        {
            var schedules = dbHelper.GetUserSchedules();
            var username = User.Identity.IsAuthenticated ? User.Identity.Name : "Guest";
            ViewData["Title"] = "User Dashboard";
            ViewData["Schedules"] = schedules;
            ViewData["Username"] = username;
            return View();
        }
        // GET: View Movie Details
        public async Task<IActionResult> ViewMovie(int id)
        {
            var movie = await dbHelper.GetMovieByIdAsync(id);
            var schedule = await dbHelper.GetScheduleByIDAsync(id);
            if (movie == null || schedule == null)
            {
                return NotFound();
            }
            ViewData["Title"] = "View Movie";
            ViewData["Movie"] = movie;
            ViewData["Schedule"] = schedule;
            return View(movie);
        }
        // GET:  Movie Details
        public IActionResult MovieDetails(int id)
        {
            var movie = dbHelper.GetMovieDetails(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
        // GET: Book Ticket
        public IActionResult BookTicket(int movieID, int? scheduleID = null)
        {
            try
            {
                if (movieID <= 0)
                {
                    ViewBag.ErrorMessage = "Invalid Movie ID.";
                    return View("Error");
                }
                var schedules = dbHelper.GetAvailableSchedules(movieID, scheduleID);
                if (schedules == null || !schedules.Any())
                {
                    schedules = new List<Schedule>();
                    ViewBag.ErrorMessage = "No available schedules for this movie.";
                }
                var model = new Ticket
                {
                    Schedules = schedules
                };
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while loading schedules: " + ex.Message;
                return View("Error");
            }
        }
        // POST: Payment Ticket
        public IActionResult PaymentTicket(int scheduleId, int numberOfSeats)
        {
            try
            {
                if (numberOfSeats <= 0)
                {
                    ModelState.AddModelError("", "Number of seats must be greater than zero.");
                    return RedirectToAction("BookTicket");
                }
                var paymentDetails = dbHelper.GetPaymentDetails(scheduleId, numberOfSeats);
                if (paymentDetails == null)
                {
                    return NotFound("No payment details found for the selected schedule.");
                }
                return View(paymentDetails);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                throw;
            }
        }
        // POST: Insert Ticket
        public IActionResult ConfirmBooking(int ScheduleID, int NumberOfSeats, decimal TotalPriceWithGST, char SeatClass)
        {
            try
            {
                if (NumberOfSeats <= 0)
                {
                    throw new ArgumentException("Number of seats must be greater than zero.");
                }
                if (!new[] { 'A', 'B', 'C' }.Contains(SeatClass))
                {
                    throw new ArgumentException("Invalid seat class. Must be 'A', 'B', or 'C'.");
                }
                // Retrieve the logged-in user's ID from the cache
                var userId = cache.Get<string>("UserId");
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    throw new UnauthorizedAccessException("User is not logged in.");
                }
                var ticket = new Ticket
                {
                    ScheduleID = ScheduleID,
                    UserID = parsedUserId,
                    SeatClass = SeatClass,
                    NumberOfSeats = NumberOfSeats,
                    TotalPrice = TotalPriceWithGST
                };
                string ticketNumber = dbHelper.InsertTicket(ticket);
                if (string.IsNullOrEmpty(ticketNumber))
                {
                    throw new Exception("Failed to generate ticket number.");
                }
                PaymentTicket paymentDetails = dbHelper.GetPaymentDetails(ScheduleID, NumberOfSeats);
                if (paymentDetails == null)
                {
                    throw new Exception("Failed to retrieve payment details from the database.");
                }
                ViewBag.TicketNumber = ticketNumber;
                ViewBag.Message = "Movie Ticket Booked Successfully!";
                return PartialView("BookingConfirmation", paymentDetails);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return PartialView("BookTicket");
            }
        }
        // GET: User Tickets
        public IActionResult UserTickets()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Signin", "Account");
            }
            var tickets = dbHelper.GetUserTickets(userId.Value);
            return View(tickets);
        }
    }
}
