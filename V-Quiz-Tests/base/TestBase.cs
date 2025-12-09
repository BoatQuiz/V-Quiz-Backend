using Moq;
using V_Quiz_Backend.Interface.Repos;

namespace V_Quiz_Tests
{
    public abstract class TestBase
    {
        protected readonly Mock<IFlagRepository> _mockFlagRepo;
        protected readonly Mock<ISessionRepository> _mockSessionRepo;
        protected readonly Mock<IQuestionRepository> _mockQuestionRepo;

        protected TestBase(Mock<IFlagRepository> mockFlagRepo, Mock<ISessionRepository> mockSessionRepo, Mock<IQuestionRepository> mockQuestionRepo)
        {
            _mockFlagRepo = mockFlagRepo;
            _mockSessionRepo = mockSessionRepo;
            _mockQuestionRepo = mockQuestionRepo;
        }
    }
}
