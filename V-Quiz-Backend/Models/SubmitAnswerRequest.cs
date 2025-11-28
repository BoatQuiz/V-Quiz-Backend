using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.Models
{
    public class SubmitAnswerRequest
    {
        public Guid SessionId { get; set; }
        public string QuestionId { get; set; } = string.Empty;
        public int SelectedAnswer { get; set; }
    }
}
