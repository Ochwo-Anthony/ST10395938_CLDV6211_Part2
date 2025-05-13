using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10395938_CLDV6211_POEPart1.Models
{
    public class Venues
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "Venue name is required")]
        public string VenueName { get; set; }

        [Required(ErrorMessage = "Venue location is required")]
        public string VenueLocation { get; set; }

        [Required(ErrorMessage = "Venue capacity is required")]
        public int VenueCapacity { get; set; }


        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public List<Events> Events { get; set; } = new();

        public List<Bookings> Bookings { get; set; } = new();

    }
}
