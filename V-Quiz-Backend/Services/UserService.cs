using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;
using BCrypt.Net;
using V_Quiz_Backend.Interface;

namespace V_Quiz_Backend.Services
{
    public class UserService(IUserRepository repo, IPasswordHasher hasher) : IUserService
    {
        public Task<ServiceResponse<UserId>> LoginUserAsync(LoginDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<UserId>> RegisterUserAsync(LoginDto dto)
        {
            // Kontrollera om användarnamnet redan finns
            var existingUser = await repo.GetUserByNameAsync(dto.Username);
            if (existingUser != null)
            {
                return ServiceResponse<UserId>.Fail("Username already exists");
            }

            var newUser = new UserEntity
            {
                UserId = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = hasher.Hash(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await repo.CreateUserAsync(newUser);
            return ServiceResponse<UserId>.Ok(new UserId { Id = newUser.UserId }, "User registered successfully");
        }
    }
}
