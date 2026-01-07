using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class SessionService(ISessionRepository repo) : ISessionService
    {
        private readonly ISessionRepository _repo = repo;

        public async Task<ServiceResponse<Session>> CreateSessionAsync(Guid? userId = null)
        {

            var session = new Session
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                
                NumCorrectAnswers = 0,
                NumQuestions = 0,
                StoppedAt = null,
                IsCompleted = false
            };

            try
            {
                var insertSucceded = await _repo.CreateSessionAsync(session);

                if (!insertSucceded)
                {
                    return ServiceResponse<Session>.Fail("Failed to create session.");
                }
            return ServiceResponse<Session>.Ok(session, "Session created successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResponse<Session>.Fail("Failed to create session: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<Session>> GetSessionByIdAsync(Guid sessionId)
        {
            var session = await _repo.GetSessionAsync(sessionId);

            if (session == null)
            {
                return ServiceResponse<Session>.Fail("Session not found.");
            }

            return ServiceResponse<Session>.Ok(session);
        }

        public async Task<ServiceResponse<bool>> UpdateSessionAsync(Session session)
        {
            try
            {
                await _repo.UpdateSessionAsync(session);
                return ServiceResponse<bool>.Ok(true, "Session updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Fail("Failed to update session: " + ex.Message);

            }
        }

        public async Task<ServiceResponse<bool>> SetCurrentQuestionAsync(Guid sessionId, string currentQuestionId)
        {
            try
            {
                await _repo.SetCurrentQuestionAsync(sessionId, currentQuestionId);
                return ServiceResponse<bool>.Ok(true, "Current question set successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Fail("Failed to set current question: " + ex.Message);
            }
        }
    }
}
