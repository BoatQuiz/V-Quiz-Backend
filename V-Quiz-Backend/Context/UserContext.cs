using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Context
{
    public static class UserContext
    {
        public static Guid? TryGetUserId(HttpRequestData req)
        {
            var cookie = req.Cookies.FirstOrDefault(c => c.Name == "user_identity");
            if (cookie == null)
                return null;

            try
            {
                // 1. URL-dekoda
                var decoded = Uri.UnescapeDataString(cookie.Value);

                // 2. OM det är JSON-in-string → packa upp
                if (decoded.StartsWith("\""))
                {
                    decoded = JsonSerializer.Deserialize<string>(decoded);
                }

                // 3. NU är decoded alltid riktig JSON
                var identity = JsonSerializer.Deserialize<UserIdentity>(decoded);

                // 4. Returnera Guid
                return identity != null && Guid.TryParse(identity.UserId, out var userId)
                    ? userId
                    : null;

            }
            catch
            {
                return null;
            }
        }
    }
    public class UserIdentity
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = default!;
        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;
    }
}
