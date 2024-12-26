namespace MovieTicketBookingSystem.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int MovieID { get; set; }
        public int TheaterID { get; set; }
        public char Class { get; set; }
        public TimeSpan ShowTime { get; set; }
        public DateTime Date { get; set; }
        // Navigation Properties for display purposes
        public string MovieName { get; set; }
        public string MovieImage { get; set; }
        public string TheaterName { get; set; }
        public string Location { get; set; }
        public int TotalSeatCount { get; set; }
        public int RemainingSeats { get; set; }
        public decimal TicketRate { get; set; }
    }
}
