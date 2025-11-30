using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.DTO
{
    public class FlagQuestionRequestDto
    {
        public string QuestionId { get; set; } = null!;
        public string? UserId { get; set; }
        public string? Comment { get; set; }
    }
}
