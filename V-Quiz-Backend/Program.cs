using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Repository;
using V_Quiz_Backend.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Register MongoDbService
builder.Services.AddSingleton<MongoDbService>();

// Register Repository
builder.Services.AddSingleton<IQuestionRepository, QuestionRepository>();
builder.Services.AddSingleton<ISessionRepository, SessionRepository>();
builder.Services.AddSingleton<IFlagRepository, FlagRepository>();

// Register Service
builder.Services.AddSingleton<IQuestionService, QuestionService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IFlagService, FlagService>();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
