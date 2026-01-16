namespace V_Quiz_Backend.DTO
{
    public class SessionDtoResult
    {
        public Guid SessionId { get; set; }
        public int TargetQuestionCount { get; set; }
        public int QuestionsAnswered { get; set; }

    }
}
