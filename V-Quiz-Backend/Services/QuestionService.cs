using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class QuestionService(QuestionRepository repo, SessionService sessionService)
    {
        private readonly QuestionRepository _repo = repo;
        private readonly SessionService _sessionService = sessionService;

        public async Task<ServiceResponse<QuestionResponse>> StartQuizAsync(Guid? userId = null)
        {
            var session = await _sessionService.CreateSessionAsync(userId);
            var question = await _repo.GetRandomQuestionAsync(session.Data.UsedQuestions);

            return ServiceResponse<QuestionResponse>.Ok(new QuestionResponse
            {
                Session = new SubmitSessionId
                {
                    SessionId = session.Data.Id
                },
                Question = new QuestionResponseDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.Text,
                    Options = question.Options
                }
            });
        }

        public async Task<ServiceResponse<Question>> GetQuestionByIdAsync(string questionId)
        {
            var question = await _repo.GetQuestionByIdAsync(questionId);

            if (question == null)
            {
                return ServiceResponse<Question>.Fail("Question not found.");
            }

            return ServiceResponse<Question>.Ok(question);
        }

        public async Task<ServiceResponse<SubmitAnswerResponse>> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var response = new ServiceResponse<SubmitAnswerResponse>();
            var session = await _sessionService.GetSessionByIdAsync(request.SessionId);

            if (session == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session not found.");
            }

            if (session.Data.IsCompleted)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session is already completed.");
            }
            var question = await GetQuestionByIdAsync(request.QuestionId);
            if (question == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Question not found.");
            }

            bool isCorrect = question.Data.CorrectIndex == request.SelectedAnswer;

            if (isCorrect)
            {
                session.Data.NumCorrectAnswers++;
            }
            session.Data.NumQuestions++;
            session.Data.UsedQuestions.Add(request.QuestionId.ToString());

            var isLastQuestion = false;
            if (session.Data.NumQuestions >= 10)
            {
                isLastQuestion = true;
                session.Data.StoppedAt = DateTime.UtcNow;
                session.Data.IsCompleted = true;
            }

            await _sessionService.UpdateSessionAsync(session.Data);

            response.Data = new SubmitAnswerResponse
            {
                IsCorrect = isCorrect,
                CorrectAnswer = question.Data.Options[question.Data.CorrectIndex],
                IsLastQuestion = isLastQuestion,
                Score = session.Data.NumCorrectAnswers,
                QuestionsAnswered = session.Data.NumQuestions
            };

            return response;
        }

        public async Task<ServiceResponse<QuestionResponse>> GetNextQuestionAsync(SubmitSessionId sessionReq)
        {
            var session = await _sessionService.GetSessionByIdAsync(sessionReq.SessionId);
            var question = await _repo.GetRandomQuestionAsync(session.Data.UsedQuestions);

            return ServiceResponse<QuestionResponse>.Ok(
                new QuestionResponse
                {
                    Session = new SubmitSessionId
                    {
                        SessionId = session.Data.Id
                    },
                    Question = new QuestionResponseDto
                    {
                        QuestionId = question.QuestionId,
                        QuestionText = question.Text,
                        Options = question.Options
                    }
                }
            );
        }
    }
}
