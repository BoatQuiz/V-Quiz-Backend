
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Services
{
    public interface IQuestionService
    {
        public Task<ServiceResponse<Question>> GetQuestionByIdAsync(string questionId);
        public Task<ServiceResponse<QuestionResponseDto>> GetRandomQuestionAsync(Session session);
        public Task<ServiceResponse<QuizMetaDataDto>> GetQuizMetaDataAsync();
    }
}
