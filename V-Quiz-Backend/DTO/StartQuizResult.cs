using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.DTO
{
    public class StartQuizResult
    {
        public SessionId Session { get; set; }
        public QuestionOutDto RandomQuestion { get; set; }
    }
}
