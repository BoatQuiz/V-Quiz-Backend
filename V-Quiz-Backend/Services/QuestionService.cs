using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Services
{
    public class QuestionService(IQuestionRepository repo) : IQuestionService
    {
        private readonly IQuestionRepository _repo = repo;


        public async Task<ServiceResponse<Question>> GetQuestionByIdAsync(string questionId)
        {
            var question = await _repo.GetQuestionByIdAsync(questionId);

            if (question == null)
            {
                return ServiceResponse<Question>.Fail("Question not found.");
            }

            return ServiceResponse<Question>.Ok(question);
        }

        public async Task <ServiceResponse<QuestionResponseDto>> GetRandomQuestionAsync(Session session)
        {
            var filter = new QuestionFilter
            {
                ExcludedQuestionIds = session.UsedQuestions.Select(q => q.QuestionId),
                AllowedCategories = session.Player.Categories,
                Difficulty = null,
                Audience = session.Player.Audience
            };

            var question = await _repo.GetRandomQuestionAsync(filter);
            
            if (question == null)
            {
                return ServiceResponse<QuestionResponseDto>.Fail("No more questions available.");
            }
            
            return ServiceResponse<QuestionResponseDto>.Ok(new QuestionResponseDto
            {
                QuestionId = question.QuestionId,
                QuestionText = question.Text,
                Options = question.Options,
                CorrectIndex = question.CorrectIndex,
            });
        }
    }
}
