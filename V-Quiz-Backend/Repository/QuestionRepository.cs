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
    }
}
