using System.ComponentModel.DataAnnotations;

namespace MovieTicketBookingSystem.Models
{
    public class Movie
    {
        [Key]
        public int MovieID { get; set; }

        [Required, StringLength(255)]
        public string MovieName { get; set; }

        [StringLength(100)]
        public string Genre { get; set; }

        public int? Duration { get; set; }

        [StringLength(50)]
        public string Language { get; set; }

        public string Description { get; set; }

        public string Image { get; set; } 
        public IFormFile UploadedImage { get; set; }
    }
}
