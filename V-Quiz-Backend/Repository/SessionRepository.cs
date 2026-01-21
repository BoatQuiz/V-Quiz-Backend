using MongoDB.Driver;
using V_Quiz_Backend.DTO;
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

        public async Task SetCurrentQuestionAsync(Guid sessionId, CurrentQuestionState question)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, sessionId);
            var update = Builders<Session>.Update.Set(s => s.CurrentQuestion, question);
            
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task<SessionIdentity?> GetUserIdBySessionIdAsync(Guid sessionId)
        {
            return await _collection
                .Find(s => s.Id == sessionId)
                .Project(s => new SessionIdentity
                {
                    SessionId = s.Id,
                    UserId = s.Player.UserId
                    // TODO: Koppla på användarnamn när användarmodellen är klar
                    //UserName = s.UserName
                })
                .FirstOrDefaultAsync();
        }

        public async Task AppendUsedQuestionAsync(Guid sessionId, UsedQuestion usedQuestion, bool endSession)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, sessionId);
            var updates = new List<UpdateDefinition<Session>>
            {
                Builders<Session>.Update.Push(s => s.UsedQuestions, usedQuestion),
                Builders<Session>.Update.Set(s => s.CurrentQuestion, null)
            };

            if (endSession)
            {
                updates.Add(Builders<Session>.Update.Set(s => s.EndedAtUtc, DateTime.UtcNow));
            }

            var update = Builders<Session>.Update.Combine(updates);
            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
