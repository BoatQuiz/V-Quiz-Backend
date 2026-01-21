using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Services
{
    public interface IUserService
    { 
        Task<ServiceResponse<UserId>> RegisterUserAsync(LoginDto dto);
        Task<ServiceResponse<UserId>> LoginUserAsync(LoginDto dto);

        Task<ServiceResponse<QuizProfile>> GetQuizProfileAsync(Guid? userId);
        Task<ServiceResponse<SessionUser>> GetSessionUserAsync(Guid userId);
    }
}
