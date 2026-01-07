using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V_Quiz_Backend.Models
{
    public class UsedQuestion
    {
        public string QuestionId { get; set; } = string.Empty;
        public string Category { get; set; } = "";
        public bool AnsweredCorrectly { get; set; } = false;
        
        // Backend-only messuring
        public double TimeMs { get; set; } = 0;

        // For future use / Analytics
        public string TimeBucket { get; set; } = "";
    }
}
