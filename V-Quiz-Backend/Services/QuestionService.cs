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

        public async Task<ServiceResponse<QuestionResponseDto>> GetRandomQuestionAsync(Session session)
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
                Category = question.Category
            });
        }

        public static QuestionResponseDto ShuffleQuestion(QuestionResponseDto question)
        {
            var rnd = new Random();
            var options = question.Options
                .Select((text, index) => new
                {
                    Text = text,
                    IsCorrect = index == question.CorrectIndex
                })
                .OrderBy(_ => rnd.Next())
                .ToList();

            question.Options = options.Select(o => o.Text).ToList();
            question.CorrectIndex = options.FindIndex(o => o.IsCorrect);

            return question;
        }

        public async Task<ServiceResponse<QuizMetaDataDto>> GetQuizMetaDataAsync()
        {
            var rawData = await _repo.GetQuizMetaDataAsync();
            
            var audiences = rawData
                .GroupBy(q => q.Audience)
                .Select(g => new AudienceMetaDto
                {
                    Name = g.Key,
                    Categories = g
                    .Select(q => q.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList()
                })
                .OrderBy(a => a.Name)
                .ToList();

            return new ServiceResponse<QuizMetaDataDto> { Data = new QuizMetaDataDto { Audiences = audiences }, Success = true };
        }
    }
}
