using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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

        public async Task<Question> GetRandomQuestionAsync(
            IEnumerable<string> excludedQuestionIds,
            IEnumerable<string>? allowedCategories)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var filterBuilder = Builders<Question>.Filter;

            var filter = filterBuilder.Nin(q => q.QuestionId, excludedQuestionIds);

            if (allowedCategories != null && allowedCategories.Any())
            {
                filter &= filterBuilder.AnyIn(q => q.Category, allowedCategories);
            }

            var question = await _collection
                .Aggregate()
                .Match(filter)
                .Sample(1)
                .FirstOrDefaultAsync();

            sw.Stop();

            _logger.LogDebug("GetRandomQuestionAsync DB query took {ElapsedMs} ms", sw.ElapsedMilliseconds);

            return question;
        }

    }
}
