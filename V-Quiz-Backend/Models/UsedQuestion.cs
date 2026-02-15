namespace V_Quiz_Backend.Models
{
    public class UsedQuestion
    {
        public string QuestionId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool AnsweredCorrectly { get; set; } = false;
        
        // Backend-only messuring
        public double TimeMs { get; set; } = 0;

        // For future use / Analytics
        public string TimeBucket { get; set; } = "";
    }
}
