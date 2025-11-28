using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.Models
{
    public class Session
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.String)]
        public Guid? UserId { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [BsonElement("stoppedAt")]
        [BsonIgnoreIfNull]
        public DateTime? StoppedAt { get; set; } = null;
        
        [BsonElement("usedQuestions")]
        public List<string> UsedQuestions { get; set; } = [];
        
        [BsonElement("numCorrectAnswers")]
        public int NumCorrectAnswers { get; set; } = 0;
        
        [BsonElement("numQuestions")]
        public int NumQuestions { get; set; } = 0;
    }
}
