using Microsoft.AspNetCore.Identity;

namespace Event_Management_and_Ticket_Booking_System.Models
{
    public static class SeedDB
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Organizer", "Attendee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
