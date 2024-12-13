namespace Event_Management_and_Ticket_Booking_System.Models;
using System.Text.Json;

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            WriteIndented = true // Optional: For better readability of JSON
        };
        session.SetString(key, JsonSerializer.Serialize(value, options));
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        if (value == null)
        {
            return default;
        }

        var options = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };
        return JsonSerializer.Deserialize<T>(value, options);
    }
}
