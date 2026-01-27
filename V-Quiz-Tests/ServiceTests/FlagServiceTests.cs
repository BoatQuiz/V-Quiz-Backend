using Moq;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Tests.ServiceTests
{
    public class FlagServiceTests : TestBase
    {
        [Fact]
        public async Task FlagQuestion_ShouldReturnSuccess_WhenQuestionIsFlagged()
        {
            // Arrange
            var request = new FlagRequestDto
            {
                QuestionId = "q1",
                SessionId = Guid.NewGuid(),
                Comment = "Inappropriate content"
            };
            FlagRepoMock
                .Setup(r => r.GetFlaggedQuestionByIdAsync("q1"))
                .ReturnsAsync((FlaggedQuestion?)null);
            FlagRepoMock
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);
            SessionServiceMock
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(new ServiceResponse<SessionIdentity>
                {
                    Success = true,
                    Data = new SessionIdentity { UserId = Guid.NewGuid() }
                });

            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);

            // Act
            var result = await flagService.FlagQuestion(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q1", result.Data.QuestionId);
            Assert.Single(result.Data.Flags);
            Assert.Equal("Inappropriate content", result.Data.Flags[0].Comment);
        }
        [Fact]
        public async Task FlagQuestion_ShouldReturnFail_WhenRequestIsInvalid()
        {
            // Arrange
            var flagReq = new FlagRequestDto
            {
                QuestionId = "",
                SessionId = Guid.Empty,
                Reasons = new List<FlagReason>()
            };

            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);

            // Act
            var result = await flagService.FlagQuestion(flagReq);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid flag request data.", result.Message);
        }
        [Fact]
        public async Task FlagQuestion_ShouldReturnFail_WhenRequestIsNull()
        {
            // Arrange
            FlagRequestDto? flagReq = null;
            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);
            // Act
            var result = await flagService.FlagQuestion(flagReq);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid flag request data.", result.Message);
        }
        [Fact]
        public async Task FlagQuestion_ShouldStoreUserId_WhenUserIsLoggedIn()
        {
            // Arrange
            var request = new FlagRequestDto
            {
                QuestionId = "q2",
                SessionId = Guid.NewGuid(),
                Comment = "Offensive content"
            };
            var userId = Guid.NewGuid();
            SessionServiceMock
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(ServiceResponse<SessionIdentity>.Ok(new SessionIdentity { UserId = userId }));

            FlagRepoMock
                .Setup(r => r.GetFlaggedQuestionByIdAsync("q2"))
                .ReturnsAsync((FlaggedQuestion?)null);

            FlagRepoMock
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);

            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);

            // Act
            var result = await flagService.FlagQuestion(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q2", result.Data.QuestionId);
            Assert.Single(result.Data.Flags);
            Assert.Equal(userId, result.Data.Flags[0].UserId);
        }
        [Fact]
        public async Task FlagQuestion_ShouldAllowAnonymousFlagging_WhenUserIsNotLoggedIn()
        {
            // Arrange
            var request = new FlagRequestDto
            {
                QuestionId = "q3",
                SessionId = Guid.NewGuid(),
                Comment = "Misleading content"
            };

            SessionServiceMock
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(ServiceResponse<SessionIdentity>.Ok(new SessionIdentity { UserId = null }));

            FlagRepoMock
                .Setup(r => r.GetFlaggedQuestionByIdAsync("q3"))
                .ReturnsAsync((FlaggedQuestion?)null);

            FlagRepoMock
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);

            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);

            // Act
            var result = await flagService.FlagQuestion(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q3", result.Data.QuestionId);
            Assert.Single(result.Data.Flags);
            Assert.Null(result.Data!.Flags.First().UserId);
        }
        [Fact]
        public async Task FlagQuestion_ShouldReturnFail_WhenUserIsNotSuccessfull()
        {
            // Arrange
            var request = new FlagRequestDto
            {
                QuestionId = "q4",
                SessionId = Guid.NewGuid(),
                Comment = "Inappropriate content"
            };
            SessionServiceMock
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(ServiceResponse<SessionIdentity>.Fail("Invalid session"));
            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);

            // Act
            var result = await flagService.FlagQuestion(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid session. Unable to identify user.", result.Message);

        }
        [Fact]
        public async Task FlagQuestion_ShouldReturnFail_WhenNoQuestionIsReturned()
        {
            // Arrange
            var request = new FlagRequestDto
            {
                QuestionId = "nonexistent_question",
                SessionId = Guid.NewGuid(),
                Comment = "Spam content"
            };

            var identity = new SessionIdentity { UserId = Guid.NewGuid() };
            FlagRepoMock
                .Setup(r => r.GetFlaggedQuestionByIdAsync("nonexistent_question"))
                .ReturnsAsync((FlaggedQuestion?)null);
            SessionServiceMock
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(ServiceResponse<SessionIdentity>.Ok(identity));

            FlagRepoMock
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);

            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);

            // Act
            var result = await flagService.FlagQuestion(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("nonexistent_question", result.Data.QuestionId);
            Assert.Single(result.Data.Flags);
            Assert.Equal("Spam content", result.Data.Flags[0].Comment);
        }
        [Fact]
        public async Task FlagQuestion_ShouldSetDefaultReason_WhenNoReasonsProvided()
        {
            // Arrange
            var request = new FlagRequestDto
            {
                QuestionId = "q5",
                SessionId = Guid.NewGuid(),
                Comment = "No reason provided"
            };

            SessionServiceMock
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(ServiceResponse<SessionIdentity>.Ok(new SessionIdentity { UserId = Guid.NewGuid() }));

            FlagRepoMock
                .Setup(r => r.GetFlaggedQuestionByIdAsync("q5"))
                .ReturnsAsync((FlaggedQuestion?)null);

            FlagRepoMock
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);

            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);

            // Act
            var result = await flagService.FlagQuestion(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q5", result.Data.QuestionId);
            Assert.Single(result.Data.Flags);
            Assert.Contains(FlagReason.Other, result.Data.Flags[0].Reason);
        }
        [Fact]
        public async Task FlagQuestion_ShouldAddToExistingFlaggedQuestion_WhenItExists()
        {
            // Arrange
            var existingFlaggedQuestion = new FlaggedQuestion
            {
                QuestionId = "q6",
                Flags = new List<FlagEntry>
                {
                    new FlagEntry
                    {
                        UserId = Guid.NewGuid(),
                        Reason = new List<FlagReason> { FlagReason.SpellingError },
                        Comment = "Spam content",
                        FlaggedAt = DateTime.UtcNow.AddDays(-1)
                    }
                },
                IsResolved = false
            };
            var request = new FlagRequestDto
            {
                QuestionId = "q6",
                SessionId = Guid.NewGuid(),
                Comment = "Offensive content",
                Reasons = new List<FlagReason> { FlagReason.WrongAnswer }
            };
            SessionServiceMock
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(ServiceResponse<SessionIdentity>.Ok(new SessionIdentity { UserId = Guid.NewGuid() }));
            FlagRepoMock
                .Setup(r => r.GetFlaggedQuestionByIdAsync("q6"))
                .ReturnsAsync(existingFlaggedQuestion);
            FlagRepoMock
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);
            var flagService = new FlagService(FlagRepoMock.Object, SessionServiceMock.Object);
            // Act
            var result = await flagService.FlagQuestion(request);
            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q6", result.Data.QuestionId);
            Assert.Equal(2, result.Data.Flags.Count);
            Assert.Equal("Offensive content", result.Data.Flags[1].Comment);
        }
    }
}
