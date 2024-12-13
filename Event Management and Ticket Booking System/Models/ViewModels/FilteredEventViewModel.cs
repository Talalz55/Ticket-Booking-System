using Event_Management_and_Ticket_Booking_System.Models.Entities;

namespace Event_Management_and_Ticket_Booking_System.Models.ViewModels
{
    public class FilteredEventViewModel
    {
        public IEnumerable<Event> Events { get; set; }
        public string Search { get; set; }
        public EventCategory? SelectedCategory { get; set; }
        public string SortOrder { get; set; }
        public IEnumerable<EventCategory> Categories => Enum.GetValues(typeof(EventCategory)).Cast<EventCategory>();
    }
}
