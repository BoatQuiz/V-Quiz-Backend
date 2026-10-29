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
            var f = Builders<Question>.Filter;

            // Bara aktiva frågor (samma som metadatan räknar)
            var mongoFilter = f.Eq(q => q.IsActive, true);

            if (filter.ExcludedQuestionIds.Any())
            {
                mongoFilter &= Builders<Question>.Filter.Nin(q => q.QuestionId, filter.ExcludedQuestionIds);
            }

            if (filter.Selection?.Count > 0)
            {
                mongoFilter &= BuildSelectionFilter(filter.Selection);
            }
            else if (filter.AllowedCategories?.Any() == true)
            {
                mongoFilter &= f.In(q => q.Category, filter.AllowedCategories);
            }

            if (filter.Difficulty.HasValue)
            {
                mongoFilter &= Builders<Question>.Filter.Eq(q => q.Difficulty, filter.Difficulty.Value);
            }

            if (filter.Audience?.Any() == true)
            {
                mongoFilter &= Builders<Question>.Filter.Eq(q => q.Audience, filter.Audience);
            }

            return await _collection
                .Aggregate()
                .Match(mongoFilter)
                .Sample(1)
                .FirstOrDefaultAsync();
        }

        // Kategori utan valda subkategorier = hela kategorin.
        // Subkategori utan valda topics = hela subkategorin.
        // Allt kombineras med OR mellan valen.
        private static FilterDefinition<Question> BuildSelectionFilter(List<CategorySelection> selection)
        {
            var f = Builders<Question>.Filter;
            var categoryFilters = new List<FilterDefinition<Question>>();

            foreach (var cat in selection)
            {
                var catFilter = f.Eq(q => q.Category, cat.Name);

                if (cat.Subcategories.Count == 0)
                {
                    categoryFilters.Add(catFilter);
                    continue;
                }

                var subFilters = new List<FilterDefinition<Question>>();
                foreach (var sub in cat.Subcategories)
                {
                    var subFilter = f.AnyEq(q => q.Subcategory, sub.Name);

                    if (sub.Topics.Count > 0)
                    {
                        subFilter &= f.In(q => q.Topic, sub.Topics);
                    }

                    subFilters.Add(subFilter);
                }

                categoryFilters.Add(catFilter & f.Or(subFilters));
            }

            return f.Or(categoryFilters);
        }

        public async Task<List<QuizMetadataProjection>> GetQuizMetaDataAsync()
        {
            return await _collection
                .Find(q => q.IsActive)
                .Project(q => new QuizMetadataProjection
                {
                    Audience = q.Audience,
                    Category = q.Category,
                    Subcategory = q.Subcategory,
                    Topic = q.Topic
                })
                .ToListAsync();
        }
    }
}
