using MongoDB.Driver;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Repository
{
    public class UserRepository(MongoDbService mongo) : IUserRepository
    {
        private readonly IMongoCollection<UserEntity> _collection = mongo.Database.GetCollection<UserEntity>("Users");

        public async Task CreateUserAsync(UserEntity user)
        {
            await _collection.InsertOneAsync(user);
            
            // Denna skulle kunna byggas om för att få en bekräftelde på att det inte blir fel
        }

        public async Task<QuizProfile> GetQuizProfileAsync(Guid userId)
        {
            return await _collection
                .Find(user => user.UserId == userId)
                .Project(user => new QuizProfile
                {
                    Audience = user.QuizProfile.Audience,
                    Categories = user.QuizProfile.Categories
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UserEntity> GetUserByNameAsync(string userName)
        {
            return await _collection
                .Find(user => user.Username == userName)
                .FirstOrDefaultAsync();
        }

        public async Task<SessionUser> GetSessionUserAsync(Guid userId)
        {
            return await _collection
                .Find(user => user.UserId == userId)
                .Project(user => new SessionUser
                {
                    UserId = user.UserId,
                    Audience = user.QuizProfile.Audience,
                    Categories = user.QuizProfile.Categories
                })
                .FirstOrDefaultAsync();
        }
    }
}
