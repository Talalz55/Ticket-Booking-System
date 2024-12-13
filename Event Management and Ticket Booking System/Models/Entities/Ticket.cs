using System.Text.Json.Serialization;

namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; } // Foreign key to Event
        public string? BuyerId { get; set; } // Foreign key to User (if purchased)
        public TicketType Type { get; set; } // E.g., Regular, VIP
        public decimal Price { get; set; }
        public bool IsReserved { get; set; } = false;

        // Navigation Properties
        [JsonIgnore]
        public Event Event { get; set; }
        public ApplicationUser? Buyer { get; set; }

    }

    public enum TicketType
    {
        Regular,
        VIP
    }
}
