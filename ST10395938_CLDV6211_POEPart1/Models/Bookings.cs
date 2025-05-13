using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10395938_CLDV6211_POEPart1.Models
{
    public class Bookings
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        [Key]
        public int BookingId { get; set; }

        
        public int EventId { get; set; }
        
        [ForeignKey("EventId")]
        public Events? Events { get; set; }

       
        public int VenueId { get; set; }
 
        [ForeignKey("VenueId")]
        public Venues? Venues { get; set; }

        public DateTime BookingDate { get; set; }
    }
}
