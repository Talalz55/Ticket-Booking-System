using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace Event_Management_and_Ticket_Booking_System.Models.Entities
{
        public class Event
        {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event Name is required.")]
        [StringLength(100, ErrorMessage = "Event Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public EventCategory Category { get; set; } // Enum

        [Required(ErrorMessage = "Venue is required.")]
        [StringLength(200, ErrorMessage = "Venue cannot exceed 200 characters.")]
        public string Venue { get; set; }

        [Required(ErrorMessage = "Event Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Base Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base Price must be greater than 0.")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Total Seats is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Total Seats must be at least 1.")]
        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; } // Automatically calculated in controller

        // Navigation Properties
        public string? OrganizerId { get; set; }
        public ApplicationUser? Organizer { get; set; }
        [JsonIgnore]
        public ICollection<Ticket>? Tickets { get; set; }
    }
    public enum EventCategory
    {
        Cultural,
        Sports,
        Educational,
        Corporate,
        Religious,
        Social,
        Political,
        Other
    }
    
}
