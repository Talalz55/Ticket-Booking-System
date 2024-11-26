using Event_Management_and_Ticket_Booking_System.Models;
using Event_Management_and_Ticket_Booking_System.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Event_Management_and_Ticket_Booking_System.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var events = _context.Events.Include(e => e.Organizer).ToList();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Populate categories for the dropdown list
            ViewBag.Categories = Enum.GetValues(typeof(EventCategory)).Cast<EventCategory>().ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                                      .ToList();
            ViewBag.Errors = errors;
            /*if (!ModelState.IsValid)
                return View(model);*/


            model.OrganizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Add the event to the database
            _context.Events.Add(model);
            await _context.SaveChangesAsync();


            // Generate tickets for the event
            var tickets = new List<Ticket>();
            for (int i = 1; i <= model.TotalSeats; i++)
            {
                tickets.Add(new Ticket
                {
                    EventId = model.Id,
                    TicketNumber = $"Seat-{i}",
                    Type = TicketType.Regular // Default type, can be customized
                });
            }

            // Save tickets to the database
            _context.Tickets.AddRange(tickets);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

       /* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                // Set the organizer (logged-in user)
                @event.OrganizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirect to event listing
            }

            // If the model state is not valid, show the form again
            ViewBag.Categories = Enum.GetValues(typeof(EventCategory)).Cast<EventCategory>().ToList();
            return View(@event);
        }*/
        public IActionResult Calendar()
        {
            var events = _context.Events.Select(e => new {
                e.Title,
                Start = e.Date.ToString("yyyy-MM-dd HH:mm:ss"),
                End = e.Date.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss") // assuming 1 hour duration
            }).ToList();

            var eventsJson = JsonConvert.SerializeObject(events);
            ViewData["EventsJson"] = eventsJson;

            return View();
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Organizer) // Include related data if necessary
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [Authorize] // Optional: Add authorization if you want to restrict this to only logged-in users
        public async Task<IActionResult> Edit(int id)
        {
            var eventToEdit = await _context.Events.FindAsync(id);

            if (eventToEdit == null)
            {
                return NotFound();
            }

            return View(eventToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Optional: Add authorization if needed
        public async Task<IActionResult> Edit(int id, Event updatedEvent)
        {
            if (id != updatedEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(updatedEvent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to Index or any other page after successful update
            }

            return View(updatedEvent);

        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        // GET: Event/Delete/{id}
        [Authorize] // Optional: Add authorization if you want to restrict access
        public async Task<IActionResult> Delete(int id)
        {
            var eventToDelete = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventToDelete == null)
            {
                return NotFound();
            }

            return View(eventToDelete);
        }

        // POST: Event/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize] // Optional: Add authorization if needed
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventToDelete = await _context.Events
                .FindAsync(id);

            if (eventToDelete == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to Index or another page after deletion
        }
    }
}
