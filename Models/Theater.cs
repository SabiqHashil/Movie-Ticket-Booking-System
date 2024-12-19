namespace MovieTicketBookingSystem.Models
{
    public class Theater
    {
        public int TheaterID { get; set; }
        public string TheaterName { get; set; }
        public string Location { get; set; }
        public decimal ClassA_Rate { get; set; }
        public int ClassA_SeatCount { get; set; }
        public decimal ClassB_Rate { get; set; }
        public int ClassB_SeatCount { get; set; }
        public decimal ClassC_Rate { get; set; }
        public int ClassC_SeatCount { get; set; }

    }
}
