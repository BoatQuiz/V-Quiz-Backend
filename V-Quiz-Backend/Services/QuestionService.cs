using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class QuestionService(IQuestionRepository repo, ISessionService sessionService) : IQuestionService
    {
        private readonly IQuestionRepository _repo = repo;
        private readonly ISessionService _sessionService = sessionService;

        public async Task<ServiceResponse<QuestionResponse>> StartQuizAsync(Guid? userId = null)
        {
            var session = await _sessionService.CreateSessionAsync(userId);

            if (!session.Success || session == null || session.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Failed to create session.");
            }
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
            var sessionResponse = await _sessionService.GetSessionByIdAsync(request.SessionId);

            if (!sessionResponse.Success || sessionResponse == null || sessionResponse.Data == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session not found.");
            }

            var session = sessionResponse.Data;

            if (session.IsCompleted)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Session is already completed.");
            }
            var questionResponse = await GetQuestionByIdAsync(request.QuestionId);

            if(!questionResponse.Success || questionResponse == null || questionResponse.Data == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Question not found.");
            }
            var question = questionResponse;

            if (question == null)
            {
                return ServiceResponse<SubmitAnswerResponse>.Fail("Question not found.");
            }

            bool isCorrect = question.Data.CorrectIndex == request.SelectedAnswer;

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
                CorrectIndex = question.Data.CorrectIndex,
                CorrectAnswer = question.Data.Options[question.Data.CorrectIndex],
                IsLastQuestion = isLastQuestion,
                Score = session.NumCorrectAnswers,
                QuestionsAnswered = session.NumQuestions
            };

            return response;
        }

        public async Task<ServiceResponse<QuestionResponse>> GetNextQuestionAsync(SubmitSessionId sessionReq)
        {
            var session = await _sessionService.GetSessionByIdAsync(sessionReq.SessionId);
            if (!session.Success || session == null || session.Data == null)
            {
                return ServiceResponse<QuestionResponse>.Fail("Session not found.");
            }
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
