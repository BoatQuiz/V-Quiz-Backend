using Microsoft.Azure.Functions.Worker.Http;

namespace V_Quiz_Backend.Context
{
    public static class UserContext
    {
        public static Guid? TryGetUserId(HttpRequestData req)
        {
            // 1. Cookie (Isolated Worker)
            var userCookie = req.Cookies.FirstOrDefault(c => c.Name == "userId");
            if (userCookie != null && Guid.TryParse(userCookie.Value, out var userId)) { return userId; }
            

            // 2. DEV-Header
            if (req.Headers.TryGetValues("X-User-Id", out var values) && Guid.TryParse(values.FirstOrDefault(), out var headerUserId))
            {
                return headerUserId;
            }
            

            // 3. Framtida auth(JWT / Entra ID)
            return null;
        }
    }
}
