using Microsoft.AspNetCore.Identity;

namespace Event_Management_and_Ticket_Booking_System.Models
{
    public class User : IdentityUser
    {
        // Additional properties for user details can be added as needed
        public string FullName { get; set; }
        public ICollection<Event> RegisteredEvents { get; set; }
    }
}
