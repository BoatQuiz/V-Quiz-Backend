using MongoDB.Bson.Serialization.Attributes;

namespace V_Quiz_Backend.Models
{
    public class SessionUser
    {
        [BsonElement("userId")]
        public Guid? UserId { get; set; }

        [BsonElement("audience")]
        public string Audience { get; set; } = "General";

        [BsonElement("categories")]
        public List<string>? Categories { get; set; } = [];
    }
}
