using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.Models
{
    public class FlagEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Guid? UserId { get; set; }
        public List<FlagReason> Reason { get; set; } = [];
        public string? Comment { get; set; }
        public DateTime FlaggedAt { get; set; } = DateTime.UtcNow;

    }
}
