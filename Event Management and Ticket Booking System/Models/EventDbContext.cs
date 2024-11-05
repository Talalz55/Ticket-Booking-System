using Microsoft.EntityFrameworkCore;

namespace Event_Management_and_Ticket_Booking_System.Models
{
    public class EventDbContext: DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options): base(options)
        {
             
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
}
}
