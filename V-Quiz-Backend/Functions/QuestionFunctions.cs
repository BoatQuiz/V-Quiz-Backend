using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Functions
{
    public class QuestionFunctions(QuestionService service)
    {
        private readonly QuestionService _service = service;

        [Function("GetQuestions")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var questions = await _service.GetAllQuestionsAsync();
            var json = System.Text.Json.JsonSerializer.Serialize(questions);
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);
            return response;
        }        
    }
}
