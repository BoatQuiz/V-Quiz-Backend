using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using V_Quiz_Backend.Models;
using V_Quiz_Backend.Services;

namespace V_Quiz_Backend.Functions
{
    public class QuestionFunctions(QuestionService questionService, SessionService sessionService)
    {
        private readonly QuestionService _questionService = questionService;
        private readonly SessionService _sessionService = sessionService;

        [Function("GetQuestions")]
        public async Task<HttpResponseData> GetQuestionsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "questions")] HttpRequestData req)
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            var json = System.Text.Json.JsonSerializer.Serialize(questions);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");
            await response.WriteStringAsync(json);
            return response;
        }

        [Function("GetQuestionCount")]
        public async Task<HttpResponseData> GetQuestionCountAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "questions/count")] HttpRequestData req)
        {
            var count = await _questionService.GetQuestionCountAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(new { count });
            return response;
        }

        [Function("StartQuiz")]
        public async Task<HttpResponseData> StartQuizAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/start")] HttpRequestData req)
        {
            var session = await _questionService.StartQuizAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(session);
            return response;
        }

        [Function("SubmitAnswer")]
        public async Task<HttpResponseData> SubmitAnswer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quiz/submitAnswer")] HttpRequestData req)
        {
            var request = await req.ReadFromJsonAsync<SubmitAnswerRequest>();
            if (request == null)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Invalid request payload.");
                return badRequest;
            }

            var responseObj = await _questionService.SubmitAnswerAsync(request);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(responseObj);
            return response;
        }

        [Function("GetNextQuestion")]
        public async Task<HttpResponseData> GetNextQuestionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/getNextQuestion")] HttpRequestData req)
        {
            var request = await req.ReadFromJsonAsync<SubmitSessionId>();
            if (request == null)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Invalid request payload");
                return badRequest;
            }

            var responseObj = await _questionService.GetNextQuestionAsync(request);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(responseObj);
            return response;
        }
    }
}
