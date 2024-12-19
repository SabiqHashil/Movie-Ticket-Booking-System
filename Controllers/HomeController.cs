using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieTicketBookingSystem.DAL;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly DatabaseHelper dbHelper;

        public HomeController(ILogger<HomeController> logger, DatabaseHelper dbHelper)
        {
            logger = logger;
            this.dbHelper = dbHelper;
        }
        // Index View Movie Data
        public async Task<IActionResult> Index()
        {
            try
            {
                // Fetch all movies
                var movies = await dbHelper.GetAllMoviesAsync();

                //ViewData["Movies"] = movies;

                return View(movies);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching movies for Index.");
                return RedirectToAction("Error");
            }
        }
        // AboutUs View
        public IActionResult AboutUs()
        {
            return View();
        }
        //ContactUs View
        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
