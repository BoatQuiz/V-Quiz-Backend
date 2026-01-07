using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Repos
{
    public interface ISessionRepository
    {
        public Task<bool> CreateSessionAsync(Session session);
        public Task<Session> GetSessionAsync(Guid sessionId);
        public Task UpdateSessionAsync(Session session);
        public Task SetCurrentQuestionAsync(Guid sessionId, CurrentQuestionState question);
    }
}
