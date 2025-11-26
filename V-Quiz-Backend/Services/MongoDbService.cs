using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace V_Quiz_Backend.Services
{
    public class MongoDbService
    {
        public IMongoDatabase Database { get; }

        public MongoDbService(IConfiguration config)
        {
            var connectionString = config["MongoDbConnection"];
            var dbName = config["MongoDbDatabase"];

            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(dbName);
        }
    }
}
