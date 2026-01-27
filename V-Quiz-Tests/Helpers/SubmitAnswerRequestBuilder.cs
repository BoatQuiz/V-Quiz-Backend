using V_Quiz_Backend.Models;

namespace V_Quiz_Tests.Helpers
{
    public class SubmitAnswerRequestBuilder
    {
        private Guid _sessionId = Guid.NewGuid();
        private string _questionId = "q1";
        private int _selectedAnswer = 0;

        public SubmitAnswerRequestBuilder WithSessionId(Guid sessionId)
        {
            _sessionId = sessionId;
            return this;
        }

        public SubmitAnswerRequestBuilder WithQuestionId(string questionId)
        {
            _questionId = questionId;
            return this;
        }

        public SubmitAnswerRequestBuilder WithSelectedAnswer(int selectedAnswer)
        {
            _selectedAnswer = selectedAnswer;
            return this;
        }

        public SubmitAnswerRequest Build()
        {
            return new SubmitAnswerRequest
            {
                SessionId = _sessionId,
                QuestionId = _questionId,
                SelectedAnswer = _selectedAnswer
            };
        }

    }
}
