using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Services
{
    public interface ISessionService
    {
        Task<ServiceResponse<Session>> CreateSessionAsync(Guid? userId = null, int targetQuestionsCount = 10, List<string>? allowedCategories = null);
        Task<ServiceResponse<Session>> GetSessionByIdAsync(Guid sessionId);
        Task<ServiceResponse<bool>> SetCurrentQuestionAsync(Guid sessionId, QuestionResponseDto question);
        Task<ServiceResponse<SessionIdentity>> GetUserIdBySessionIdAsync(Guid sessionId);
        Task AppendAnsweredQuestionAsync(Session session, UsedQuestion usedQuestion);
    }
}
