
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Services
{
    public interface IQuestionService
    {
        public Task<ServiceResponse<QuestionResponse>> StartQuizAsync(Guid? userId = null);
        public Task<ServiceResponse<Question>> GetQuestionByIdAsync(string questionId);
        public Task<ServiceResponse<SubmitAnswerResponse>> SubmitAnswerAsync(SubmitAnswerRequest request);
        public Task<ServiceResponse<QuestionResponse>> GetNextQuestionAsync(SubmitSessionId sessionId);
    }
}
