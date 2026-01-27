using V_Quiz_Backend.DTO;

namespace V_Quiz_Tests.Helpers
{
    public class QuestionResponseDtoBuilder
    {
        private string _questionId = "q1";
        private string _questionText = "What is the capital of France?";
        private List<string> _options = new() { "Berlin", "Madrid", "Paris", "Rome" };
        private List<string> _category = new() { "Geography" };
        private int _correctIndex = 0;

        public QuestionResponseDtoBuilder WithId(string id)
        {
            _questionId = id;
            return this;
        }

        public QuestionResponseDtoBuilder WithText(string text)
        {
            _questionText = text;
            return this;
        }

        public QuestionResponseDtoBuilder WithOptions(List<string> options, int correctIndex)
        {
            _options = options;
            _correctIndex = correctIndex;
            return this;
        }

        public QuestionResponseDto Build()
        {
            return new QuestionResponseDto
            {
                QuestionId = _questionId,
                QuestionText = _questionText,
                Options = _options,
                Category = _category,
                CorrectIndex = _correctIndex
            };
        }
    }
}
