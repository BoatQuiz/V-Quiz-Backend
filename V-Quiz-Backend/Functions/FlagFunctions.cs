using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System.Net;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

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
        string rawBody = await new StreamReader(req.Body).ReadToEndAsync();
        var request = JsonConvert.DeserializeObject<FlagRequestDto>(rawBody);
        if (request == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid request payload.");
            return badRequest;
        }
        var responseObj = await _flagService.FlagQuestion(request);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(responseObj);
        return response;
    }

    [Function("GetFlagReason")]
    public async Task <HttpResponseData> GetFlagReasons(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "flag/reasons")] HttpRequestData req)
    {
        var reasons = Enum.GetValues<FlagReason>()
            .Select(reason => new FlagReasonDto { Key=reason.ToString(), Label = reason.ToString() }).ToList();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(reasons);
        return response;
    }
}