namespace V_Quiz_Backend.DTO
{
    public class QuizMetaDataDto
    {
        public List<AudienceMetaDto> Audiences { get; set; } = [];
    }

    public class AudienceMetaDto
    {
        public string Name { get; set; } = string.Empty;
        public List<CategoryMetaDto> Categories { get; set; } = [];
    }

    public class CategoryMetaDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public List<SubcategoryMetaDto> Subcategories { get; set; } = [];
    }

    public class SubcategoryMetaDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public List<TopicMetaDto> Topics { get; set; } = [];
    }

    public class TopicMetaDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class QuizMetadataProjection
    {
        public string Audience { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string>? Subcategory { get; set; }
        public string? Topic { get; set; }
    }
}
