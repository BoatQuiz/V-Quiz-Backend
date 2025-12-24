using MongoDB.Driver;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Services
{
    public class UserService(IUserRepository repo, IPasswordHasher hasher) : IUserService
    {
        public async Task<ServiceResponse<UserId>> LoginUserAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return ServiceResponse<UserId>.Fail("Username and password cannot be empty");
            }

            var existingUser = await repo.GetUserByNameAsync(dto.Username);
            if (existingUser == null)
            {
                return ServiceResponse<UserId>.Fail("Invalid username or password");
            }

            if (!hasher.Verify(dto.Password, existingUser.PasswordHash))
            {
                return ServiceResponse<UserId>.Fail("Invalid username or password");
            }
            return ServiceResponse<UserId>.Ok(new UserId { Id = existingUser.UserId }, "Login successful");
        }

        public async Task<ServiceResponse<UserId>> RegisterUserAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return ServiceResponse<UserId>.Fail("Username and password cannot be empty");
            }
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
            try
            {
                await repo.CreateUserAsync(newUser);
            }
            // Denna delen finns inget test på för det kunde inte göras på ett snyggt sätt
            // Till V.1 kan man införa den
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return ServiceResponse<UserId>.Fail($"Error creating user: {ex.Message}");
            }
            return ServiceResponse<UserId>.Ok(new UserId { Id = newUser.UserId }, "User registered successfully");
        }
    }
}
