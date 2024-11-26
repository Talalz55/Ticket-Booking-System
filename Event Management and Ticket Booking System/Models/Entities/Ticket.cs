namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; } // Primary key

        public string TicketNumber { get; set; } // Unique identifier, required

        public decimal Price { get; set; } // Required

        public TicketType Type { get; set; } // E.g., "Regular", "VIP", required

        public DateTime? PurchaseDate { get; set; } // Nullable, if a ticket hasn’t been purchased yet
        public bool IsReserved { get; set; }  // True if booked, false otherwise
        public int Quantity { get; set; }

        // Foreign key to Event
        public int EventId { get; set; } // Required
        public Event? Event { get; set; } // Nullable navigation property, optional to load

        // Foreign key to User (Buyer)
        public string? BuyerId { get; set; } // Nullable, if a ticket hasn’t been assigned to a buyer yet
        public User? Buyer { get; set; } // Nullable navigation property, optional to load

        /*public string? QRCodePath { get; set; } // Nullable, if QR code hasn’t been generated yet*/

    }

    public enum TicketType
    {
        Regular,
        VIP
    }
}
