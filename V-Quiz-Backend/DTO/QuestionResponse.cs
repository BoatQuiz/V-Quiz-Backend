using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.DTO
{
    public class QuestionResponse
    {
        public required SessionDtoResult Session { get; set; }
        public required QuestionResponseDto Question { get; set; }
    }
}
