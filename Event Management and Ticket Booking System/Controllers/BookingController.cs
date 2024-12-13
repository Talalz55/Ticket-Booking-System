using Event_Management_and_Ticket_Booking_System.Models.Entities;
using Event_Management_and_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event_Management_and_Ticket_Booking_System.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Booking/Book/5
        [Authorize] // Ensure only logged-in users can book
        public async Task<IActionResult> Book(int id)
        {
            var eventDetails = await _context.Events.FindAsync(id);

            if (eventDetails == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("Index", "Event");
            }

            if (eventDetails.AvailableSeats <= 0)
            {
                TempData["ErrorMessage"] = "No seats available for this event.";
                return RedirectToAction("Details", "Event", new { id });
            }

            return View(eventDetails); // Show booking details
        }

        // POST: /Booking/Confirm
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Confirm(int eventId, int quantity)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Invalid quantity selected.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            var eventDetails = await _context.Events.Include(e => e.Tickets).FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventDetails == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("Index", "Event");
            }

            if (eventDetails.AvailableSeats < quantity)
            {
                TempData["ErrorMessage"] = $"Only {eventDetails.AvailableSeats} seats are available.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            // Create tickets for the user
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            for (int i = 0; i < quantity; i++)
            {
                _context.Tickets.Add(new Ticket
                {
                    EventId = eventDetails.Id,
                    BuyerId = userId,
                    Type = TicketType.Regular, // You can enhance this with a ticket type selection
                    Price = eventDetails.BasePrice,
                    IsReserved = true
                });
            }

            // Deduct available seats
            eventDetails.AvailableSeats -= quantity;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tickets booked successfully!";
            return RedirectToAction("MyBookings");
        }

        // GET: /Booking/MyBookings
        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var tickets = await _context.Tickets
                .Include(t => t.Event)
                .Where(t => t.BuyerId == userId)
                .ToListAsync();

            return View(tickets); // Pass tickets to the view
        }
    }
}
