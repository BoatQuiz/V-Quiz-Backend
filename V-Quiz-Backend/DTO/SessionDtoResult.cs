using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.DTO
{
    public class SessionDtoResult
    {
        public Guid SessionId { get; set; }
        public int Score { get; set; }
        public int NumUsedQuestions { get; set; }

    }
}
