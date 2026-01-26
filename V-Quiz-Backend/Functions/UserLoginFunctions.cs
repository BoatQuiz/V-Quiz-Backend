using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Services;

namespace V_Quiz_Backend.Functions
{
    public class UserLoginFunctions(IUserService userService)
    {

        [Function("UserLogin")]
        public async Task<HttpResponseData> UserLoginAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/login")] HttpRequestData req)
        {
            var body = await JsonSerializer.DeserializeAsync<LoginDto>(req.Body);
            if (body == null)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Invalid request body.");
                return badRequest;
            }
            var result = await userService.LoginUserAsync(body);
            var response = req.CreateResponse(
                result.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized
                );
            await response.WriteAsJsonAsync(result);
            return response;
        }
    }
}
