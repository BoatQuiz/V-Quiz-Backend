namespace V_Quiz_Backend.DTO
{
    public class SessionSummaryDto
    {
        public int TotalQuestions { get; set; }
        public int TotalCorrect { get; set; }
        public string Audience { get; set; } = null!;
        public List<CategorySummaryDto> Categories { get; set; } = [];
    }

    public class CategorySummaryDto
    {
        public string Category { get; set; } = null!;
        public int Correct {  get; set; }
        public int Total {  get; set; }
        public int Percent { get; set; }
    }
}
