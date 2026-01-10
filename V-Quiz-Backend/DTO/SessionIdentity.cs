using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.DTO
{
    public class SessionIdentity
    {
        public Guid SessionId { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
