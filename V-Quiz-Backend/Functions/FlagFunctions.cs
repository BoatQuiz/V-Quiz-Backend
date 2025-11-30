using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Functions;

public class FlagFunctions(FlagService flagService)
{
    private readonly FlagService _flagService = flagService;

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