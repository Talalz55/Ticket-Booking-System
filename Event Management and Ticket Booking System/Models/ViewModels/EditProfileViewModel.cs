using System.ComponentModel.DataAnnotations;

namespace Event_Management_and_Ticket_Booking_System.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }
    }
}
