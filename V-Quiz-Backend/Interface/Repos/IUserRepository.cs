using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Repos
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserByNameAsync(string userName);
        Task CreateUserAsync(UserEntity user);
        Task<QuizProfile> GetQuizProfileAsync(Guid userId);
        Task<SessionUser> GetSessionUserAsync(Guid userId);
    }
}
