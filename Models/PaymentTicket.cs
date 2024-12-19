namespace MovieTicketBookingSystem.Models
{
    public class PaymentTicket
    {
        public int ScheduleID { get; set; }
        public string MovieName { get; set; }
        public string TheaterName { get; set; }
        public string Location { get; set; }
        public string SeatClass { get; set; }
        public decimal ClassRate { get; set; }
        public int NumberOfSeats { get; set; }
        public int GST { get; set; }
        public decimal TotalPriceWithGST { get; set; }
    }
}
