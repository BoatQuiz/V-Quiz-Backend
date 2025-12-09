using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Services
{
    public class FlagService : IFlagService 
    {
        private readonly IFlagRepository _flagRepo;

        public FlagService(IFlagRepository flagRepo)
        {
            _flagRepo = flagRepo;
        }

        public async Task<ServiceResponse<FlaggedQuestion>> FlagQuestion(FlagRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.QuestionId))
            {
                return ServiceResponse<FlaggedQuestion>.Fail("Invalid flag request data.");
            }

            if (request.Reasons == null || request.Reasons.Count == 0)
            {
                // Denna sätter som default reason "Other" om ingen anledning anges
                request.Reasons = [FlagReason.Other];
            }

            var flagEntry = new FlagEntry
            {
                UserId = request.UserId,
                Reason = request.Reasons,
                Comment = request.Comment,
                FlaggedAt = DateTime.UtcNow
            };

            var flaggedQuestion = await _flagRepo.GetFlaggedQuestionByIdAsync(request.QuestionId);

            // If no existing flag entry, create a new one
            if (flaggedQuestion == null) 
            {
                flaggedQuestion = new FlaggedQuestion
                {
                    QuestionId = request.QuestionId,
                    Flags = new List<FlagEntry> { flagEntry },
                    IsResolved = false,
                };
                await _flagRepo.AddFlaggedQuestionAsync(flaggedQuestion);
                return ServiceResponse<FlaggedQuestion>.Ok(flaggedQuestion, "Question flagged successfully.");
            }

            // If an existing flag entry is found, append the new flag
            flaggedQuestion.Flags.Add(flagEntry);
            await _flagRepo.UpdateFlaggedQuestionAsync(flaggedQuestion);
            return ServiceResponse<FlaggedQuestion>.Ok(flaggedQuestion, "Question flagged successfully.");
        }
    }
}
