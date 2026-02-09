using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using V_Quiz_Backend.Context;
using V_Quiz_Backend.DTO;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Models;

namespace V_Quiz_Backend.Functions
{
    public class QuizFunctions(IQuizService quizService)
    {
        [Function("StartQuiz")]
        public async Task<HttpResponseData> StartQuizAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "quiz/start")] HttpRequestData req)
        {
            var userId = UserContext.TryGetUserId(req);

            var session = await quizService.StartQuizAsync(userId);
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

            var responseObj = await quizService.SubmitAnswerAsync(request);
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

            var responseObj = await quizService.GetNextQuestionAsync(request);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(responseObj);
            return response;
        }

        [Function("GetMetaData")]
        public async Task<HttpResponseData> GetMetadataAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "quiz/metadata")] HttpRequestData req)
        {
            var metadataResponse = await quizService.GetQuizMetaDataAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(metadataResponse);
            return response;
        }
    }
}
