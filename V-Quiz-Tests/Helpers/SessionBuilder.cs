using V_Quiz_Backend.Models;

namespace V_Quiz_Tests.Helpers
{
    public class SessionBuilder
    {
        private Guid _id = Guid.NewGuid();
        private DateTime? _endedAt = null;
        private int _targetQuestionCount = 5;
        private List<UsedQuestion> _usedQuestions = new();
        private SessionUser _player = new()
        { 
            Audience = "general", 
            Categories = ["science"]
        };
        private CurrentQuestionState? _currentQuestion = null;

        public SessionBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public SessionBuilder Ended()
        {
            _endedAt = DateTime.UtcNow;
            return this;
        }

        public SessionBuilder WithUsedQuestions(int count, bool answeredCorrectly = true)
        {
            _usedQuestions = Enumerable.Range(0, count)
                .Select(i => new UsedQuestion
                {
                    QuestionId = $"q{i}",
                    AnsweredCorrectly = answeredCorrectly,
                    Category = ["science"],
                    TimeMs = 1000
                }).ToList();
            return this;
        }

        public SessionBuilder WithCurrentQuestion(
            string questionId = "q1",
            int correctIndex = 1,
            string category = "science")
        {
            _currentQuestion = new CurrentQuestionState
            {
                QuestionId = questionId,
                CorrectIndex = correctIndex,
                Category = [category],
                AskedAtUtc = DateTime.UtcNow.AddSeconds(-5)
            };
            return this;
        }



        public Session Build()
        {
            return new Session
            {
                Id = _id,
                Player = _player,
                TargetQuestionCount = _targetQuestionCount,
                EndedAtUtc = _endedAt,
                UsedQuestions = _usedQuestions,
                CurrentQuestion = _currentQuestion,
            };
        }
    }
}
