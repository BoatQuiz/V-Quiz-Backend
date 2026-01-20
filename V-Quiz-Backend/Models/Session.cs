using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace V_Quiz_Backend.Models
{
    public class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonElement("player")]
        public SessionUser Player { get; set; } = null!;

        //Metadata        
        [BsonElement("startedAtUtc")]
        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
        
        [BsonElement("endedAtUtc")]
        [BsonIgnoreIfNull]
        public DateTime? EndedAtUtc { get; set; } 

        [BsonElement("targetQuestionCount")]
        public int TargetQuestionCount { get; set; }

        [BsonElement("currentQuestion")]
        public CurrentQuestionState? CurrentQuestion { get; set; } = null;

        [BsonElement("usedQuestions")]
        public List<UsedQuestion> UsedQuestions { get; set; } = [];       
    }
}
