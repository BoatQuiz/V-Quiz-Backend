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
        public async Task<ServiceResponse<QuizProfile>> GetQuizProfileAsync(Guid? userId)
        {
            // Ej inloggad användare, returnera standardprofil
            if (!userId.HasValue)
            {
                var userProfile = new QuizProfile
                {
                    Audience = "general",
                    Categories = []
                };
                return ServiceResponse<QuizProfile>.Ok(userProfile, "Default quiz profile retrieved successfully");
            }

            var profile = await repo.GetQuizProfileAsync(userId.Value);
            if (profile == null)
            {
                return ServiceResponse<QuizProfile>.Fail("Quiz profile not found");
            }
            return ServiceResponse<QuizProfile>.Ok(profile, "Quiz profile retrieved successfully");
        }

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
            return ServiceResponse<UserId>.Ok(new UserId { Id = existingUser.UserId, Username = existingUser.Username }, "Login successful");
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
                QuizProfile = new QuizProfile
                {
                    Audience = "General",
                    Categories = []
                },
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

        public async Task<ServiceResponse<SessionUser>> GetSessionUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return ServiceResponse<SessionUser>.Fail("Invalid user ID");
            }
            var user = await repo.GetSessionUserAsync(userId);
            if (user == null)
            {
                return ServiceResponse<SessionUser>.Fail("User not found");
            }
            return ServiceResponse<SessionUser>.Ok(user, "Session user retrieved successfully");
        }

        public async Task<ServiceResponse> UpdateQuizProfileAsync(Guid? userId, QuizProfileDto profile)
        {
            if (!userId.HasValue)
            {
                return ServiceResponse.Fail("User Id is missing");
            }

            //if (!profile || !profile.Audience || !profile.Categories) 
            //{
            //    return ServiceResponse.Fail("Missing quizProfil");
            //}

            var user = await GetUserEntityAsync(userId.Value);
            if (user == null || user.Data == null)
            {
                return ServiceResponse.Fail("Could not find user");
            }
                user.Data.QuizProfile.Categories = profile.Categories;
                user.Data.QuizProfile.Audience = profile.Audience;
                user.Data.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateQuizProfileAsync(userId.Value, user.Data.QuizProfile);
            return ServiceResponse.Ok("QuizProfile updated");
        }

        public async Task<ServiceResponse<UserEntity>> GetUserEntityAsync(Guid userId)
        {
            var userEntity = await repo.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                return ServiceResponse<UserEntity>.Fail("User could not be found");
            }
            return ServiceResponse<UserEntity>.Ok(userEntity);
        }
    }
}
