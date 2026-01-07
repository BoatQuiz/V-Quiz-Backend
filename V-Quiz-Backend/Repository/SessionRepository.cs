using MongoDB.Driver;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly IMongoCollection<Session> _collection;
        public SessionRepository(MongoDbService mongo)
        {
            _collection = mongo.Database.GetCollection<Session>("Sessions");
        }

        public async Task<bool> CreateSessionAsync(Session session)
        {
            try
            {
                await _collection.InsertOneAsync(session);
                return true;
            }
            catch
            {
                return false;

            }
        }

        public async Task<Session> GetSessionAsync(Guid sessionId)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, sessionId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateSessionAsync(Session session)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, session.Id);
            await _collection.ReplaceOneAsync(filter, session);
        }

        public async Task SetCurrentQuestionAsync(Guid sessionId, string questionId)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, sessionId);
            var update = Builders<Session>.Update.Set(s => s.CurrentQuestion, new CurrentQuestionState
            {
                QuestionId = questionId,
                AskedAtUtc = DateTime.UtcNow
            });
            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
