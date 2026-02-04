using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IMongoCollection<Question> _collection;
        private readonly ILogger<QuestionRepository> _logger;

        public QuestionRepository(MongoDbService mongoDbService, ILogger<QuestionRepository> logger)
        {
            _collection = mongoDbService.Database.GetCollection<Question>("Questions");
            _logger = logger;
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<int> GetQuestionCountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(_ => true);
        }

        public async Task<Question> GetQuestionByIdAsync(string questionId)
        {
            var filter = Builders<Question>.Filter.Eq(q => q.QuestionId, questionId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Question> GetRandomQuestionAsync(QuestionFilter filter)
        {
            var mongoFilter = Builders<Question>.Filter.Empty;

            if (filter.ExcludedQuestionIds.Any())
            {
                mongoFilter &= Builders<Question>.Filter.Nin(q => q.QuestionId, filter.ExcludedQuestionIds);
            }

            if (filter.AllowedCategories?.Any() == true)
            {
                mongoFilter &= Builders<Question>.Filter.AnyIn(q => q.Category, filter.AllowedCategories);
            }

            if (filter.Difficulty.HasValue)
            {
                mongoFilter &= Builders<Question>.Filter.Eq(q => q.Difficulty, filter.Difficulty.Value);
            }

            if (filter.Audience?.Any() == true)
            {
                mongoFilter &= Builders<Question>.Filter.AnyEq(q => q.Audience, filter.Audience);
            }

            return await _collection
                .Aggregate()
                .Match(mongoFilter)
                .Sample(1)
                .FirstOrDefaultAsync();
        }

        public async Task<List<QuizMetadataProjection>> GetQuizMetaDataAsync()
        {
            return await _collection
                .Find(q => q.IsActive)
                .Project(q => new QuizMetadataProjection
                {
                    Audience = q.Audience,
                    Category = q.Category
                })
                .ToListAsync();
        }
    }
}
