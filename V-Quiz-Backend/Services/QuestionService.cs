using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _repo;
        private readonly SessionService _sessionService;
        public QuestionService(QuestionRepository repo, SessionService sessionService)
        {
            _repo = repo;
            _sessionService = sessionService;
        }
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _repo.GetAllQuestionsAsync();
        }

        public async Task<int> GetQuestionCountAsync()
        {
            return await _repo.GetQuestionCountAsync();
        }

        public async Task<Session> StartQuizAsync(Guid? userId = null)
        {
           var session = await _sessionService.CreateSessionAsync(userId);
            return session;
        }
    }
}
