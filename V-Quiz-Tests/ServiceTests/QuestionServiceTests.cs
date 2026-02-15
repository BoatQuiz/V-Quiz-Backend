using Moq;
using V_Quiz_Backend.DTO;
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
                Category = "Geography"
            };

            var filter = new QuestionFilter
            {
                ExcludedQuestionIds = [],
                AllowedCategories = null,
                Difficulty = null,
                Audience = ""
            };

            var session = new Session
            {
                Player = new SessionUser { Audience = "general", Categories = ["history", "Geography"] },
                UsedQuestions = new List<UsedQuestion>()
            };

            QuestionRepoMock
                .Setup(r => r.GetRandomQuestionAsync(It.IsAny<QuestionFilter>()))
                .ReturnsAsync(question);

            var service = new QuestionService(QuestionRepoMock.Object);

            // Act
            var result = await service.GetRandomQuestionAsync(session);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("q2", result.Data.QuestionId);
        }
        [Fact]
        public async Task GetRandomQuestion_ShouldReturnFail_WhenNoQuestionsAvailable()
        {
            var filter = new QuestionFilter
            {
                ExcludedQuestionIds = [],
                AllowedCategories = null,
                Difficulty = null,
                Audience = ""
            };

            var session = new Session
            {
                Player = new SessionUser { Audience = "general", Categories = ["science"] },
                UsedQuestions = new List<UsedQuestion>()
            };

            // Arrange
            QuestionRepoMock
                .Setup(r => r.GetRandomQuestionAsync(filter))
                .ReturnsAsync((Question?)null);

            var service = new QuestionService(QuestionRepoMock.Object);

            // Act
            var result = await service.GetRandomQuestionAsync(session);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
        }
        [Fact]
        public void ShuffleQuestion_Should_RandomizeOptions_And_UpdateCorrectIndex()
        {
            // Arrange
            var question = new QuestionResponseDto
            {
                QuestionId = "q3",
                QuestionText = "What is 2 + 2?",
                Options = new List<string> { "3", "4", "5", "6" },
                CorrectIndex = 1
            };
            // Act
            var shuffledQuestion = QuestionService.ShuffleQuestion(question);
            // Assert
            Assert.Equal(4, shuffledQuestion.Options.Count);
            Assert.Contains("4", shuffledQuestion.Options);
            Assert.InRange(shuffledQuestion.CorrectIndex, 0, 3);
            Assert.Equal("4", shuffledQuestion.Options[shuffledQuestion.CorrectIndex]);
        }

        [Fact]
        public async Task GetQuizMetaDataAsync_Should_ReturnCorrectIndex()
        {
            // Arrange
            var projection = new List<QuizMetadataProjection>
            {   new()
                {
                    Audience = "General",
                    Category = "Language"
                },
                new() {
                Audience = "Shipping",
                Category = "Navigation"
                }
            };
            QuestionRepoMock
                .Setup(r => r.GetQuizMetaDataAsync())
                .ReturnsAsync(projection);

            var service = new QuestionService(QuestionRepoMock.Object);
            
            // Act
            var result = await service.GetQuizMetaDataAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("General", result.Data.Audiences[0].Name);
        }
        [Fact]
        public async Task GetQuizMetaDataAsync_IfRawDataIsNull_ShouldReturnFail() {
            QuestionRepoMock
                    .Setup(r => r.GetQuizMetaDataAsync())
                    .ReturnsAsync(new List<QuizMetadataProjection>());

            var service = new QuestionService (QuestionRepoMock.Object);

            var result = await service.GetQuizMetaDataAsync();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data.Audiences);
        }
    }
}


