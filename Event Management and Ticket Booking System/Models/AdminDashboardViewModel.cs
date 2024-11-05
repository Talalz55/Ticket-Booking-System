namespace Event_Management_and_Ticket_Booking_System.Models
{
    public class AdminDashboardViewModel
    {
        public List<User> Users { get; set; }
        public List<Event> Events { get; set; }

        // You can add more properties as needed for stats or additional functionality
        public int TotalUsers => Users?.Count ?? 0;
        public int TotalEvents => Events?.Count ?? 0;
    }
}
