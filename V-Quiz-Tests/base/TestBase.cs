using Moq;
using V_Quiz_Backend.Interface;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;

namespace V_Quiz_Tests
{
    public abstract class TestBase
    {
        protected readonly Mock<IFlagRepository> FlagRepoMock;
        protected readonly Mock<ISessionRepository> SessionRepoMock;
        protected readonly Mock<IQuestionRepository> QuestionRepoMock;
        protected readonly Mock<IUserRepository> UserRepoMock;
        protected readonly Mock<IPasswordHasher> PasswordHasherMock;
        protected readonly Mock<IUserService> UserServiceMock;

        protected TestBase()
        {
            FlagRepoMock = new Mock<IFlagRepository>();
            SessionRepoMock = new Mock<ISessionRepository>();
            QuestionRepoMock = new Mock<IQuestionRepository>();
            UserRepoMock = new Mock<IUserRepository>();
            PasswordHasherMock = new Mock<IPasswordHasher>();
            UserServiceMock = new Mock<IUserService>();
        }
    }
}
