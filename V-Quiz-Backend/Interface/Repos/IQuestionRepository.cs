using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Repos
{
    public interface IQuestionRepository
    {
        public Task <List<Question>> GetAllQuestionsAsync();
        public Task <int> GetQuestionCountAsync();
        public Task <Question> GetQuestionByIdAsync(string questionId);
        public Task <Question> GetRandomQuestionAsync(
            IEnumerable<string> excludedQuestionIds,
            IEnumerable<string>? allowedCategories);
    }
}
