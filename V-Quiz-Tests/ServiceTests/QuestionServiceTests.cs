using Moq;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Tests.ServiceTests
{
    public class QuestionServiceTests : TestBase
    {
        public QuestionServiceTests(Mock<IFlagRepository> mockFlagRepo, Mock<ISessionRepository> mockSessionRepo, Mock<IQuestionRepository> mockQuestionRepo) : base(mockFlagRepo, mockSessionRepo, mockQuestionRepo)
        {
        }

        [Fact]
        public async Task SubmitAnswer_Should_ReturnCorrect_When_AnswerIsCorrect()
        {
            // Arrange
            var mockSessionService = new Mock<ISessionService>();
            var mockRepo = new Mock<IQuestionRepository>();

            var session = new Session
            {
                Id = Guid.NewGuid(),
                NumCorrectAnswers = 0,
                NumQuestions = 0,
                IsCompleted = false,
                UsedQuestions = new List<string>()
            };

            mockSessionService
                .Setup(s => s.GetSessionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));

            mockSessionService
                .Setup(s => s.UpdateSessionAsync(It.IsAny<Session>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            var question = new Question
            {
                QuestionId = "q1",
                CorrectIndex = 1,
                Text = "Test?",
                Options = ["A", "B"]
            };

            mockRepo
                .Setup(r => r.GetQuestionByIdAsync("q1"))
                .ReturnsAsync(question);

            var questionService = new QuestionService(mockRepo.Object, mockSessionService.Object);
            var request = new SubmitAnswerRequest
            {
                SessionId = session.Id,
                QuestionId = "q1",
                SelectedAnswer = 1
            };

            // Act
            var result = await questionService.SubmitAnswerAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);


        }

        [Fact]
        public async Task GetRandomQuestion_Should_NotReturnUsededQuestion()
        {
            var service = new QuestionService(_mockQuestionRepo.Object);
        }


    }
}
