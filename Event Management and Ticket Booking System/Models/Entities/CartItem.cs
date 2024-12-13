namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; } // Foreign key to Cart
        public int EventId { get; set; } // Foreign key to Event
        public int TicketId { get; set; } // Foreign key to Ticket
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation Properties
        public Cart Cart { get; set; }
        public Event Event { get; set; }
        public Ticket Ticket { get; set; }
    }

    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Foreign key to User
        public decimal TotalPrice { get; set; }

        // Navigation Properties
        public ApplicationUser User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
