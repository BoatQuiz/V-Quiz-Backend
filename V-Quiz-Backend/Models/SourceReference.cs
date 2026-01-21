using MongoDB.Bson.Serialization.Attributes;

namespace V_Quiz_Backend.Models
{
    public class SourceReference
    {
        [BsonElement("source")]
        public string Source { get; set; } = string.Empty;

        [BsonElement("chapter")]
        public string? Chapter { get; set; }

        [BsonElement("section")]
        public string? Section { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("page")]
        public int? Page { get; set; }
    }
}
