using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Services
{
    public class SessionService(ISessionRepository repo, IUserService userService) : ISessionService
    {
        private readonly ISessionRepository _repo = repo;
        private readonly IUserService _userService = userService;

        public async Task<ServiceResponse<Session>> CreateSessionAsync(Guid? userId = null, int targetQuestionsCount = 10, List<string>? allowedCategories = null)
        {
            var quizProfileResponse = await _userService.GetQuizProfileAsync(userId);

            if (quizProfileResponse == null ||
                !quizProfileResponse.Success ||
                quizProfileResponse.Data == null)
            {
                return ServiceResponse<Session>.Fail("Failed to retrieve user quiz profile.");
            }

            var session = new Session
            {
                Player = new SessionUser
                {
                    UserId = userId,
                    Audience = quizProfileResponse.Data.Audience ?? "general",
                    Categories = quizProfileResponse.Data.Categories,
                },
                StartedAtUtc = DateTime.UtcNow,

                TargetQuestionCount = targetQuestionsCount,

                CurrentQuestion = null,
                UsedQuestions = new List<UsedQuestion>(),
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
                var question = new CurrentQuestionState
                {
                    QuestionId = currentQuestionId,
                    AskedAtUtc = DateTime.UtcNow,
                };
                await _repo.SetCurrentQuestionAsync(sessionId, question);
                return ServiceResponse<bool>.Ok(true, "Current question set successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Fail("Failed to set current question: " + ex.Message);
            }
        }

        public async Task<ServiceResponse<SessionIdentity>> GetUserIdBySessionIdAsync(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
            {
                return ServiceResponse<SessionIdentity>.Fail("Invalid session ID.");
            }

            var userId = await _repo.GetUserIdBySessionIdAsync(sessionId);
            if (userId == null)
            {
                return ServiceResponse<SessionIdentity>.Fail("Session not found.");
            }
            return ServiceResponse<SessionIdentity>.Ok(userId);
        }

        public async Task AppendAnsweredQuestionAsync(Session session, UsedQuestion usedQuestion)
        {
            bool endSession = session.UsedQuestions.Count + 1 >= session.TargetQuestionCount;

            await _repo.AppendUsedQuestionAsync(session.Id, usedQuestion, endSession);
        }
    }
}
