using Event_Management_and_Ticket_Booking_System.Models;
using Event_Management_and_Ticket_Booking_System.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Runtime.ConstrainedExecution;
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
        // GET: /Event
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search, EventCategory? category, string sortOrder)
        {
            var events = await _context.Events
        .OrderBy(e => e.Name)
        .ToListAsync();

            return View(events);
        }

        // GET: /Event/Details/{id}
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var eventDetails = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (eventDetails == null)
            {
                return NotFound();
            }

            return View(eventDetails);
        }

        // GET: /Event/Create
        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Event/Create
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create(Event model)
        {
            if (model != null)
            {
                model.AvailableSeats = model.TotalSeats;
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                model.OrganizerId = userId;

                _context.Events.Add(model);
                await _context.SaveChangesAsync();


                var ticket = new Ticket
                {
                    EventId = model.Id, // Use the event ID from the saved model
                    Type = TicketType.Regular, // Default ticket type; adjust as needed
                    Price = model.BasePrice,
                    IsReserved = false // Tickets are not reserved initially
                };
                /*// Generate tickets for the event
                var tickets = new List<Ticket>();
                for (int i = 0; i < model.TotalSeats; i++)
                {
                    tickets.Add(new Ticket
                    {
                        EventId = model.Id, // Use the event ID from the saved model
                        Type = TicketType.Regular, // Default ticket type; adjust as needed
                        Price = model.BasePrice,
                        IsReserved = false // Tickets are not reserved initially
                    });
                }

                _context.Tickets.AddRange(tickets);*/
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync(); // Save tickets to the database
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction("Index");
            }
            // Log validation errors for debugging
            foreach (var modelStateKey in ModelState.Keys)
            {
                var errors = ModelState[modelStateKey]?.Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                }
            }

            return View(model);
        }

        // GET: /Event/Edit/{id}
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(int id)
        {
            var eventToEdit = await _context.Events.FindAsync(id);
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (eventToEdit == null || eventToEdit.OrganizerId != userId)
            {
                return Unauthorized();
            }

            return View(eventToEdit);
        }

        // POST: /Event/Edit/{id}
        [HttpPost]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(int id, Event model)
        {
            /*if (id != model.Id)
            {
                return BadRequest();
            }*/

            if (ModelState.IsValid)
            {
                var eventToUpdate = await _context.Events.FindAsync(id);
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (eventToUpdate == null || eventToUpdate.OrganizerId != userId)
                {
                    return Unauthorized();
                }

                eventToUpdate.Name = model.Name;
                eventToUpdate.Description = model.Description;
                eventToUpdate.Date = model.Date;
                eventToUpdate.Venue = model.Venue;
                eventToUpdate.Category = model.Category;

                _context.Update(eventToUpdate);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyEvents");
            }

            return View(model);
        }

        // GET: /Event/Delete/{id}
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);

            if (eventToDelete == null || eventToDelete.OrganizerId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            {
                return Unauthorized();
            }

            return View(eventToDelete);
        }

        // POST: /Event/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);

            if (eventToDelete == null || eventToDelete.OrganizerId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
            {
                return Unauthorized();
            }

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyEvents");
        }

        // GET: /Event/MyEvents
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> MyEvents()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var myEvents = await _context.Events
                .Where(e => e.OrganizerId == userId)
                .ToListAsync();

            return View(myEvents);
        }
    }
}
