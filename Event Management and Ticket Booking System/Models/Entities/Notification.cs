namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to User
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public ApplicationUser User { get; set; }
    }
}
