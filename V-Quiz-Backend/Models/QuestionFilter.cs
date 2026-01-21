namespace V_Quiz_Backend.Models
{
    public class QuestionFilter
    {
        public IEnumerable<string> ExcludedQuestionIds { get; set; } = [];
        public IEnumerable<string>? AllowedCategories { get; set; }
        public int? Difficulty { get; set; }
        public string Audience { get; set; } = null!;
    }
}
