using Moq;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Tests.ServiceTests
{
    public class SessionServiceTests : TestBase
    {
        [Fact]
        public async Task CreateSession_ShouldReturnValidSessionId()
        {
            // Arrange
            SessionRepoMock.Setup(repo => repo.CreateSessionAsync(It.IsAny<Session>()))
                .ReturnsAsync(true);

            var service = new SessionService(SessionRepoMock.Object);
            // Act
            var result = await service.CreateSessionAsync();
            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
           
        }

        [Fact]
        public async Task CreateSession_Should_ReturnFail_WhenInsertFails()
        {
            // Arrange
            SessionRepoMock
                .Setup(r => r.CreateSessionAsync(It.IsAny<Session>()))
                .ReturnsAsync(false);

            var service = new SessionService(SessionRepoMock.Object);

            // Act
            var result = await service.CreateSessionAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task GetSessionById_Should_ReturnSuccess_WhenSessionExists()
        {
            // Arrange
            var session = new Session { Id = Guid.NewGuid() };

            SessionRepoMock
                .Setup(r => r.GetSessionAsync(session.Id))
                .ReturnsAsync(session);

            var service = new SessionService(SessionRepoMock.Object);

            // Act
            var result = await service.GetSessionByIdAsync(session.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(session.Id, result.Data.Id);
        }
        [Fact]
        public async Task GetSessionById_Should_ReturnFail_WhenSessionDoesNotExist()
        {
            // Arrange
            SessionRepoMock
                .Setup(r => r.GetSessionAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Session?)null);

            var service = new SessionService(SessionRepoMock.Object);

            // Act
            var result = await service.GetSessionByIdAsync(Guid.NewGuid());

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task UpdateSession_Should_ReturnSuccess_WhenUpdateSucceeds()
        {
            // Arrange
            var session = new Session { Id = Guid.NewGuid() };

            SessionRepoMock
                .Setup(r => r.UpdateSessionAsync(session))
                .Returns(Task.CompletedTask);

            var service = new SessionService(SessionRepoMock.Object);

            // Act
            var result = await service.UpdateSessionAsync(session);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
        }
        [Fact]
        public async Task UpdateSession_Should_ReturnFail_WhenExceptionThrown()
        {
            // Arrange
            var session = new Session { Id = Guid.NewGuid() };

            SessionRepoMock
                .Setup(r => r.UpdateSessionAsync(session))
                .ThrowsAsync(new Exception("DB error"));

            var service = new SessionService(SessionRepoMock.Object);

            // Act
            var result = await service.UpdateSessionAsync(session);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to update session: DB error", result.Message);
        }

        [Fact]
        public async Task SetCurrentQuestionAsync_Should_UpdateCurrentQuestion()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var question = new CurrentQuestionState
            {
                QuestionId = "q23",
                AskedAtUtc = DateTime.UtcNow
            };

            SessionRepoMock
                .Setup(r => r.SetCurrentQuestionAsync(sessionId, question));
            var service = new SessionService(SessionRepoMock.Object);

            // Act
            var result = await service.SetCurrentQuestionAsync(sessionId, question.QuestionId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Current question set successfully.", result.Message);
        }

        [Fact]
        public async Task SetCurrentQuestionAsync_Should_ReturnFail_WhenExceptionThrown()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var questionId = "q23";
            SessionRepoMock
                .Setup(r => r.SetCurrentQuestionAsync(sessionId, It.IsAny<CurrentQuestionState>()))
                .ThrowsAsync(new Exception("DB error"));
            var service = new SessionService(SessionRepoMock.Object);
            // Act
            var result = await service.SetCurrentQuestionAsync(sessionId, questionId);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to set current question: DB error", result.Message);
        }
    }
}
