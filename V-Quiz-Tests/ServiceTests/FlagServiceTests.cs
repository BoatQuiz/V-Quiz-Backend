using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
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
            var flagService = new FlagService(mockFlagRepo.Object);
            var request = new FlagQuestionRequestDto
            {
                QuestionId = "q1",
                UserId = "user1",
                Comment = "Inappropriate content"
            };
            mockFlagRepo
                .Setup(r => r.GetFlaggedQuestionByIdAsync("q1"))
                .ReturnsAsync((FlaggedQuestion?)null);
            mockFlagRepo
                .Setup(r => r.AddFlaggedQuestionAsync(It.IsAny<FlaggedQuestion>()))
                .Returns(Task.CompletedTask);
            // Act
            var result = await flagService.FlagQuestion(request);
            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q1", result.Data.QuestionId);
            Assert.Single(result.Data.Flags);
            Assert.Equal("user1", result.Data.Flags[0].UserId);
            Assert.Equal("Inappropriate content", result.Data.Flags[0].Comment);
        }
    }
}
