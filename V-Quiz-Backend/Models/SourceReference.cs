using MongoDB.Bson.Serialization.Attributes;

namespace V_Quiz_Backend.Models
{
    [BsonIgnoreExtraElements]
    public class SourceReference
    {
        [BsonElement("document")]
        public string? Document { get; set; }

        [BsonElement("revNo")]
        public int? RevNo { get; set; }

        [BsonElement("revDate")]
        public string? RevDate { get; set; }

        [BsonElement("pages")]
        public SourcePages? Pages { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class SourcePages
    {
        [BsonElement("pdf")]
        public int? Pdf { get; set; }

        [BsonElement("chapter")]
        public int? Chapter { get; set; }
    }
}