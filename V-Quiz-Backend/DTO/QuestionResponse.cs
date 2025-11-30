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
        public SubmitSessionId SessionId { get; set; }
        public QuestionResponseDto Question { get; set; }
    }
}
