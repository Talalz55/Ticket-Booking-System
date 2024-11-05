using System.ComponentModel.DataAnnotations;

namespace Event_Management_and_Ticket_Booking_System.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsPublic { get; set; }
    }
}
