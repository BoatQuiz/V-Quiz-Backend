namespace V_Quiz_Backend.Models
{
    public class SubmitAnswerResponse
    {
        public bool IsCorrect { get; set; }
        public int CorrectIndex { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsLastQuestion { get; set; }
        public int Score { get; set; }
        public int QuestionsAnswered { get; set; }
    }
}
