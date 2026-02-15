using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using V_Quiz_Backend.Interface.Services;

namespace V_Quiz_Backend.Functions;

public class SessionFunctions
{
    private readonly ILogger<SessionFunctions> _logger;
    private readonly ISessionService _sessionService;

    public SessionFunctions(ILogger<SessionFunctions> logger, ISessionService sessionService)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    [Function("SessionSummarize")]
    public async Task<HttpResponseData> SessionSummarize(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "session/{sessionId}/summary")] HttpRequestData req, Guid sessionId)
    {
        var summary = await _sessionService.GetSessionSummaryAsync(sessionId);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(summary);
        return response;
    }
}