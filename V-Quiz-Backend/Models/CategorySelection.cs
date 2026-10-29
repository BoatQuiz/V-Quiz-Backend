using MongoDB.Bson.Serialization.Attributes;

namespace V_Quiz_Backend.Models
{
    // Användarens valda träd: kategori -> subkategorier -> topics.
    // Tom lista på en nivå betyder "allt under denna nivå".
    public class CategorySelection
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("subcategories")]
        public List<SubcategorySelection> Subcategories { get; set; } = [];
    }

    public class SubcategorySelection
    {
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("topics")]
        public List<string> Topics { get; set; } = [];
    }
}
