using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Functions;

public class FlagFunctions
{
    private readonly IFlagService _flagService;

    public FlagFunctions(IFlagService flagService)
    {
        _flagService = flagService;
    }

    [Function("FlagFunctions")]
    public async Task<HttpResponseData> FlagQuestionAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "flag/question")] HttpRequestData req)
    {
        var request = await req.ReadFromJsonAsync<DTO.FlagQuestionRequestDto>();
        if (request == null)
        {
            var badRequest = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid request payload.");
            return badRequest;
        }
        var responseObj = await _flagService.FlagQuestion(request);
        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(responseObj);
        return response;
    }
}