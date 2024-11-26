namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Speaker { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
