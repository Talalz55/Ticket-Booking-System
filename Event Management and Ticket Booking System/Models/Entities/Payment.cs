namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to User
        public int EventId { get; set; } // Foreign key to Event
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // E.g., Credit Card, PayPal
        public string TransactionId { get; set; } // ID from payment provider
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ApplicationUser User { get; set; }
        public Event Event { get; set; }
    }
}
