using Moq;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Tests.ServiceTests
{
    public class QuestionServiceTests : TestBase
    {
        [Fact]
        public async Task GetQuestionByIdAsync_ReturnSuccess_WhenQuestionExists()
        {
            var question = new Question
            {
                QuestionId = "q1",
                Text = "What is the capital of France?",
                Options = new List<string> { "Paris", "London", "Berlin", "Madrid" },
                CorrectIndex = 0
            };

            QuestionRepoMock
                .Setup(repo => repo.GetQuestionByIdAsync("q1"))
                .ReturnsAsync(question);

            var service = new QuestionService(QuestionRepoMock.Object);

            // Act
            var result = await service.GetQuestionByIdAsync("q1");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q1", result.Data!.QuestionId);
        }
        [Fact]
        public async Task GetQuestionByIdAsync_ReturnFail_WhenQuestionDoesNotExist()
        {
            QuestionRepoMock
                .Setup(repo => repo.GetQuestionByIdAsync("q2"))
                .ReturnsAsync((Question?)null);
            var service = new QuestionService(QuestionRepoMock.Object);
            // Act
            var result = await service.GetQuestionByIdAsync("q2");
            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Question not found.", result.Message);

        }

        [Fact]
        public async Task GetRandomQuestion_Should_ReturnQuestion_WhenAvailable()
        {
            // Arrange
            var question = new Question
            {
                QuestionId = "q2",
                Text = "Capital of Sweden?",
                Options = ["Stockholm", "Oslo"],
                CorrectIndex = 0,
                Category = "Geography",
            };

            QuestionRepoMock
                .Setup(r => r.GetRandomQuestionAsync(
                    It.IsAny<IEnumerable<string>>(), 
                    It.IsAny<IEnumerable<string>?>()
                ))
                .ReturnsAsync(question);

            var service = new QuestionService(QuestionRepoMock.Object);

            // Act
            var result = await service.GetRandomQuestionAsync(new List<string>());

            // Assert
            Assert.True(result.Success);
            Assert.Equal("q2", result.Data.QuestionId);
        }

        [Fact]
        public async Task GetRandomQuestion_Should_ReturnFail_WhenNoQuestionsAvailable()
        {
            // Arrange
            QuestionRepoMock
                .Setup(r => r.GetRandomQuestionAsync(
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<IEnumerable<string>?>()
                    ))
                .ReturnsAsync((Question?)null);

            var service = new QuestionService(QuestionRepoMock.Object);

            // Act
            var result = await service.GetRandomQuestionAsync(new List<string>());

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }
    }
}


   