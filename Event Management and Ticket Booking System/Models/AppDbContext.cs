using Event_Management_and_Ticket_Booking_System.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_and_Ticket_Booking_System.Models
{
    public class AppDbContext: IdentityDbContext<User>
    {
        // Constructor to accept DbContext options
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets for each model
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        // Configure model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
            .HasOne(e => e.Organizer) // Each Event has one Organizer
            .WithMany(u => u.CreatedEvents) // Each User can create multiple Events
            .HasForeignKey(e => e.OrganizerId) // Foreign key in Event
            .IsRequired(); // This makes the relationship required

           

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.Tickets) // Assuming User has PurchasedTickets
                .HasForeignKey(t => t.BuyerId)
                .IsRequired(false); // BuyerId is nullable
                                    // Cart - User relationship
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId);

            // CartItem - Ticket relationship
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Ticket)
                .WithMany()
                .HasForeignKey(ci => ci.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem - Cart relationship
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
