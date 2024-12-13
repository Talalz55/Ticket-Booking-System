namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int EventId { get; set; } // Foreign key to Event
        public string UserId { get; set; } // Foreign key to User
        public int Rating { get; set; } // 1 to 5 stars
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Event Event { get; set; }
        public ApplicationUser User { get; set; }
    }
}
