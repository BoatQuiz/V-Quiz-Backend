using Moq;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Tests.ServiceTests
{
    public class SessionServiceTests : TestBase
    {
        [Fact]
        public async Task CreateSession_ShouldReturn_ValidSessionId()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };
            // Arrange
            SessionRepoMock.Setup(repo => repo.CreateSessionAsync(It.IsAny<Session>()))
                .ReturnsAsync(true);

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));


            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);
            // Act
            var result = await service.CreateSessionAsync(userId);
            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);

        }
        [Fact]
        public async Task CreateSession_Should_ReturnFail_WhenInsertFails()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };
            // Arrange
            SessionRepoMock
                .Setup(r => r.CreateSessionAsync(It.IsAny<Session>()))
                .ReturnsAsync(false);

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.CreateSessionAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateSession_ShouldReturnFail_WhenExeption()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]
            };
            // Arrange
            SessionRepoMock
                .Setup(r => r.CreateSessionAsync(It.IsAny<Session>()))
                .ThrowsAsync(new Exception("DB error"));

            UserServiceMock
                .Setup(svc => svc.GetQuizProfileAsync(userId))
                .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.CreateSessionAsync(userId);
            
            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Failed to create session: DB error", result.Message);
        }

        [Fact]
        public async Task CreateSession_ShouldReturnFail_WhenInsertionSuccededIsFail()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]
            };
            // Arrange
            SessionRepoMock
                .Setup(r => r.CreateSessionAsync(It.IsAny<Session>()))
                .ReturnsAsync(false);

            UserServiceMock
                .Setup(svc => svc.GetQuizProfileAsync(userId))
                .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.CreateSessionAsync(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Failed to create session.", result.Message);
        }
        [Fact]
        public async Task GetSessionById_Should_ReturnSuccess_WhenSessionExists()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };

            // Arrange
            var session = new Session { Id = Guid.NewGuid() };

            SessionRepoMock
                .Setup(r => r.GetSessionAsync(session.Id))
                .ReturnsAsync(session);

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));


            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.GetSessionByIdAsync(session.Id);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(session.Id, result.Data.Id);
        }
        [Fact]
        public async Task GetSessionById_Should_ReturnFail_WhenSessionDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };
            // Arrange
            SessionRepoMock
                .Setup(r => r.GetSessionAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Session?)null);

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.GetSessionByIdAsync(Guid.NewGuid());

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task UpdateSession_Should_ReturnSuccess_WhenUpdateSucceeds()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };



            // Arrange
            var session = new Session { Id = Guid.NewGuid() };

            SessionRepoMock
                .Setup(r => r.UpdateSessionAsync(session))
                .Returns(Task.CompletedTask);

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                            .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.UpdateSessionAsync(session);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
        }
        [Fact]
        public async Task UpdateSession_Should_ReturnFail_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };


            // Arrange
            var session = new Session { Id = Guid.NewGuid() };

            SessionRepoMock
                .Setup(r => r.UpdateSessionAsync(session))
                .ThrowsAsync(new Exception("DB error"));

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                            .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.UpdateSessionAsync(session);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to update session: DB error", result.Message);
        }
        [Fact]
        public async Task SetCurrentQuestionAsync_Should_UpdateCurrentQuestion()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };



            // Arrange
            var sessionId = Guid.NewGuid();
            var question = new CurrentQuestionState
            {
                QuestionId = "q23",
                QuestionText = "What is the capital of France?",
                ShuffledOptions = new List<string> { "Berlin", "Madrid", "Paris", "Rome" },
                CorrectIndex = 2,
                AskedAtUtc = DateTime.UtcNow
            };

            var questionResponse = new QuestionResponseDto
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                Options = question.ShuffledOptions,
                CorrectIndex = question.CorrectIndex,
            };

            SessionRepoMock
                .Setup(r => r.SetCurrentQuestionAsync(sessionId, question));

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                            .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            var result = await service.SetCurrentQuestionAsync(sessionId, questionResponse);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Current question set successfully.", result.Message);
        }
        [Fact]
        public async Task SetCurrentQuestionAsync_Should_ReturnFail_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            var profile = new QuizProfile
            {
                Audience = "general",
                Categories = ["science", "history"]

            };

            // Arrange
            var sessionId = Guid.NewGuid();
            var question = new CurrentQuestionState
            {
                QuestionId = "q23",
                QuestionText = "What is the capital of France?",
                ShuffledOptions = new List<string> { "Berlin", "Madrid", "Paris", "Rome" },
                CorrectIndex = 2,
                AskedAtUtc = DateTime.UtcNow
            };

            var questionResponse = new QuestionResponseDto
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                Options = question.ShuffledOptions,
                CorrectIndex = question.CorrectIndex,
            };

            SessionRepoMock
                .Setup(r => r.SetCurrentQuestionAsync(sessionId, It.IsAny<CurrentQuestionState>()))
                .ThrowsAsync(new Exception("DB error"));

            UserServiceMock.Setup(svc => svc.GetQuizProfileAsync(userId))
                            .ReturnsAsync(ServiceResponse<QuizProfile>.Ok(profile));

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);
            // Act
            var result = await service.SetCurrentQuestionAsync(sessionId, questionResponse);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to set current question: DB error", result.Message);
        }
        [Fact]
        public async Task AppendAnsweredQuestionAsync_WhenLastQuestion_SetsEndSessionTrue()
        {
            // Arrange
            Session session = new()
            {
                Id = Guid.NewGuid(),
                TargetQuestionCount = 5,
                UsedQuestions =
                [
                    new() { QuestionId = "q1" },
                    new() { QuestionId = "q2" },
                    new() { QuestionId = "q3" },
                    new() { QuestionId = "q4" },
                ]
            };

            UsedQuestion question = new();

            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            await service.AppendAnsweredQuestionAsync(session, question);

            // Assert
            SessionRepoMock.Verify(r =>
            r.AppendUsedQuestionAsync(
                session.Id,
                question, true
            ), Times.Once);
        }
        [Fact]
        public async Task AppendAnsweredQuestionAsync_WhenNotLastQuestion_SetsEndSessionFalse()
        {
            // Arrange
            var session = new Session
            {
                Id = Guid.NewGuid(),
                TargetQuestionCount = 5,
                UsedQuestions =
                [
                    new() { QuestionId = "q1" },
                    new() { QuestionId = "q2" },
                    new() { QuestionId = "q3" },
                ]
            };

            var question = new UsedQuestion();
            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);

            // Act
            await service.AppendAnsweredQuestionAsync(session, question);
            // Assert
            SessionRepoMock.Verify(r =>
            r.AppendUsedQuestionAsync(
                session.Id,
                question, false
            ), Times.Once);
        }
        [Fact]
        public async Task GetUserIdBySessionIdAsync_Should_ReturnFail_WhenSessionIdIsEmpty()
        {
            // Arrange
            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);
            
            // Act
            var result = await service.GetUserIdBySessionIdAsync(Guid.Empty);
            
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid session ID.", result.Message);
        }
        [Fact]
        public async Task GetUserIdBySessionIdAsync_Should_ReturnFail_WhenSessionNotFound()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            SessionRepoMock
                .Setup(r => r.GetUserIdBySessionIdAsync(sessionId))
                .ReturnsAsync((SessionIdentity?)null);
            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);
            
            // Act
            var result = await service.GetUserIdBySessionIdAsync(sessionId);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Session not found.", result.Message);
        }
        [Fact]
        public async Task GetUserIdBySessionIdAsync_Should_ReturnSuccess_WhenSessionFound()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var expectedIdentity = new SessionIdentity
            {
                UserId = Guid.NewGuid(),
                UserName = "testuser"
            };
            SessionRepoMock
                .Setup(r => r.GetUserIdBySessionIdAsync(sessionId))
                .ReturnsAsync(expectedIdentity);
            var service = new SessionService(SessionRepoMock.Object, UserServiceMock.Object);
            
            // Act
            var result = await service.GetUserIdBySessionIdAsync(sessionId);
            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedIdentity.UserId, result.Data.UserId);
            Assert.Equal(expectedIdentity.UserName, result.Data.UserName);
        }
    }

}
