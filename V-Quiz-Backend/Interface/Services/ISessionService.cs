using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Services
{
    public interface ISessionService
    {
        Task<ServiceResponse<Session>> CreateSessionAsync(Guid? userId = null);
        Task<ServiceResponse<Session>> GetSessionByIdAsync(Guid sessionId);
        Task<ServiceResponse<bool>> UpdateSessionAsync(Session session);
    }
}
