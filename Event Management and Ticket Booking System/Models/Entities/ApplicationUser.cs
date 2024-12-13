using Microsoft.AspNetCore.Identity;
using System.Net.Sockets;

namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } // Admin, Organizer, or Attendee
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Event> OrganizedEvents { get; set; }
        public ICollection<Ticket> PurchasedTickets { get; set; }
    }
}
