using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Services
{
    public class QuizService : IQuizService
    {
        private readonly ISessionService _sessionService;
        private readonly IQuestionService _questionService;

        public QuizService(ISessionService sessionService, IQuestionService questionService)
        {
            _sessionService = sessionService;
            _questionService = questionService;
        }
        // Skall jag verkligen skicka in userId här?
        // Eller jag skall nog koppla userId i sessionservicen
        public async Task<ServiceResponse<QuestionResponse>> StartQuizAsync(Guid? userId = null)
        {
            var sessionResponse = await _sessionService.CreateSessionAsync(userId);
            if (!sessionResponse.Success || sessionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Failed to create session.");
            }

            var session = sessionResponse.Data;

            var questionResponse = await _questionService.GetRandomQuestionAsync(
                excludedQuestionIds: session.UsedQuestions.Select(q => q.QuestionId));
            if (!questionResponse.Success || questionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Failed to retrieve question.");
            }

            var q = questionResponse.Data;

            await _sessionService.SetCurrentQuestionAsync(session.Id, q.QuestionId);

            return ServiceResponse<QuestionResponse>.Ok(new QuestionResponse
            {
                Session = new SessionDtoResult
                {
                    SessionId = session.Id,
                    Score = session.NumCorrectAnswers,
                    NumUsedQuestions = session.NumQuestions
                },
                Question = new QuestionResponseDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Options = q.Options
                }
            });
        }

        public async Task<ServiceResponse<QuestionResponse>> GetNextQuestionAsync(SubmitSessionId sessionReq)
        {
            // Hämta nuvarande session
            var sessionResponse = await _sessionService.GetSessionByIdAsync(sessionReq.SessionId);
            if (!sessionResponse.Success || sessionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Invalid session.");
            }

            var session = sessionResponse.Data;

            // Hämta en ny fråga som inte har använts i sessionen
            var questionResponse = await _questionService.GetRandomQuestionAsync(excludedQuestionIds: session.UsedQuestions.Select(q => q.QuestionId));
            if (!questionResponse.Success || questionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Failed to retrieve question.");
            }

            var q = questionResponse.Data;

            await _sessionService.SetCurrentQuestionAsync(session.Id, q.QuestionId);

            return ServiceResponse<QuestionResponse>.Ok(new QuestionResponse
            {
                Session = new SessionDtoResult { SessionId = session.Id, Score = session.NumCorrectAnswers, NumUsedQuestions = session.NumQuestions },
                Question = new QuestionResponseDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Options = q.Options
                }
            });
        }

        public async Task<ServiceResponse<SubmitAnswerResponse>> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var sessionResponse = await _sessionService.GetSessionByIdAsync(request.SessionId);
            if (!sessionResponse.Success || sessionResponse.Data == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Invalid session.");
            }
            var session = sessionResponse.Data;

            if (session.IsCompleted)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session is already completed.");
            }

            var questionResponse = await _questionService.GetQuestionByIdAsync(request.QuestionId);
            if (!questionResponse.Success || questionResponse.Data == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Invalid question.");
            }

            var question = questionResponse.Data;

            bool isCorrect = request.SelectedAnswer == question.CorrectIndex;

            if (session.CurrentQuestion == null || session.CurrentQuestion.QuestionId != request.QuestionId)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("No active question");
            }

            var now = DateTime.UtcNow;
            var timeMs = (now - session.CurrentQuestion.AskedAtUtc).TotalMilliseconds;


            if (isCorrect)
            {
                session.NumCorrectAnswers++;
            }
            session.NumQuestions++;
            session.UsedQuestions.Add(new UsedQuestion
            {
                QuestionId = question.QuestionId,
                Category = "",
                AnsweredCorrectly = isCorrect,
                TimeMs = timeMs

            });

            session.CurrentQuestion = null;

            bool isLastQuestion = false;
            // Denna styr hur många frågor en quiz har
            // Skulle kunna ändra detta så att en spelare kan välja själv
            if (session.NumQuestions >= 10)
            {
                session.IsCompleted = true;
                isLastQuestion = true;
                session.StoppedAt = DateTime.UtcNow;
            }

            await _sessionService.UpdateSessionAsync(session);

            return ServiceResponse<SubmitAnswerResponse>.Ok(new SubmitAnswerResponse
            {
                IsCorrect = isCorrect,
                CorrectIndex = question.CorrectIndex,
                CorrectAnswer = question.Options[question.CorrectIndex],
                QuestionsAnswered = session.NumQuestions,
                Score = session.NumCorrectAnswers,
                IsLastQuestion = isLastQuestion
            });
        }

    }
}
