using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class SessionService
    {
        private readonly SessionRepository _repo;

        public SessionService(SessionRepository repo)
        {
            _repo = repo;
        }

        public async Task<Session> CreateSessionAsync(Guid? userId=null)
        {
            var session = new Session
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UsedQuestions = new List<string>(),
                NumCorrectAnswers = 0,
                NumQuestions = 0,
                StoppedAt = null,
                IsCompleted = false
            };
            await _repo.CreateSessionAsync(session);
            return session;
        }

        public async Task<Session> GetSessionByIdAsync(Guid sessionId)
        {
            return await _repo.GetSessionAsync(sessionId);
        }

        public async Task UpdateSessionAsync(Session session)
        {
            await _repo.UpdateSessionAsync(session);
        }
    }
}
