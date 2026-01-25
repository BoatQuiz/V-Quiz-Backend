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
            // 1. Skapa session
            var sessionResponse = await _sessionService.CreateSessionAsync(
                userId: userId,
                targetQuestionsCount: 10);

            if (!sessionResponse.Success || sessionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Failed to create session.");
            }

            var session = sessionResponse.Data;

            // 2. Hämta första frågan(Inga exkluderingar eftersom det är första frågan)
            var questionResponse = await _questionService.GetRandomQuestionAsync(session);
                
            if (!questionResponse.Success || questionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Failed to retrieve question.");
            }

            var question = questionResponse.Data;

            // 3. Sätt nuvarande fråga i sessionen 
            await _sessionService.SetCurrentQuestionAsync(session.Id, question);

            // 4. Returnera minimal payload med session och fråga
            return ServiceResponse<QuestionResponse>.Ok(new QuestionResponse
            {
                Session = new SessionDtoResult
                {
                    SessionId = session.Id,
                    QuestionsAnswered = 0,
                    TargetQuestionCount = session.TargetQuestionCount
                },
                Question = new QuestionResponseDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    Options = question.Options
                }
            });
        }

        public async Task<ServiceResponse<QuestionResponse>> GetNextQuestionAsync(SubmitSessionId sessionReq)
        {
            // 1. Hämta session
            var sessionResponse = await _sessionService.GetSessionByIdAsync(sessionReq.SessionId);
            if (!sessionResponse.Success || sessionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Invalid session.");
            }

            var session = sessionResponse.Data;

            // 2. Kolla om session inte är avslutad
            if (session.EndedAtUtc != null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Session is already completed.");
            }

            // 3. Blockera om en fråga redan är aktiv
            if (session.CurrentQuestion != null)
            {
                return ServiceResponse<QuestionResponse>.Fail("There is already an active question.");
            }

            // 4. Hämta nästa fråga
            var excludedIds = session.UsedQuestions.Select(q => q.QuestionId);


            var questionResponse = await _questionService.GetRandomQuestionAsync(session);

            if (!questionResponse.Success || questionResponse.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Failed to retrieve question.");
            }

            var q = questionResponse.Data;

            // 5. Sätt nuvarande fråga i sessionen + starttid
            await _sessionService.SetCurrentQuestionAsync(session.Id, q);

            // 6. Returnera minimal payload med session och fråga
            return ServiceResponse<QuestionResponse>.Ok(new QuestionResponse
            {
                Session = new SessionDtoResult
                {
                    SessionId = session.Id,
                    QuestionsAnswered = session.UsedQuestions.Count,
                    TargetQuestionCount = session.TargetQuestionCount
                },
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
            // 1. Hämta session
            var sessionResponse = await _sessionService.GetSessionByIdAsync(request.SessionId);
            if (!sessionResponse.Success || sessionResponse.Data == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Invalid session.");
            }
            var session = sessionResponse.Data;

            // 2. Kolla om session inte är avslutad
            if (session.EndedAtUtc != null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session is already completed.");
            }

            // 3. Kontroller att frågan är aktiv
            if (session.CurrentQuestion == null || session.CurrentQuestion.QuestionId != request.QuestionId)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("No active question.");
            }

            // 4. Hämta fråga
            //var questionResponse = await _questionService.GetQuestionByIdAsync(request.QuestionId);
            //if (!questionResponse.Success || questionResponse.Data == null)
            //{
            //    return ServiceResponse<SubmitAnswerResponse>.Fail("Invalid question.");
            //}

            //var question = questionResponse.Data;

            // 5. Kontrollera svaret + tid
            bool isCorrect = request.SelectedAnswer == session.CurrentQuestion.CorrectIndex;

            var timeMs = (DateTime.UtcNow - session.CurrentQuestion.AskedAtUtc).TotalMilliseconds;

            // 6. Skapa UsedQuestion
            var usedQuestion = new UsedQuestion
            {
                QuestionId = session.CurrentQuestion.QuestionId,
                Category = session.CurrentQuestion.Category,
                AnsweredCorrectly = isCorrect,
                TimeMs = timeMs
            };

            // 7. Skriv till databasen
            bool isLastQuestion = session.UsedQuestions.Count >= session.TargetQuestionCount;

            await _sessionService.AppendAnsweredQuestionAsync(session, usedQuestion);


            // 10. Returnera svar
            return ServiceResponse<SubmitAnswerResponse>.Ok(new SubmitAnswerResponse
            {
                IsCorrect = isCorrect,
                CorrectIndex = session.CurrentQuestion.CorrectIndex,
                
                QuestionsAnswered = session.UsedQuestions.Count + 1,
                Score = session.UsedQuestions.Count(q => q.AnsweredCorrectly) + (isCorrect ? 1 : 0),

                IsLastQuestion = isLastQuestion
            });
        }

    }
}
