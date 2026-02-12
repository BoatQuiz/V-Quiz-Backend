using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.Models
{
    public class Question
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; } = string.Empty;

        [BsonElement("questionId")]
        public string QuestionId { get; set; } = string.Empty;

        [BsonElement("category")]
        public List<string> Category { get; set; } = [];

        [BsonElement("subcategory")]
        public List<string>? Subcategory { get; set; } = [];

        [BsonElement("difficulty")]
        public int Difficulty { get; set; } = 1;

        [BsonElement("audience")]
        public List<string> Audience { get; set; } = [ "General"];

        [BsonElement("rank")]
        public List<string> Rank { get; set; } = [];

        [BsonElement("text")]
        public string Text { get; set; } = string.Empty;

        [BsonElement("options")]
        public List<string> Options { get; set; } = [];

        [BsonElement("correctIndex")]
        public int CorrectIndex { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("version")]
        public int Version { get; set; }

        [BsonElement("source")]
        [BsonIgnoreIfNull]
        public SourceReference? Source { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

    }
}
