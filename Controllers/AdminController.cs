using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBookingSystem.DAL;
using MovieTicketBookingSystem.Helper;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseHelper dbHelper;

        // AdminController
        public AdminController()
        {
            dbHelper = new DatabaseHelper();
        }
        // Admin Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }
        // Movie List View (GET)
        public async Task<IActionResult> Movies()
        {
            var movies = await dbHelper.GetMoviesAsync();
            return View(movies);
        }
        // Add Movie View (GET)
        public IActionResult AddMovie()
        {
            return View();
        }
        // Add Movie Data (POST)
        [HttpPost]
        public async Task<IActionResult> AddMovie(Movie movie)
        {
            if (ModelState.IsValid)
            {
                return View(movie);
            }

            try
            {
                if (movie.UploadedImage != null && movie.UploadedImage.Length > 0)
                {
                    //var lastMovieId = await dbHelper.GetLastMovieIdAsync();
                    using var memoryStream = new MemoryStream();
                    await movie.UploadedImage.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    movie.Image = Convert.ToBase64String(imageBytes);

                    // Save movie to database
                    var isInserted = await dbHelper.InsertMovieAsync(movie);
                    if (isInserted)
                    {
                        return RedirectToAction("Movies");
                    }
                    ModelState.AddModelError("", "Failed to add movie.");
                }
                else
                {
                    ModelState.AddModelError("UploadedImage", "Please upload a valid image file.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            return View(movie);
        }
        // Update Movie View (GET)
        public async Task<IActionResult> UpdateMovie(int id)
        {
            try
            {
                var movie = await dbHelper.GetMovieByIdAsync(id);
                if (movie != null)
                {
                    return View(movie);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                return RedirectToAction("Movies");
            }
        }
        // Update Movie Data (POST)
        [HttpPost]
        public async Task<IActionResult> UpdateMovie(Movie movie)
        {
            var tempMoviedata = await dbHelper.GetMovieByIdAsync(movie.MovieID);
            if (!ModelState.IsValid)
            {
                try
                {
                    if (movie.UploadedImage != null && movie.UploadedImage.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await movie.UploadedImage.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        movie.Image = Convert.ToBase64String(imageBytes);
                    }
                    else if (tempMoviedata != null)
                    {
                        movie.Image = tempMoviedata.Image;
                    }

                    var isUpdated = await dbHelper.UpdateMovieAsync(movie);
                    if (isUpdated)
                    {
                        return RedirectToAction("Movies");
                    }

                    ModelState.AddModelError("", "Failed to update movie.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            return View(movie);
        }
        // Delete Movie (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteMovieConfirmed(int id)
        {
            try
            {
                var isDeleted = await dbHelper.DeleteMovieByIdAsync(id);
                if (isDeleted)
                {
                    TempData["AlertMessage"] = "Data Deleted Successfully...";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction("Movies");
                }
                ModelState.AddModelError("", "Failed to delete movie.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            return RedirectToAction("Movies");
        }
        // Theater List View (GET)
        public async Task<IActionResult> Theaters()
        {
            var theaters = await dbHelper.GetTheatersAsync();
            return View(theaters);
        }
        // Add Theater View (GET)
        public IActionResult AddTheater()
        {
            return View();
        }
        // Insert Theater (POST)
        [HttpPost]
        public async Task<IActionResult> AddTheater(Theater model)
        {
            if (ModelState.IsValid)
            {
                var isAdded = await dbHelper.InsertTheaterAsync(model);
                if (isAdded)
                {
                    TempData["SuccessMessage"] = "Theater added successfully!";
                    return RedirectToAction("Theaters");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add the theater.");
                }
            }
            return View(model);
        }
        // Update Theater View (GET)
        public async Task<IActionResult> UpdateTheater(int id)
        {
            try
            {
                var movie = await dbHelper.GetTheaterByIdAsync(id);
                if (movie != null)
                {
                    return View(movie);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                return RedirectToAction("Theaters");
            }
        }
        // Update Theater (POST)
        [HttpPost]
        public async Task<IActionResult> UpdateTheater(Theater theater)
        {
            var tempTheaterData = await dbHelper.GetTheaterByIdAsync(theater.TheaterID);

            if (ModelState.IsValid)
            {
                try
                {
                    var isUpdated = await dbHelper.UpdateTheaterAsync(theater);

                    if (isUpdated)
                    {
                        return RedirectToAction("Theaters"); 
                    }

                    ModelState.AddModelError("", "Failed to update theater.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            return View(theater);  
        }
        // Delete Theater (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteTheaterConfirmed(int id)
        {
            try
            {
                var isDeleted = await dbHelper.DeleteTheaterByIdAsync(id);
                if (isDeleted)
                {
                    TempData["AlertMessage"] = "Data Deleted Successfully...";
                    TempData["AlertType"] = "danger";
                    return RedirectToAction("Theaters");
                }
                ModelState.AddModelError("", "Failed to delete movie.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
            }

            return RedirectToAction("Theaters");
        }
        // Schedules List View (GET)
        public async Task<IActionResult> Schedules()
        {
            var schedules = await dbHelper.GetAllSchedulesAsync();
            if (!(schedules != null && schedules.Count != 0))
            {
                Console.WriteLine("No schedules found.");
            }
            else
            {
                Console.WriteLine($"Found {schedules.Count} schedules.");
            }

            return View(schedules);
        }
        // Insert Schedule View (GET)
        public async Task<IActionResult> AddSchedule()
        {
            try
            {
                var movies = await dbHelper.GetAllMoviesAsync();
                var theaters = await dbHelper.GetTheatersAsync();

                ViewBag.Movies = movies;
                ViewBag.Theaters = theaters;

                return View(new Schedule());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return View("Error");
            }
        }
        // Insert Schedule (POST)
        [HttpPost]
        public async Task<IActionResult> AddSchedule(Schedule model)
        {
            if (!ModelState.IsValid)
            {
                var isAdded = await dbHelper.InsertScheduleAsync(model);
                if (isAdded)
                {
                    TempData["SuccessMessage"] = "Schedule added successfully!";
                    return RedirectToAction("Schedules");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to add the schedule.");
                }
            }
            return View(model);
        }
        // Edit Schedule (GET)
        public async Task<IActionResult> UpdateSchedule(int id)
        {
            var schedule = await dbHelper.GetScheduleByIDAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            ViewBag.Movies = await dbHelper.GetMoviesAsync();
            ViewBag.Theaters = await dbHelper.GetTheatersAsync();

            return View(schedule);
        }
        // Edit Schedule (POST)
        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(int id, Schedule schedule)
        {
            schedule.ScheduleID = id;
            if (!ModelState.IsValid)
            {
                try
                {
                    if (DateTime.TryParse(schedule.ShowTime.ToString(), out DateTime parsedTime))
                    {
                        schedule.ShowTime = parsedTime.TimeOfDay;
                    }
                    else
                    {
                        ModelState.AddModelError("ShowTime", "Invalid time format.");
                        return View(schedule);
                    }

                    var isUpdated = await dbHelper.UpdateScheduleAsync(schedule);
                    if (isUpdated)
                    {
                        TempData["SuccessMessage"] = "Schedule updated successfully!";
                        return RedirectToAction("Schedules");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update the schedule.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the schedule: " + ex.Message);
                }
            }

            ViewBag.Movies = await dbHelper.GetMoviesAsync();
            ViewBag.Theaters = await dbHelper.GetTheatersAsync();
            return View(schedule);
        }
        // Delete Schedule (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var isDeleted = await dbHelper.DeleteScheduleAsync(id);
            if (isDeleted)
            {
                return RedirectToAction("Schedules");
            }
            return BadRequest();
        }



    }
}
