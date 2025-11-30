using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.DTO
{
    public class QuestionResponseDto
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
    }
}
