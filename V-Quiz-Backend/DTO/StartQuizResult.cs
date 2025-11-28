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
        public Session Session { get; set; }
        public Question RandomQuestion { get; set; }
    }
}
