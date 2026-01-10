using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Services
{
    public interface ISessionService
    {
        Task<ServiceResponse<Session>> CreateSessionAsync(Guid? userId = null);
        Task<ServiceResponse<Session>> GetSessionByIdAsync(Guid sessionId);
        Task<ServiceResponse<bool>> UpdateSessionAsync(Session session);
        Task<ServiceResponse<bool>> SetCurrentQuestionAsync(Guid sessionId, string questionId);
        Task<ServiceResponse<SessionIdentity>> GetUserIdBySessionIdAsync(Guid sessionId);
    }
}
