using Event_Management_and_Ticket_Booking_System.Models;
using Event_Management_and_Ticket_Booking_System.Models.Entities;
using Event_Management_and_Ticket_Booking_System.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Event_Management_and_Ticket_Booking_System.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;
        public BookingController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Booking/Event/{eventId}
        [HttpGet]
        public async Task<IActionResult> Event(int eventId)
        {
            Console.WriteLine($"Received eventId: {eventId}");
            var eventDetails = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventDetails == null)
            {
                return RedirectToAction("Index");
                return NotFound();
            }

            // Get available tickets (unreserved)
            var availableTickets = eventDetails.Tickets.Where(t => !t.IsReserved).ToList();
            return View(availableTickets); // Pass the available tickets to the view
        }
        // POST: Booking/AddToCart/{eventId}
        [HttpPost]
        public async Task<IActionResult> AddToCart(int eventId, TicketType ticketType, int quantity)
        {
            var eventDetails = await _context.Events
                .Include(e => e.Tickets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventDetails == null)
            {
                return NotFound();
            }

            var availableTicket = eventDetails.Tickets
                .FirstOrDefault(t => t.Type == ticketType && !t.IsReserved);

            if (availableTicket == null)
            {
                // Handle case where seats are unavailable or insufficient
                return View("Error", new ErrorViewModel { RequestId = "Not enough tickets available." });
            }

            var cart = GetUserCart();  // Get or create a cart for the user (session-based or database-based)
            var cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.EventId == eventId && ci.Type == ticketType);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.TotalPrice = cartItem.Quantity * availableTicket.Price;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    EventId = eventId,
                    Type = ticketType,
                    Quantity = quantity,
                    TotalPrice = quantity * availableTicket.Price
                });
            }

            await _context.SaveChangesAsync(); // Save the cart state (optional if using in-memory session cart)

            return RedirectToAction("Cart");
        }

        // GET: Booking/Cart
        public IActionResult Cart()
        {
            var cart = GetUserCart();  // Get the current user's cart
            return View(cart); // Pass the cart to the view
        }

        // POST: Booking/Checkout
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var cart = GetUserCart();

            // Process the checkout, update ticket availability, etc.
            foreach (var item in cart.CartItems)
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.EventId == item.EventId && t.Type == item.Type && !t.IsReserved);

                if (ticket != null)
                {
                    ticket.IsReserved = true;  // Mark ticket as reserved
                }
            }

            // After checkout, you might want to clear the cart and confirm the booking to the user.
            await _context.SaveChangesAsync();
            return RedirectToAction("Confirmation"); // Redirect to a confirmation page
        }

        public async Task<IActionResult> Confirmation()
        {
            return View();
        }

        private Cart GetUserCart()
        {
            // Get the current user's ID from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User not logged in.");
            }

            // Retrieve the user's cart, including the associated CartItems
            var userCart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (userCart == null)
            {
                // Optionally, create a new cart for the user if it doesn't exist
                userCart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(userCart);
                _context.SaveChanges();
            }

            return userCart;
        }
    }
}
