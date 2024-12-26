using System.Data;
using Microsoft.Data.SqlClient;
using MovieTicketBookingSystem.Models;


namespace MovieTicketBookingSystem.DAL
{
    public class DatabaseHelper
    {
        private readonly IConfiguration _configuration;
        public DatabaseHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }
        // GetConnectionString
        private string GetConnectionString()
        {
            try
            {
                return _configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving the connection string: " + ex.Message);
                throw;
            }
        }
        // SignUpUser
        public static bool SignUpUser(string firstName, string lastName, DateTime dob, string gender, string phoneNumber,
            string email, string address, string state, string city, string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_SignUp", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dob);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@State", state);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing the SignUp stored procedure: " + ex.Message);
                return false;
            }
        }
        // SignInUser
        public static User? LoginUser(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(new DatabaseHelper().GetConnectionString()))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_Login", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    Console.WriteLine($"Field {i}: {reader.GetName(i)} = {reader[i]}");
                                }
                            }
                        }
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Username = reader["Username"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while executing the Login stored procedure: " + ex.Message);
            }

            return null;
        }
        // Insert Movie
        public async Task<bool> InsertMovieAsync(Movie movie)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("sp_InsertMovie", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MovieName", movie.MovieName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Genre", movie.Genre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Duration", movie.Duration.HasValue ? movie.Duration.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Language", movie.Language ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", movie.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", movie.Image ?? (object)DBNull.Value);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error Code: {sqlEx.Number}");
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }
        // Get Movies
        public async Task<List<Movie>> GetMoviesAsync()
        {
            var movies = new List<Movie>();
            try
            {
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("sp_GetMovies", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                movies.Add(new Movie
                                {
                                    MovieID = reader.GetInt32(reader.GetOrdinal("MovieID")),
                                    MovieName = reader.GetString(reader.GetOrdinal("MovieName")),
                                    Genre = reader["Genre"] as string,
                                    Duration = reader["Duration"] as int?,
                                    Language = reader["Language"] as string,
                                    Description = reader["Description"] as string,
                                    Image = reader["Image"] as string
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            return movies;
        }
        // Get Movie By ID
        public async Task<Movie> GetMovieByIdAsync(int movieId)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("sp_GetMovieById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MovieID", movieId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    return new Movie
                    {
                        MovieID = (int)reader["MovieID"],
                        MovieName = reader["MovieName"].ToString(),
                        Genre = reader["Genre"].ToString(),
                        Duration = (int)reader["Duration"],
                        Language = reader["Language"].ToString(),
                        Description = reader["Description"].ToString(),
                        Image = reader["Image"].ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }
        // Update Movie
        public async Task<bool> UpdateMovieAsync(Movie movie)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("sp_UpdateMovie", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MovieID", movie.MovieID);
                cmd.Parameters.AddWithValue("@MovieName", movie.MovieName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Genre", movie.Genre ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Duration", movie.Duration.HasValue ? movie.Duration.Value : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Language", movie.Language ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", movie.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Image", movie.Image ?? (object)DBNull.Value);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error Code: {sqlEx.Number}");
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }
        // Delete Movie
        public async Task<bool> DeleteMovieByIdAsync(int movieId)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("sp_DeleteMovie", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MovieID", movieId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error Code: {sqlEx.Number}");
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }
        // Get All Movies
        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            var movies = new List<Movie>();
            try
            {
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand("GetAllMovies", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                movies.Add(new Movie
                                {
                                    MovieID = reader.GetInt32(reader.GetOrdinal("MovieID")),
                                    MovieName = reader.GetString(reader.GetOrdinal("MovieName")),
                                    Genre = reader["Genre"] as string,
                                    Duration = reader["Duration"] as int?,
                                    Language = reader["Language"] as string,
                                    Description = reader["Description"] as string,
                                    Image = reader["Image"] as string
                                });
                            }
                        }
                    }
               }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            return movies;
        }
        // Get All Theaters
        public async Task<List<Theater>> GetTheatersAsync()
        {
            var theaters = new List<Theater>();
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("GetAllTheaters", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    theaters.Add(new Theater
                    {
                        TheaterID = reader.GetInt32(0),
                        TheaterName = reader.GetString(1),
                        Location = reader.GetString(2),
                        ClassA_Rate = reader.GetDecimal(3),
                        ClassA_SeatCount = reader.GetInt32(4),
                        ClassB_Rate = reader.GetDecimal(5), 
                        ClassB_SeatCount = reader.GetInt32(6),
                        ClassC_Rate = reader.GetDecimal(7), 
                        ClassC_SeatCount = reader.GetInt32(8) 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            return theaters;
        }
        // Insert Theater
        public async Task<bool> InsertTheaterAsync(Theater theater)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("AddTheater", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TheaterName", theater.TheaterName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Location", theater.Location ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassA_Rate", theater.ClassA_Rate > 0 ? theater.ClassA_Rate : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassB_Rate", theater.ClassB_Rate > 0 ? theater.ClassB_Rate : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassC_Rate", theater.ClassC_Rate > 0 ? theater.ClassC_Rate : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassA_SeatCount", theater.ClassA_SeatCount > 0 ? theater.ClassA_SeatCount : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassB_SeatCount", theater.ClassB_SeatCount > 0 ? theater.ClassB_SeatCount : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassC_SeatCount", theater.ClassC_SeatCount > 0 ? theater.ClassC_SeatCount : (object)DBNull.Value);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error Code: {sqlEx.Number}");
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }
        // Get Theater By ID
        public async Task<Theater> GetTheaterByIdAsync(int theaterId)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("GetTheaterById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TheaterID", theaterId);
                using var reader = await cmd.ExecuteReaderAsync();
                if (reader.Read())
                {
                    return new Theater
                    {
                        TheaterID = (int)reader["TheaterID"],
                        TheaterName = reader["TheaterName"].ToString(),
                        Location = reader["Location"].ToString(),
                        ClassA_Rate = (decimal)reader["ClassA_Rate"],
                        ClassB_Rate = (decimal)reader["ClassB_Rate"],
                        ClassC_Rate = (decimal)reader["ClassC_Rate"],
                        ClassA_SeatCount = reader["ClassA_SeatCount"] as int? ?? 0,
                        ClassB_SeatCount = reader["ClassB_SeatCount"] as int? ?? 0,
                        ClassC_SeatCount = reader["ClassC_SeatCount"] as int? ?? 0
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }
        // Update Theater
        public async Task<bool> UpdateTheaterAsync(Theater theater)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("sp_UpdateTheater", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TheaterID", theater.TheaterID);
                cmd.Parameters.AddWithValue("@TheaterName", string.IsNullOrEmpty(theater.TheaterName) ? (object)DBNull.Value : theater.TheaterName);
                cmd.Parameters.AddWithValue("@Location", string.IsNullOrEmpty(theater.Location) ? (object)DBNull.Value : theater.Location);
                cmd.Parameters.AddWithValue("@ClassA_Rate", theater.ClassA_Rate);
                cmd.Parameters.AddWithValue("@ClassB_Rate", theater.ClassB_Rate);
                cmd.Parameters.AddWithValue("@ClassC_Rate", theater.ClassC_Rate);
                cmd.Parameters.AddWithValue("@ClassA_SeatCount", theater.ClassA_SeatCount > 0 ? (object)theater.ClassA_SeatCount : DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassB_SeatCount", theater.ClassB_SeatCount > 0 ? (object)theater.ClassB_SeatCount : DBNull.Value);
                cmd.Parameters.AddWithValue("@ClassC_SeatCount", theater.ClassC_SeatCount > 0 ? (object)theater.ClassC_SeatCount : DBNull.Value);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error Code: {sqlEx.Number}");
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }
        // Delete Theater
        public async Task<bool> DeleteTheaterByIdAsync(int theaterId)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("sp_DeleteTheater", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TheaterID", theaterId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error Code: {sqlEx.Number}");
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }
        // Get All Schedules (GET)
        public async Task<List<Schedule>> GetAllSchedulesAsync()
        {
            var schedules = new List<Schedule>();
            try
            {
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    await conn.OpenAsync();
                    using var cmd = new SqlCommand("ViewAllSchedules", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var movieImageBytes = reader["MovieImage"] as byte[];
                        string? base64Image = movieImageBytes != null
                            ? Convert.ToBase64String(movieImageBytes)
                            : null;
                        schedules.Add(new Schedule
                        {
                            ScheduleID = reader.GetInt32(reader.GetOrdinal("ScheduleID")),
                            MovieName = reader.GetString(reader.GetOrdinal("MovieName")),
                            MovieImage = base64Image,
                            TheaterName = reader.GetString(reader.GetOrdinal("TheaterName")),
                            Location = reader.GetString(reader.GetOrdinal("Location")),
                            Class = reader.GetString(reader.GetOrdinal("Class"))[0],
                            TotalSeatCount = reader.GetInt32(reader.GetOrdinal("TotalSeatCount")),
                            RemainingSeats = reader.GetInt32(reader.GetOrdinal("RemainingSeats")),
                            ShowTime = reader.GetTimeSpan(reader.GetOrdinal("ShowTime")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            TicketRate = reader.GetDecimal(reader.GetOrdinal("TicketRate")),
                        });
                    }
                }
                Console.WriteLine($"Schedules retrieved: {schedules.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
            }
            return schedules;
        }
        // Insert Schedule (POST)
        public async Task<bool> InsertScheduleAsync(Schedule schedule)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("InsertSchedule", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MovieID", schedule.MovieID);
                cmd.Parameters.AddWithValue("@TheaterID", schedule.TheaterID);
                cmd.Parameters.AddWithValue("@Class", schedule.Class);
                cmd.Parameters.AddWithValue("@ShowTime", schedule.ShowTime);
                cmd.Parameters.AddWithValue("@Date", schedule.Date);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error Code: {sqlEx.Number}");
                Console.WriteLine("SQL Error: " + sqlEx.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return false;
            }
        }
        // Get Schedule By ID (GET)
        public async Task<Schedule> GetScheduleByIDAsync(int scheduleID)
        {
            Schedule schedule = null;
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("GetScheduleByID", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ScheduleID", scheduleID);
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    schedule = new Schedule
                    {
                        ScheduleID = reader.GetInt32(reader.GetOrdinal("ScheduleID")),
                        MovieName = reader.GetString(reader.GetOrdinal("MovieName")),
                        TheaterName = reader.GetString(reader.GetOrdinal("TheaterName")),
                        Location = reader.GetString(reader.GetOrdinal("Location")),
                        Class = reader.GetString(reader.GetOrdinal("Class"))[0],
                        TotalSeatCount = reader.GetInt32(reader.GetOrdinal("TotalSeatCount")),
                        RemainingSeats = reader.GetInt32(reader.GetOrdinal("RemainingSeats")),
                        ShowTime = reader.GetTimeSpan(reader.GetOrdinal("ShowTime")),
                        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                        TicketRate = reader.GetDecimal(reader.GetOrdinal("ClassRate"))
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return schedule;
        }
        // Update Schedule (POST)
        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("UpdateSchedule", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ScheduleID", schedule.ScheduleID);
                cmd.Parameters.AddWithValue("@MovieID", schedule.MovieID);
                cmd.Parameters.AddWithValue("@TheaterID", schedule.TheaterID);
                cmd.Parameters.AddWithValue("@Class", schedule.Class);
                cmd.Parameters.AddWithValue("@ShowTime", schedule.ShowTime);
                cmd.Parameters.AddWithValue("@Date", schedule.Date);
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        // Delete Schedule (POST)
        public async Task<bool> DeleteScheduleAsync(int scheduleID)
        {
            try
            {
                using var conn = new SqlConnection(GetConnectionString());
                await conn.OpenAsync();
                using var cmd = new SqlCommand("DeleteSchedule", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ScheduleID", scheduleID);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        // List of Scheduled Movies for User Dashboard (GET)
        public List<Schedule> GetUserSchedules()
        {
            try
            {
                var schedules = new List<Schedule>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand("sp_UserShow", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var schedule = new Schedule
                                {
                                    MovieID = Convert.ToInt32(reader["MovieID"]),
                                    MovieName = reader["MovieName"].ToString(),
                                    MovieImage = reader["MovieImage"].ToString()
                                };

                                schedules.Add(schedule);
                            }
                        }
                    }
                }
                return schedules;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // Retrieves the full detail of a movie (GET)
        public Movie GetMovieDetails(int movieID)
        {
            try
            {
                Movie movie = null;
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand("sp_GetMovieDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MovieID", movieID);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                movie = new Movie
                                {
                                    MovieID = Convert.ToInt32(reader["MovieID"]),
                                    MovieName = reader["MovieName"].ToString(),
                                    Genre = reader["Genre"].ToString(),
                                    Duration = reader["Duration"] != DBNull.Value ? Convert.ToInt32(reader["Duration"]) : (int?)null,
                                    Language = reader["Language"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Image = reader["Image"].ToString()
                                };
                            }
                        }
                    }
                }
                return movie;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // Book Ticket Available Schedules (GET)
        public List<Schedule> GetAvailableSchedules(int movieID, int? scheduleID = null)
        {
            try
            {
                var schedules = new List<Schedule>();
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand command = new SqlCommand("sp_MovieBooking", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MovieID", movieID);
                        if (scheduleID.HasValue)
                        {
                            command.Parameters.AddWithValue("@ScheduleID", scheduleID.Value);
                        }
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var schedule = new Schedule
                                {
                                    ScheduleID = reader.GetInt32(reader.GetOrdinal("ScheduleID")),
                                    MovieName = reader.GetString(reader.GetOrdinal("MovieName")),
                                    TheaterName = reader.GetString(reader.GetOrdinal("TheaterName")),
                                    Location = reader.GetString(reader.GetOrdinal("Location")),
                                    Class = reader.GetString(reader.GetOrdinal("Class"))[0],
                                    TicketRate = Convert.ToDecimal(reader["ClassRate"]),
                                    ShowTime = reader.GetTimeSpan(reader.GetOrdinal("ShowTime")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    RemainingSeats = reader.GetInt32(reader.GetOrdinal("RemainingSeats"))
                                };

                                schedules.Add(schedule);
                            }
                        }
                    }
                }
                return schedules;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // PaymentDetails (GET)
        public PaymentTicket GetPaymentDetails(int scheduleId, int numberOfSeats)
        {
            try
            {
                if (numberOfSeats <= 0) throw new ArgumentException("Number of seats must be greater than zero.");
                PaymentTicket ticket = null;
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetPaymentDetails", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ScheduleID", scheduleId);
                        cmd.Parameters.AddWithValue("@NumberOfSeats", numberOfSeats);
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ticket = new PaymentTicket
                                {
                                    ScheduleID = Convert.ToInt32(reader["ScheduleID"]),
                                    MovieName = reader["MovieName"].ToString(),
                                    TheaterName = reader["TheaterName"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    SeatClass = reader["SeatClass"].ToString(),
                                    ClassRate = Convert.ToDecimal(reader["ClassRate"]),
                                    NumberOfSeats = Convert.ToInt32(reader["NumberOfSeats"]),
                                    TotalPriceWithGST = Convert.ToDecimal(reader["TotalPriceWithGST"])
                                };
                            }
                        }
                    }
                }
                return ticket;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // Insert Ticket (POST)
        public string InsertTicket(Ticket ticket)
        {
            try
            {
                string ticketNumber = string.Empty;
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertTicket", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ScheduleID", ticket.ScheduleID);
                        cmd.Parameters.AddWithValue("@UserID", ticket.UserID);
                        cmd.Parameters.AddWithValue("@SeatClass", ticket.SeatClass);
                        cmd.Parameters.AddWithValue("@NumberOfSeats", ticket.NumberOfSeats);
                        cmd.Parameters.AddWithValue("@TotalPrice", ticket.TotalPrice);

                        SqlParameter ticketNumberParam = new SqlParameter("@TicketNumber", SqlDbType.NVarChar, 50)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(ticketNumberParam);
                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            ticketNumber = ticketNumberParam.Value.ToString();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error while inserting the ticket: " + ex.Message);
                        }
                    }
                }
                return ticketNumber;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // User Tickets (GET)
        public List<UserTicket> GetUserTickets(int userId)
        {
            try
            {
                var tickets = new List<UserTicket>();
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    using (var command = new SqlCommand("sp_GetUserTickets", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", userId);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(new UserTicket
                                {
                                    TicketID = (int)reader["TicketID"],
                                    TicketNumber = reader["TicketNumber"].ToString(),
                                    SeatClass = reader["SeatClass"].ToString(),
                                    NumberOfSeats = (int)reader["NumberOfSeats"],
                                    TotalPrice = (decimal)reader["TotalPrice"],
                                    BookingDate = (DateTime)reader["BookingDate"],
                                    MovieName = reader["MovieName"].ToString(),
                                    Genre = reader["Genre"].ToString(),
                                    Language = reader["Language"].ToString(),
                                    Duration = (int)reader["Duration"],
                                    TheaterName = reader["TheaterName"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    ShowTime = (TimeSpan)reader["ShowTime"],
                                    Date = (DateTime)reader["Date"]
                                });
                            }
                        }
                    }
                }
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
        // Get All Tickets for ADMIN (GET)
        public List<UserTicket> GetAllBookedTickets()
        {
            try
            {
                var tickets = new List<UserTicket>();
                using (var connection = new SqlConnection(GetConnectionString()))
                {
                    using (var command = new SqlCommand("sp_GetAllBookedTickets", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(new UserTicket
                                {
                                    TicketID = (int)reader["TicketID"],
                                    UserID = (int)reader["UserID"],
                                    UserName = reader["UserName"].ToString(),
                                    TicketNumber = reader["TicketNumber"].ToString(),
                                    SeatClass = reader["SeatClass"].ToString(),
                                    ClassRate = (decimal)reader["ClassRate"],
                                    NumberOfSeats = (int)reader["NumberOfSeats"],
                                    TotalPrice = (decimal)reader["TotalPrice"],
                                    BookingDate = (DateTime)reader["BookingDate"],
                                    MovieName = reader["MovieName"].ToString(),
                                    Language = reader["Language"].ToString(),
                                    TheaterName = reader["TheaterName"].ToString(),
                                    Location = reader["Location"].ToString(),
                                    ShowTime = (TimeSpan)reader["ShowTime"],
                                    Date = (DateTime)reader["Date"]
                                });
                            }
                        }
                    }
                }
                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
    }
}
