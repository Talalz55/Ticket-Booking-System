using Event_Management_and_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management_and_Ticket_Booking_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly EventDbContext _context;

        public AdminController(EventDbContext context)
        {
            _context = context;
        }

        // View for the dashboard
        public IActionResult Dashboard()
        {
            var users = _context.Users.ToList();
            var events = _context.Events.ToList();
            var viewModel = new AdminDashboardViewModel
            {
                Users = users,
                Events = events
            };
            return View(viewModel);
        }

        // Manage events (CRUD)
        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            var eventItem = _context.Events.Find(id);
            if (eventItem == null) return NotFound();
            return View(eventItem);
        }

        [HttpPost]
        public IActionResult EditEvent(Event model)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        public IActionResult DeleteEvent(int id)
        {
            var eventItem = _context.Events.Find(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
    }
}
