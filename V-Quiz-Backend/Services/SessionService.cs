using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class SessionService(SessionRepository repo)
    {
        private readonly SessionRepository _repo = repo;

        public async Task<ServiceResponse<Session>> CreateSessionAsync(Guid? userId = null)
        {

            var session = new Session
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UsedQuestions = [],
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
    }
}
