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
        public int QuestionId { get; set; }
        public int SelectedAnswer { get; set; }
    }
}
