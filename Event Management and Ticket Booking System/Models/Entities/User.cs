using Microsoft.AspNetCore.Identity;
using System.Net.Sockets;

namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; } // "Admin", "Organizer", "Attendee"
        public ICollection<Event> CreatedEvents { get; set; } // For Organizers
        public ICollection<Ticket> Tickets { get; set; } // For Attendees
        // One-to-One relationship with Cart
        public Cart Cart { get; set; }
    }
}
