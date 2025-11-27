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
        public QuestionService(QuestionRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _repo.GetAllQuestionsAsync();
        }

        public async Task<int> GetQuestionCountAsync()
        {
            return await _repo.GetQuestionCountAsync();
        }
    }
}
