
namespace MovieTicketBookingSystem.Models
{
    public class Ticket
    {
        public int TicketID { get; set; } 
        public int ScheduleID { get; set; }  
        public int UserID { get; set; }  
        public char SeatClass { get; set; }  
        public int NumberOfSeats { get; set; }  
        public decimal TotalPrice { get; set; }  
        public DateTime BookingDate { get; set; }  
        // Navigation properties for related data 
        public Schedule Schedule { get; set; }  
        public User User { get; set; }  
        public List<Schedule> Schedules { get; internal set; }
    }
}
