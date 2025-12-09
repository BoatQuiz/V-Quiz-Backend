using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Services
{
    public interface IQuizService
    {
        Task<ServiceResponse<QuestionResponse>> StartQuizAsync(Guid? userId = null);
        Task<ServiceResponse<QuestionResponse>> GetNextQuestionAsync(SubmitSessionId sessionReq);
        Task<ServiceResponse<SubmitAnswerResponse>> SubmitAnswerAsync(SubmitAnswerRequest request);

    }
}
