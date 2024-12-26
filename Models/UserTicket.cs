namespace MovieTicketBookingSystem.Models
{
    public class UserTicket
    {
        public int TicketID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string TicketNumber { get; set; }
        public string SeatClass { get; set; }
        public decimal ClassRate { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public string MovieName { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }
        public int Duration { get; set; }
        public string TheaterName { get; set; }
        public string Location { get; set; }
        public TimeSpan ShowTime { get; set; }
        public DateTime Date { get; set; }
    }
}
