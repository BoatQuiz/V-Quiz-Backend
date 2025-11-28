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
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _repo.GetAllQuestionsAsync();
        }

        public async Task<int> GetQuestionCountAsync()
        {
            return await _repo.GetQuestionCountAsync();
        }

        public async Task<StartQuizResult> StartQuizAsync(Guid? userId = null)
        {
            var session = await _sessionService.CreateSessionAsync(userId);
            var question = await _repo.GetRandomQuestionAsync(session.UsedQuestions);
            
            return new StartQuizResult
            {
                Session = session,
                RandomQuestion = question
            };
        }

        public async Task<Question> GetQuestionByIdAsync(string questionId)
        {
            return await _repo.GetQuestionByIdAsync(questionId);
        }

        public async Task<SubmitAnswerResponse> SubmitAnswerAsync(SubmitAnswerRequest request)
        {
            var session = await _sessionService.GetSessionByIdAsync(request.SessionId);
            if (session == null)
            {
                throw new Exception("Session not found.");
            }
            var question = await GetQuestionByIdAsync(request.QuestionId);
            if (question == null)
            {
                throw new Exception("Question not found.");
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
            }


            await _sessionService.UpdateSessionAsync(session);

            var responseObj = new SubmitAnswerResponse
            {
                IsCorrect = isCorrect,
                CorrectIndex = question.CorrectIndex,
                CorrectAnswer = question.Options[question.CorrectIndex],
                //Explanation = question.Explanation ?? "",
                //infoUrl = question.InfoUrl ?? "",
                IsLastQuestion = isLastQuestion,
                Score = session.NumCorrectAnswers,
                QuestionsAnswered = session.NumQuestions
            };

            return responseObj;
        }

        public async Task<Question> GetNextQuestionAsync (SubmitSessionId sessionReq)
        {
            var session = await _sessionService.GetSessionByIdAsync(sessionReq.SessionId);
            return await _repo.GetRandomQuestionAsync(session.UsedQuestions);
        }
    }
}
