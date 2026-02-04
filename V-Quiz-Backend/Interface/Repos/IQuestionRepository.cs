using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Repos
{
    public interface IQuestionRepository
    {
        public Task<List<Question>> GetAllQuestionsAsync();
        public Task<int> GetQuestionCountAsync();
        public Task<Question> GetQuestionByIdAsync(string questionId);
        public Task<Question> GetRandomQuestionAsync(QuestionFilter filter);
        public Task<List<QuizMetadataProjection>> GetQuizMetaDataAsync();
    }
}
