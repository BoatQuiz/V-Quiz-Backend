using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Functions
{
    public class QuizFunctions
    {
        private readonly IQuizService _quizService;

        public QuizFunctions(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [Function("StartQuiz")]
        public async Task<HttpResponseData> StartQuizAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/start")] HttpRequestData req)
        {
            var session = await _quizService.StartQuizAsync();
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

            var responseObj = await _quizService.SubmitAnswerAsync(request);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(responseObj);
            return response;
        }

        [Function("GetNextQuestion")]
        public async Task<HttpResponseData> GetNextQuestionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quiz/getNextQuestion")] HttpRequestData req)
        {
            var request = await req.ReadFromJsonAsync<SubmitSessionId>();
            if (request == null)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Invalid request payload");
                return badRequest;
            }

            var responseObj = await _quizService.GetNextQuestionAsync(request);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(responseObj);
            return response;
        }
    }
}
