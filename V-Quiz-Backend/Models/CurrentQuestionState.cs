namespace V_Quiz_Backend.Models
{
    public class CurrentQuestionState
    {
        public string QuestionId { get; set; } = string.Empty;
        public DateTime AskedAtUtc { get; set; }
        public int? TimeLimitMS { get; set; }
        }
}
