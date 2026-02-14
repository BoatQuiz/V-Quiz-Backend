using Moq;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;
using V_Quiz_Tests.Helpers;

namespace V_Quiz_Tests.ServiceTests
{
    public class QuizServiceTest : TestBase
    {
        [Fact]
        public async Task SubmitAnswer_ShouldReturnCorrect_WhenAnswerIsCorrect()
        {
            // Arrange
            // Använder SessionBuilder för att skapa en session med en aktuell fråga
            var session = new SessionBuilder()
                .WithCurrentQuestion(questionId: "q1", correctIndex: 2)
                .WithUsedQuestions(3)
                .Build();

            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));

            SessionServiceMock
                .Setup(s => s.AppendAnsweredQuestionAsync(
                    It.IsAny<Session>(),
                    It.IsAny<UsedQuestion>()))
                .Returns(Task.CompletedTask);

            // Skapar en fråga med korrekt svar på index 2
            var request = new SubmitAnswerRequestBuilder()
                .WithSessionId(session.Id)
                .WithQuestionId("q1")
                .WithSelectedAnswer(2)
                .Build();

            var quizService = new QuizService(SessionServiceMock.Object, QuestionServiceMock.Object);

            // Act
            var result = await quizService.SubmitAnswerAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data.IsCorrect);
        }
        [Fact]
        public async Task SubmitAnswer_ShouldReturnFail_WhenSessionIsNotSuccess()
        {
            // Arrange
            var invalidSessionId = Guid.NewGuid();

            var session = new SessionBuilder()
                .WithId(invalidSessionId)
                .Build();

            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Fail("Session not found."));

            var request = new SubmitAnswerRequestBuilder()
                .WithSessionId(invalidSessionId)
                .WithQuestionId("q1")
                .WithSelectedAnswer(1)
                .Build();

            var quizService = new QuizService(SessionServiceMock.Object, QuestionServiceMock.Object);

            // Act
            var result = await quizService.SubmitAnswerAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid session.", result.Message);
        }
        [Fact]
        public async Task SubmitAnswer_ShouldReturnFail_WhenSessionIsEnded()
        {
            // Arrange
            var session = new SessionBuilder()
                .Ended()
                .Build();
            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            var request = new SubmitAnswerRequestBuilder()
                .WithSessionId(session.Id)
                .WithQuestionId("q1")
                .WithSelectedAnswer(1)
                .Build();
            var quizService = new QuizService(SessionServiceMock.Object, QuestionServiceMock.Object);
            // Act
            var result = await quizService.SubmitAnswerAsync(request);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Session is already completed.", result.Message);
        }
        [Fact]
        public async Task SubmitAnswer_ShouldReturnFail_WhenNoActiveQuestion()
        {
            // Arrange
            var session = new SessionBuilder()
                .WithUsedQuestions(2)
                .Build();
            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            var request = new SubmitAnswerRequestBuilder()
                .WithSessionId(session.Id)
                .WithQuestionId("q1")
                .WithSelectedAnswer(1)
                .Build();
            var quizService = new QuizService(SessionServiceMock.Object, QuestionServiceMock.Object);
            // Act
            var result = await quizService.SubmitAnswerAsync(request);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("No active question.", result.Message);
        }
        [Fact]
        public async Task GetNextQuestionAsync_ShouldReturnNextQuestion_WhenSessionIdIsCorrect()
        {
            // Arrange
            var session = new SessionBuilder()
                .Build();

            var request = new SubmitSessionId { SessionId = session.Id };

            var responseDto = new QuestionResponseDtoBuilder()
                .WithId("q1")
                .WithText("What is the capital of France?")
                .WithOptions(
                    new List<string> { "Berlin", "Madrid", "Paris", "Rome" }, correctIndex: 2)
                .Build();


            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));

            QuestionServiceMock
                .Setup(s => s.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Ok(responseDto));

            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.GetNextQuestionAsync(request);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q1", result.Data.Question.QuestionId);
            Assert.Equal("What is the capital of France?", result.Data.Question.QuestionText);
            Assert.Equal(4, result.Data.Question.Options.Count);
        }
        [Fact]
        public async Task GetNextQuestionAsync_ShouldReturnFail_WhenSessionIdIsNull()
        {
            // Arrange
            var sessionId = new SubmitSessionId { SessionId = Guid.NewGuid() };

            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(sessionId.SessionId))
                .ReturnsAsync(ServiceResponse<Session>.Fail("Session not found."));

            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.GetNextQuestionAsync(sessionId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid session.", result.Message);
        }
        [Fact]
        public async Task GetNextQuestionAsync_ShouldReturnFail_WhenSessionDataIsNull()
        {
            // Arrange
            var sessionId = new SubmitSessionId { SessionId = Guid.NewGuid() };
            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(sessionId.SessionId))
                .ReturnsAsync(ServiceResponse<Session>.Ok(null!));
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);
            // Act
            var result = await quizService.GetNextQuestionAsync(sessionId);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid session.", result.Message);
        }
        [Fact]
        public async Task GetNextQuestionAsync_ShouldReturnFail_WhenSessionIsEnded()
        {
            // Arrange
            var session = new SessionBuilder()
                .Ended()
                .Build();
            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            var request = new SubmitSessionId { SessionId = session.Id };
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.GetNextQuestionAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Session is already completed.", result.Message);
        }
        [Fact]
        public async Task GetNextQuestionAsync_ShouldReturnFail_WhenThereIsActiveQuestion()
        {
            // Arrange
            var session = new SessionBuilder()
                .WithCurrentQuestion(questionId: "q1", correctIndex: 1)
                .Build();
            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            var request = new SubmitSessionId { SessionId = session.Id };
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.GetNextQuestionAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("There is already an active question.", result.Message);
        }
        [Fact]
        public async Task GetNextQuestionAsync_ShouldReturnFail_WhenQuestionRetrievalFails()
        {
            // Arrange
            var session = new SessionBuilder()
                .Build();
            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            QuestionServiceMock
                .Setup(s => s.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Fail("No questions available."));
            var request = new SubmitSessionId { SessionId = session.Id };
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.GetNextQuestionAsync(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to retrieve question.", result.Message);
        }
        [Fact]
        public async Task GetNextQuestionAsync_ShouldReturnFail_WhenQuestionDataIsNull()
        {
            // Arrange
            var session = new SessionBuilder()
                .Build();
            SessionServiceMock
                .Setup(s => s.GetSessionByIdAsync(session.Id))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            QuestionServiceMock
                .Setup(s => s.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Ok(null!));
            var request = new SubmitSessionId { SessionId = session.Id };
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);
            // Act
            var result = await quizService.GetNextQuestionAsync(request);
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to retrieve question.", result.Message);
        }
        [Fact]
        public async Task StartQuizAsync_ShouldReturnQuestionResponse_WhenStartQuizAsyncIsRun()
        {
            var session = new SessionBuilder()
                .WithUsedQuestions(0)
                .Build();

            var responseDto = new QuestionResponseDtoBuilder()
                .WithId("q1")
                .WithText("What is the capital of France?")
                .WithOptions(
                    new List<string> { "Berlin", "Madrid", "Paris", "Rome" }, correctIndex: 2)
                .Build();

            SessionServiceMock
                .Setup(s => s.CreateSessionAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>?>()))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));

            QuestionServiceMock
                .Setup(q => q.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Ok(responseDto));

            SessionServiceMock
                .Setup(s => s.SetCurrentQuestionAsync(
                    session.Id,
                    It.IsAny<QuestionResponseDto>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(true));

            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.StartQuizAsync();

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("q1", result.Data.Question.QuestionId);
            Assert.Equal(0, result.Data.Session.QuestionsAnswered);



        }
        [Fact]
        public async Task StartQuizAsync_ShouldReturnFail_WhenCreateSessionFails()
        {
            // Arrange
            SessionServiceMock
                .Setup(s => s.CreateSessionAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>?>()))
                .ReturnsAsync(ServiceResponse<Session>.Fail("Failed to create session."));
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.StartQuizAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to create session.", result.Message);
        }
        [Fact]
        public async Task StartQuizAsync_ShouldReturnFail_WhenSessionDataIsNull()
        {
            // Arrange
            SessionServiceMock
                .Setup(s => s.CreateSessionAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>?>()))
                .ReturnsAsync(ServiceResponse<Session>.Ok(null!));
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);
            // Act
            var result = await quizService.StartQuizAsync();
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to create session.", result.Message);
        }
        [Fact]
        public async Task StartQuizAsync_ShouldReturnFail_WhenQuestionRetrievalFails()
        {
            // Arrange
            var session = new SessionBuilder()
                .WithUsedQuestions(0)
                .Build();
            SessionServiceMock
                .Setup(s => s.CreateSessionAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>?>()))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            QuestionServiceMock
                .Setup(q => q.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Fail("No questions available."));
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);
            // Act
            var result = await quizService.StartQuizAsync();
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to retrieve question.", result.Message);
        }
        [Fact]
        public async Task StartQuizAsync_ShouldReturnFail_WhenQuestionDataIsNull()
        {
            // Arrange
            var session = new SessionBuilder()
                .WithUsedQuestions(0)
                .Build();
            SessionServiceMock
                .Setup(s => s.CreateSessionAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>?>()))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            QuestionServiceMock
                .Setup(q => q.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Ok(null!));
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);
            // Act
            var result = await quizService.StartQuizAsync();
            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to retrieve question.", result.Message);
        }
        [Fact]
        public async Task StartQuizAsync_ShouldReturnFail_WhenSetCurrentQuestionFails()
        {
            // Arrange
            var session = new SessionBuilder()
                .WithUsedQuestions(0)
                .Build();
            var responseDto = new QuestionResponseDtoBuilder()
                .WithId("q1")
                .WithText("What is the capital of France?")
                .WithOptions(
                    new List<string> { "Berlin", "Madrid", "Paris", "Rome" }, correctIndex: 2)
                .Build();
            SessionServiceMock
                .Setup(s => s.CreateSessionAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>?>()))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            QuestionServiceMock
                .Setup(q => q.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Ok(responseDto));
            SessionServiceMock
                .Setup(s => s.SetCurrentQuestionAsync(
                    session.Id,
                    It.IsAny<QuestionResponseDto>()))
                .ReturnsAsync(ServiceResponse<bool>.Fail("Failed to set current question."));
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.StartQuizAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to set current question in session.", result.Message);
        }
        [Fact]
        public async Task StartQuizAsync_ShouldReturnFail_WhenSetCurrentQuestionDataIsFalse()
        {
            // Arrange
            var session = new SessionBuilder()
                .WithUsedQuestions(0)
                .Build();
            var responseDto = new QuestionResponseDtoBuilder()
                .WithId("q1")
                .WithText("What is the capital of France?")
                .WithOptions(
                    new List<string> { "Berlin", "Madrid", "Paris", "Rome" }, correctIndex: 2)
                .Build();
            SessionServiceMock
                .Setup(s => s.CreateSessionAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<int>(),
                    It.IsAny<List<string>?>()))
                .ReturnsAsync(ServiceResponse<Session>.Ok(session));
            QuestionServiceMock
                .Setup(q => q.GetRandomQuestionAsync(session))
                .ReturnsAsync(ServiceResponse<QuestionResponseDto>.Ok(responseDto));
            SessionServiceMock
                .Setup(s => s.SetCurrentQuestionAsync(
                    session.Id,
                    It.IsAny<QuestionResponseDto>()))
                .ReturnsAsync(ServiceResponse<bool>.Ok(false));
            var quizService = new QuizService(
                SessionServiceMock.Object,
                QuestionServiceMock.Object);

            // Act
            var result = await quizService.StartQuizAsync();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to set current question in session.", result.Message);
        }

        [Fact]
        public async Task GetQuizMetaDataAsync()
        {

            // Arrange
            var metaData = new QuizMetaDataDto
            {
                Audiences = new List<AudienceMetaDto>
                {
                    new AudienceMetaDto
                    {

                    Name = "Shipping", Categories = ["Language"]
                    }
                }
            };

            var serviceResponse = ServiceResponse<QuizMetaDataDto>.Ok(metaData);

            QuestionServiceMock
                .Setup(q => q.GetQuizMetaDataAsync())
                .ReturnsAsync(serviceResponse);
            
            var quizService = new QuizService(
               SessionServiceMock.Object,
               QuestionServiceMock.Object);

            // Act
            var result = await quizService.GetQuizMetaDataAsync();

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Shipping", result.Data.Audiences[0].Name);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Audiences);
        }

    }
}
