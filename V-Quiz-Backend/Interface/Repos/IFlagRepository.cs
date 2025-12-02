using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Interface.Repos
{
    public interface IFlagRepository
    {
        Task<FlaggedQuestion?> GetFlaggedQuestionByIdAsync(string questionId);
        Task AddFlaggedQuestionAsync(FlaggedQuestion flaggedQuestion);
        Task UpdateFlaggedQuestionAsync(FlaggedQuestion flaggedQuestion);
    }
}
