using Event_Management_and_Ticket_Booking_System.Models;
using Event_Management_and_Ticket_Booking_System.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Event_Management_and_Ticket_Booking_System.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        public IActionResult Add(int eventId, int quantity = 1)
        {
            if (eventId <= 0 || quantity <= 0)
            {
                TempData["ErrorMessage"] = "Invalid event or quantity.";
                return RedirectToAction("Index");
            }

            var eventDetails = _context.Events.Include(e => e.Tickets).FirstOrDefault(e => e.Id == eventId);
            if (eventDetails == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("Index", "Event");
            }

            if (eventDetails.AvailableSeats < quantity)
            {
                TempData["ErrorMessage"] = "Not enough seats available.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            var cart = GetCart();

            // Add tickets to the cart
            var ticket = eventDetails.Tickets.FirstOrDefault(t => !t.IsReserved);
            if (ticket == null)
            {
                TempData["ErrorMessage"] = "No available tickets for this event.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.EventId == eventId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.TotalPrice = cartItem.Quantity * ticket.Price;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    EventId = eventId,
                    TicketId = ticket.Id, // Store the ticket ID for reference
                    Quantity = quantity,
                    TotalPrice = quantity * ticket.Price,
                    Ticket = ticket
                });
            }

            eventDetails.AvailableSeats -= quantity; // Reduce available seats
            _context.Update(eventDetails);
            SaveCart(cart);

            TempData["SuccessMessage"] = $"{quantity} tickets added to your cart.";
            return RedirectToAction("Details", "Event", new { id = eventId });
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            var cart = GetCart();
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == cartItem.TicketId);
            if (ticket == null)
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;
            cartItem.TotalPrice = cartItem.Quantity * ticket.Price;
            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public IActionResult Remove(int cartItemId)
        {
            var cart = GetCart();
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }

            cart.CartItems.Remove(cartItem);
            SaveCart(cart);

            return RedirectToAction("Index");
        }

        // Helper: Get the cart (session-based)
        private Cart GetCart()
        {
            var cart = HttpContext.Session.Get<Cart>("Cart");
            if (cart == null)
            {
                cart = new Cart { CartItems = new List<CartItem>() };
                SaveCart(cart);
            }
            else
            {
                // Eagerly load Event for each Ticket in the cart
                foreach (var cartItem in cart.CartItems)
                {
                    var ticket = _context.Tickets.Include(t => t.Event).FirstOrDefault(t => t.Id == cartItem.TicketId);
                    if (ticket != null)
                    {
                        cartItem.Ticket = ticket;
                    }
                }
            }
            return cart;
        }

        // Helper: Save the cart to the session
        private void SaveCart(Cart cart)
        {
            HttpContext.Session.Set("Cart", cart);
        }

        // Helper: Clear the cart
        private void ClearCart()
        {
            HttpContext.Session.Remove("Cart");
        }


        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            /*return RedirectToAction("Index", "Event");*/
            var cart = GetCart();
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty!";
                return RedirectToAction("Index", "Event");
            }

            foreach (var cartItem in cart.CartItems)
            {
                var ticket = await _context.Tickets.Include(t => t.Event).FirstOrDefaultAsync(t => t.Id == cartItem.TicketId);

                if (ticket == null)
                {
                    TempData["ErrorMessage"] = $"The ticket with ID {cartItem.TicketId} was not found.";
                    return RedirectToAction("Index");
                }

                // Check availability
                var eventDetails = ticket.Event;
                if (eventDetails.AvailableSeats < cartItem.Quantity)
                {
                    TempData["ErrorMessage"] = $"Not enough seats available for the event '{eventDetails.Name}'.";
                    return RedirectToAction("Index");
                }
                

                // Create tickets for the user
                for (int i = 0; i < cartItem.Quantity; i++)
                {
                    _context.Tickets.Add(new Ticket
                    {
                        EventId = ticket.EventId,
                        BuyerId = userId,
                        Type = ticket.Type,
                        Price = ticket.Price,
                        IsReserved = true
                    });
                }

                // Deduct available seats
                eventDetails.AvailableSeats -= cartItem.Quantity;
                _context.Update(eventDetails);                
            }

            await _context.SaveChangesAsync();
            ClearCart();

            TempData["SuccessMessage"] = "Checkout successful! Your tickets have been reserved.";
            return RedirectToAction("Index", "Event");
        }

    }
}
