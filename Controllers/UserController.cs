using System.Data;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MovieTicketBookingSystem.DAL;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseHelper dbHelper;

        public UserController(DatabaseHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }
        // User Dashboard (GET)
        public IActionResult Dashboard()
        {
            var schedules = dbHelper.GetUserSchedules();
            var username = User.Identity.IsAuthenticated ? User.Identity.Name : "Guest";

            ViewData["Title"] = "User Dashboard";
            ViewData["Schedules"] = schedules;
            ViewData["Username"] = username;

            return View();
        }
        // View Movie Details (GET)
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
        // Movie Details (GET)
        public IActionResult MovieDetails(int id)
        {
            var movie = dbHelper.GetMovieDetails(id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }
        // GET: User/BookTicket
        public IActionResult BookTicket(int movieID, int? scheduleID = null)
        {
            // Retrieve the available schedules for booking
            var schedules = dbHelper.GetAvailableSchedules(movieID, scheduleID);
            var model = new Ticket
            {
                Schedules = schedules
            };

            return View(model);
        }
        // GET: Payment Details
        public IActionResult PaymentTicket(int scheduleId, int numberOfSeats)
        {
            if(numberOfSeats <= 0)
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
        // POST: Insert Ticket
        [HttpPost]
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

                var ticket = new Ticket
                {
                    ScheduleID = ScheduleID,
                    //UserID = GetLoggedInUserId(), // Replace with logic for logged-in user ID
                    UserID = 12,
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



        private int GetUserIdFromSession()
        {
            // This is a placeholder. Implement session-based user retrieval logic.
            return 1;  // Assume user ID is 1 for demonstration
        }

    }
}
