namespace V_Quiz_Backend.Models
{
    public class CurrentQuestionState
    {
        public string QuestionId { get; set; } = string.Empty;
        public string QuestionText { get; set; }
        public List<string>? Category { get; set; }
        public DateTime AskedAtUtc { get; set; }
        public int? TimeLimitMS { get; set; }

        public List<string> ShuffledOptions { get; set; } = [];

        public int CorrectIndex { get; set; }
        }

}
