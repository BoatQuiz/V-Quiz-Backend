using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Tests.ServiceTests
{
    public class FlagServiceTests
    {
        [Fact]
        public async Task FlagQuestion_Should_ReturnSuccess_When_QuestionIsFlagged()
        {
            // Arrange
            var mockFlagRepo = new Mock<IFlagRepository>();
            var mockSessionService = new Mock<ISessionService>();
            var flagService = new FlagService(mockFlagRepo.Object, mockSessionService.Object);
            var request = new FlagRequestDto
            {
                QuestionId = "q1",
                SessionId = Guid.NewGuid(),
                Comment = "Inappropriate content"
            };
            mockFlagRepo
                .Setup(r => r.GetFlaggedQuestionByIdAsync("q1"))
                .ReturnsAsync((FlaggedQuestion?)null);
            mockFlagRepo
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);
            mockSessionService
                .Setup(s => s.GetUserIdBySessionIdAsync(request.SessionId))
                .ReturnsAsync(new ServiceResponse<SessionIdentity>
                {
                    Success = true,
                    Data = new SessionIdentity { UserId = Guid.NewGuid() }
                });
            // Act
            var result = await flagService.FlagQuestion(request);
            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q1", result.Data.QuestionId);
            Assert.Single(result.Data.Flags);
            Assert.Equal("Inappropriate content", result.Data.Flags[0].Comment);
        }
    }
}
