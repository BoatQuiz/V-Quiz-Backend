namespace V_Quiz_Backend.DTO
{
    public class QuizMetaDataDto
    {
        public List<AudienceMetaDto> Audiences { get; set; } = [];
    }

    public class AudienceMetaDto
    {
        public string Name { get; set; } = null!;
        public List<string> Categories { get; set; } = []; 
    }

    public class QuizMetadataProjection
    {
        public List<string> Audience { get; set; } = null!;
        public List<string> Category { get; set; } = null!;
    }
}
