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
        public ObjectId Id { get; set; }

        [BsonElement("id")]
        public int QuestionId { get; set; }

        [BsonElement("category")]
        public string? Category { get; set; }

        [BsonElement("difficulty")]
        public string? Difficulty { get; set; }

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("options")]
        public List<string> Options { get; set; }

        [BsonElement("correctIndex")]
        public int CorrectIndex { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("version")]
        public int Version { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

    }
}
