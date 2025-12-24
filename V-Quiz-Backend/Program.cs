using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using V_Quiz_Backend.Interface;
using V_Quiz_Backend.Interface.Repos;
using V_Quiz_Backend.Interface.Services;
using V_Quiz_Backend.Repository;
using V_Quiz_Backend.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Register MongoDbService
builder.Services.AddSingleton<MongoDbService>();

// Register Repository
builder.Services.AddSingleton<IQuestionRepository, QuestionRepository>();
builder.Services.AddSingleton<ISessionRepository, SessionRepository>();
builder.Services.AddSingleton<IFlagRepository, FlagRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();


// Register Service
builder.Services.AddSingleton<IQuestionService, QuestionService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IFlagService, FlagService>();
builder.Services.AddSingleton<IQuizService, QuizService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
