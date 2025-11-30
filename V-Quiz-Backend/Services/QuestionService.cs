using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _repo;
        private readonly SessionService _sessionService;
        public QuestionService(QuestionRepository repo, SessionService sessionService)
        {
            _repo = repo;
            _sessionService = sessionService;
        }

        public async Task<QuestionResponse> StartQuizAsync(Guid? userId = null)
        {
            var session = await _sessionService.CreateSessionAsync(userId);
            var question = await _repo.GetRandomQuestionAsync(session.UsedQuestions);
            
            return new QuestionResponse
            {
                Session = new SubmitSessionId
                {
                    SessionId = session.Id
                },
                Question = new QuestionResponseDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.Text,
                    Options = question.Options
                }

            };
        }

        public async Task<Question> GetQuestionByIdAsync(string questionId)
        {
            return await _repo.GetQuestionByIdAsync(questionId);
        }

        public async Task<ServiceResponse<SubmitAnswerResponse>> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var response = new ServiceResponse<SubmitAnswerResponse>();
            var session = await _sessionService.GetSessionByIdAsync(request.SessionId);

            if (session == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session not found.");
            }

            if (session.IsCompleted)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session is already completed.");
            }
            var question = await GetQuestionByIdAsync(request.QuestionId);
            if (question == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Question not found.");
            }

            bool isCorrect = question.CorrectIndex == request.SelectedAnswer;

            if (isCorrect)
            {
                session.NumCorrectAnswers++;
            }
            session.NumQuestions++;
            session.UsedQuestions.Add(request.QuestionId.ToString());

            var isLastQuestion = false;
            if (session.NumQuestions >= 10)
            {
                isLastQuestion = true;
                session.StoppedAt = DateTime.UtcNow;
                session.IsCompleted = true;
            }


            await _sessionService.UpdateSessionAsync(session);

            response.Data = new SubmitAnswerResponse
            {
                IsCorrect = isCorrect,
                CorrectAnswer = question.Options[question.CorrectIndex],
                IsLastQuestion = isLastQuestion,
                Score = session.NumCorrectAnswers,
                QuestionsAnswered = session.NumQuestions
            };

            return response;
        }

        public async Task<QuestionResponse> GetNextQuestionAsync (SubmitSessionId sessionReq)
        {
            var session = await _sessionService.GetSessionByIdAsync(sessionReq.SessionId);
            var question= await _repo.GetRandomQuestionAsync(session.UsedQuestions);

            return new QuestionResponse
            {
                Session = new SubmitSessionId
                {
                    SessionId = session.Id
                },
                Question = new QuestionResponseDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.Text,
                    Options = question.Options
                }

            };
        }
    }
}
