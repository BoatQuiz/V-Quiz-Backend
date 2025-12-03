using MongoDB.Driver;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Repository
{
    public class FlagRepository : IFlagRepository
    {

        private readonly IMongoCollection<FlaggedQuestion> _collection;

        public FlagRepository(MongoDbService mongo)
        {
            _collection = mongo.Database.GetCollection<FlaggedQuestion>("FlaggedQuestions");
        }

        public async Task AddFlaggedQuestionAsync(FlaggedQuestion flaggedQuestion)
        {
            await _collection.InsertOneAsync(flaggedQuestion);
        }

        public async Task<FlaggedQuestion> GetFlaggedQuestionByIdAsync(string questionId)
        {
            var filter = Builders<FlaggedQuestion>.Filter.Eq(fq => fq.QuestionId, questionId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateFlaggedQuestionAsync(FlaggedQuestion flaggedQuestion)
        {
            var filter = Builders<FlaggedQuestion>.Filter.Eq(fq => fq.Id, flaggedQuestion.Id);
            await _collection.ReplaceOneAsync(filter, flaggedQuestion);
        }
    }

}
