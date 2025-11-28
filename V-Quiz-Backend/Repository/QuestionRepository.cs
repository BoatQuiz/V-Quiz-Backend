using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Repository
{
    public class QuestionRepository
    {
        private readonly IMongoCollection<Question> _collection;

        public QuestionRepository(MongoDbService mongo)
        {
            _collection = mongo.Database.GetCollection<Question>("Questions");
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<int> GetQuestionCountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(_ => true);
        }

        public async Task <Question> GetQuestionByIdAsync(string questionId)
        {
            var filter = Builders<Question>.Filter.Eq(q => q.QuestionId, questionId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Question> GetRandomQuestionAsync(List<string> usedQuestions)
        {
            var filter = Builders<Question>.Filter.Nin(p => p.QuestionId, usedQuestions);

            return await _collection.Aggregate()
                .Match(filter)
                .Sample(1)
                .FirstOrDefaultAsync();
        }
            
    }
}
