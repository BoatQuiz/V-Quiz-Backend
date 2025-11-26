using MongoDB.Driver;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Repository
{
    public class CustomerRepository
    {
        private readonly IMongoCollection<Customer> _collection;

        public CustomerRepository(MongoDbService mongo)
        {
            _collection = mongo.Database.GetCollection<Customer>("customers");
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
    }
}
