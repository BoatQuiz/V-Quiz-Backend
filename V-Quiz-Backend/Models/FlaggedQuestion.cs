

namespace V_Quiz_Backend.Models
{
    public class FlaggedQuestion
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string QuestionId { get; set; } = null!;
        public List<FlagEntry> Flags { get; set; } = [];
        public bool IsResolved { get; set; } = false;
        public DateTime? ResolvedAt { get; set; }
        public string? ResolvedBy { get; set; }
    }
}
