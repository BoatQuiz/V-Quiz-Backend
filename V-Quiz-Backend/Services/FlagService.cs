using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Repository;

namespace V_Quiz_Backend.Services
{
    public class FlagService(FlagRepository flagRepo)
    {
        private readonly FlagRepository _flagRepo = flagRepo;

        public async Task<ServiceResponse<FlaggedQuestion>> FlagQuestion(FlagQuestionRequestDto flaggedQuestion)
        {
            if (flaggedQuestion == null || string.IsNullOrEmpty(flaggedQuestion.QuestionId))
            {
                return new ServiceResponse<FlaggedQuestion>
                {
                    Success = false,
                    Message = "Invalid flagged question data."
                };
            }

            var flagEntry = new FlagEntry
            {
                UserId = flaggedQuestion.UserId,
                Comment = flaggedQuestion.Comment
            };

            var existingFlag = await _flagRepo.GetFlaggedQuestionByIdAsync(flaggedQuestion.QuestionId);

            if (existingFlag == null) 
            {
                var flaggedQuestionModel = new FlaggedQuestion
                {
                    QuestionId = flaggedQuestion.QuestionId,
                    Flags = new List<FlagEntry> { flagEntry },
                    IsResolved = false,
                    ResolvedAt = null,
                    ResolvedBy = null
                };
                await _flagRepo.AddFlaggedQuestionAsync(flaggedQuestionModel);
                return new ServiceResponse<FlaggedQuestion>
                {
                    Data = flaggedQuestionModel,
                    Success = true,
                    Message = "Question flagged successfully."
                };
            }

            existingFlag.Flags.Add(flagEntry);
            await _flagRepo.UpdateFlaggedQuestionAsync(existingFlag);
            return new ServiceResponse<FlaggedQuestion>
            {
                Data = existingFlag,
                Success = true,
                Message = "Question flagged successfully."
            };
        }
    }
}
