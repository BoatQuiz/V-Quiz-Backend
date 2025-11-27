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

        
    }
}
