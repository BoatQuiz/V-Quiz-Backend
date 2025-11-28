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
    public class SessionRepository
    {
        private readonly IMongoCollection<Session> _collection;

        public SessionRepository(MongoDbService mongo)
        {
            _collection = mongo.Database.GetCollection<Session>("Sessions");
        }

        public async Task CreateSessionAsync(Session session)
        {
            await _collection.InsertOneAsync(session);
        }

        public async Task <Session> GetSessionAsync(Guid sessionId)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, sessionId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task UpdateSessionAsync(Session session)
        {
            var filter = Builders<Session>.Filter.Eq(s => s.Id, session.Id);
            await _collection.ReplaceOneAsync(filter, session);
        }

    }
}
