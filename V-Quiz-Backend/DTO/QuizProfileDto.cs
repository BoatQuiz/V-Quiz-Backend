using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.DTO
{
    public class QuizProfileDto
    {
        public string Audience { get; set; } = "General";
        public List<string> Categories { get; set; } = [];
        public List<CategorySelection> Selection { get; set; } = [];
    }
}
