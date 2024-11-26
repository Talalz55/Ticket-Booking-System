namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public TicketType Type { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public int TicketId { get; set; }  // Foreign key to Ticket
        public Ticket Ticket { get; set; }  // Navigation property
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }

    public class Cart
    {
        public int Id { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserId { get; set; }  // Foreign key to User
        public User User { get; set; }  // Navigation property to User
    }
}
