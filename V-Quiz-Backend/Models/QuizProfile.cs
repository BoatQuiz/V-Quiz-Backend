using MongoDB.Bson.Serialization.Attributes;

namespace V_Quiz_Backend.Models
{
    public class QuizProfile
    {
        [BsonElement("audience")]
        public string Audience { get; set; } = "General";
        [BsonElement("categories")]
        public List<string> Categories { get; set; } = [];
    }
}