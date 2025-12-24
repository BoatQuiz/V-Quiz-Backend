using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace V_Quiz_Backend.Models
{
    public class UserEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MongoId { get; set; } = null!;
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; } 
        public string PasswordHash { get; set; } = null!;
        public string? Rank { get; set; }
        public string? ShippingCompany { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
