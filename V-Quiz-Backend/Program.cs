using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using V_Quiz_Backend.Repository;
using V_Quiz_Backend.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Register MongoDbService as a singleton
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<QuestionRepository>();
builder.Services.AddSingleton<QuestionService>();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
