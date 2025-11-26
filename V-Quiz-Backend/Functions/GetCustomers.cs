using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Functions
{
    public class GetCustomers(CustomerService service)
    {
        private readonly CustomerService _service = service;

        [Function("GetCustomers")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var customers = await _service.GetAllCustomersAsync();
            var json = System.Text.Json.JsonSerializer.Serialize(customers);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);
            return response;
        }
    }
}
