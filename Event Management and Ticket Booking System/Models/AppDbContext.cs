using Event_Management_and_Ticket_Booking_System.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_and_Ticket_Booking_System.Models
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {

        // Constructor to accept DbContext options
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Role).IsRequired();
            });

            // Event configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.Venue).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Organizer)
                      .WithMany(u => u.OrganizedEvents)
                      .HasForeignKey(e => e.OrganizerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Ticket configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Type).IsRequired().HasMaxLength(50);
                entity.Property(t => t.Price).IsRequired();
                entity.HasOne(t => t.Event)
                      .WithMany(e => e.Tickets)
                      .HasForeignKey(t => t.EventId);
                entity.HasOne(t => t.Buyer)
                      .WithMany(u => u.PurchasedTickets)
                      .HasForeignKey(t => t.BuyerId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.TotalPrice).HasDefaultValue(0);
                entity.HasOne(c => c.User)
                      .WithOne()
                      .HasForeignKey<Cart>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CartItem configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.HasOne(ci => ci.Cart)
                      .WithMany(c => c.CartItems)
                      .HasForeignKey(ci => ci.CartId);
                entity.HasOne(ci => ci.Event)
                      .WithMany()
                      .HasForeignKey(ci => ci.EventId);
                entity.HasOne(ci => ci.Ticket)
                      .WithMany()
                      .HasForeignKey(ci => ci.TicketId);
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount).IsRequired();
                entity.Property(p => p.PaymentMethod).IsRequired().HasMaxLength(50);
                entity.Property(p => p.TransactionId).HasMaxLength(100);
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(p => p.Event)
                      .WithMany()
                      .HasForeignKey(p => p.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Review configuration
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Rating).IsRequired();
                entity.Property(r => r.Comment).HasMaxLength(500);
                entity.HasOne(r => r.Event)
                      .WithMany()
                      .HasForeignKey(r => r.EventId);
                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Message).IsRequired().HasMaxLength(250);
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // Configure TicketType as string in the database
            modelBuilder.Entity<Ticket>()
                .Property(t => t.Type)
                .HasConversion<string>();

            // Configure EventCategory as string in the database
            modelBuilder.Entity<Event>()
                .Property(e => e.Category)
                .HasConversion<string>();
        }

    }
}
