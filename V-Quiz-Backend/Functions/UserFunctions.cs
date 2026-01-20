using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Functions;

public class UserFunctions
{
    private readonly IUserService _userService;
    private readonly ILogger<UserFunctions> _logger;

    public UserFunctions(ILogger<UserFunctions> logger, IUserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    [Function("GetQuizProfile")]
    public async Task<HttpResponseData> GetQuizProfile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/quiz-profile")]
        HttpRequestData req)
    {
        var userId = GetUserId(req);
        var profile = await _userService.GetQuizProfileAsync(userId);

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(profile);
        return response;
    }

    private Guid GetUserId(HttpRequestData req)
    {
       var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        var userIdString = query["userId"];

        if (string.IsNullOrEmpty(userIdString))
        {
            throw new InvalidOperationException("userId is required in query parameters.");
        }
        return Guid.Parse(userIdString);
    }

    [Function("UserRegister")]
    public async Task<HttpResponseData> UserRegisterasync(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/register")] HttpRequestData req)
    {
        var body = await JsonSerializer.DeserializeAsync<LoginDto>(req.Body);
        if (body == null)
        {
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteStringAsync("Invalid request body.");
            return badRequest;
        }
        var result = await _userService.RegisterUserAsync(body);

        HttpResponseData response;

        if (result.Success)
        {
            response = req.CreateResponse(HttpStatusCode.OK);
        }
        else if (result.Message == "Username already exists")
        {
            response = req.CreateResponse(HttpStatusCode.Conflict);
        }
        else
        {
            response = req.CreateResponse(HttpStatusCode.BadRequest);
        }
        await response.WriteAsJsonAsync(result);
        return response;
    }
}