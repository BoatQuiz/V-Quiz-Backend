using Moq;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Tests.ServiceTests
{
    public class UserServiceTests : TestBase
    {
        [Fact]
        public async Task RegisterUserAsync_ReturnSuccess_WhenUsernameIsUnique()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "newuser",
                Password = "password123"
            };

            UserRepoMock
                .Setup(repo => repo.GetUserByNameAsync(dto.Username))
                .ReturnsAsync((UserEntity?)null);

            UserRepoMock
                .Setup(repo => repo.CreateUserAsync(It.IsAny<UserEntity>()))
                .Returns(Task.CompletedTask);

            var service = new UserService(UserRepoMock.Object, PasswordHasherMock.Object);
            // Act
            var result = await service.RegisterUserAsync(dto);
            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("User registered successfully", result.Message);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnFail_WhenUsernameExists()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "existinguser",
                Password = "password123"
            };
            var existingUser = new UserEntity
            {
                UserId = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            UserRepoMock
                .Setup(repo => repo.GetUserByNameAsync(dto.Username))
                .ReturnsAsync(existingUser);
            var service = new UserService(UserRepoMock.Object, PasswordHasherMock.Object);

            // Act
            var result = await service.RegisterUserAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Username already exists", result.Message);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnFail_WhenUsernameOrPasswordIsEmpty()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "",
                Password = "password123"
            };
            var service = new UserService(UserRepoMock.Object, PasswordHasherMock.Object);

            // Act
            var result = await service.RegisterUserAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Username and password cannot be empty", result.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "validuser",
                Password = "validpassword"
            };
            var existingUser = new UserEntity
            {
                UserId = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            UserRepoMock
                .Setup(repo => repo.GetUserByNameAsync(dto.Username))
                .ReturnsAsync(existingUser);
            PasswordHasherMock
                .Setup(hasher => hasher.Verify(dto.Password, existingUser.PasswordHash))
                .Returns(true);
            var service = new UserService(UserRepoMock.Object, PasswordHasherMock.Object);

            // Act
            var result = await service.LoginUserAsync(dto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Login successful", result.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnFail_WhenCredentialsAreInvalid()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "invaliduser",
                Password = "wrongpassword"
            };
            UserRepoMock
                .Setup(repo => repo.GetUserByNameAsync(dto.Username))
                .ReturnsAsync((UserEntity?)null);
            var service = new UserService(UserRepoMock.Object, PasswordHasherMock.Object);
            // Act
            var result = await service.LoginUserAsync(dto);
            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Invalid username or password", result.Message);
        }
        
        [Fact]
        public async Task LoginUserAsync_ReturnFail_WhenUsernameOrPasswordIsEmpty()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "",
                Password = ""
            };
            var service = new UserService(UserRepoMock.Object, PasswordHasherMock.Object);
            // Act
            var result = await service.LoginUserAsync(dto);
            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Username and password cannot be empty", result.Message);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnFail_WhenPasswordAreInCorrect()
        {
            // Arrange
            var dto = new LoginDto
            {
                Username = "validuser",
                Password = "validpassword"
            };
            var existingUser = new UserEntity
            {
                UserId = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = "hashedpassword",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            UserRepoMock
                .Setup(repo => repo.GetUserByNameAsync(dto.Username))
                .ReturnsAsync(existingUser);
            PasswordHasherMock
                .Setup(hasher => hasher.Verify(dto.Password, existingUser.PasswordHash))
                .Returns(false);
            var service = new UserService(UserRepoMock.Object, PasswordHasherMock.Object);

            // Act
            var result = await service.LoginUserAsync(dto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Invalid username or password", result.Message);
        }
    }

}
