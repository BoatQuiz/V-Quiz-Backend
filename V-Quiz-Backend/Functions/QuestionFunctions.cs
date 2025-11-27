using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Functions
{
    public class QuestionFunctions(QuestionService service)
    {
        private readonly QuestionService _service = service;

        [Function("GetQuestions")]
        public async Task<HttpResponseData> GetQuestionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "questions")] HttpRequestData req)
        {
            var questions = await _service.GetAllQuestionsAsync();
            var json = System.Text.Json.JsonSerializer.Serialize(questions);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);
            return response;
        }

        [Function("GetQuestionCount")]
        public async Task <HttpResponseData> GetQuestionCountAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "questions/count")] HttpRequestData req)
        {
            var count = await _service.GetQuestionCountAsync();
            
            var response = req.CreateResponse(HttpStatusCode.OK);
            
            await response.WriteAsJsonAsync(new { count });
            return response;
        }

        [Function("StartQuiz")]
        public async Task<HttpResponseData> StartQuizAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/start")] HttpRequestData req)
        {
            var session = await _service.StartQuizAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(session);
            return response;
        }

        [Function("SubmitAnswer")]
        public async Task<HttpResponseData> SubmitAnswer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quiz/submitAnswer")] HttpRequestData req)
        {

        }
    }
}
