using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
        public class Event
        {
        public int Id { get; set; } // Primary key
        public string Title { get; set;}
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; } // Location or address
        public EventCategory Category { get; set; } // Enum for predefined categories
        public bool IsPublic { get; set; } // Visibility flag
        public decimal Price { get; set; } // Base price for tickets
        public int? TotalSeats { get; set; }
        public int? ReservedSeats { get; set; }  // Track how many seats are reserved
        [ForeignKey("Organizer")]
        public string? OrganizerId { get; set; } // Foreign key, now matches User.Id type
        public User Organizer { get; set; } // Navigation property
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();        
        public ICollection<Session> Sessions { get; set; } // One-to-many relationship
    }
    public enum EventCategory
    {
        Cultural,
        Sports,
        Educational,
        Corporate,
        Religious,
        Social,
        Political,
        Other
    }
    
}
