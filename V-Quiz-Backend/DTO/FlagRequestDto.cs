using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.DTO
{
    public class FlagRequestDto
    {
        public string QuestionId { get; set; } = null!;
        public List<FlagReason> Reasons { get; set; } = new ();
        public string? UserId { get; set; }
        public string? Comment { get; set; }
    }
}
