using Event_Management_and_Ticket_Booking_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace Event_Management_and_Ticket_Booking_System.Controllers
{
    public class EventController : Controller
    {
        private readonly EventDbContext _context;

        public EventController(EventDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Event model)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
