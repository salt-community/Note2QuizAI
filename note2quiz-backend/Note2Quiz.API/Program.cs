using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.EntityFrameworkCore;
using Note2Quiz.API.Data;
using Note2Quiz.API.Interfaces;
using Note2Quiz.API.Services;
using Note2Quiz.API.Services.OpenAI;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton(sp =>
{
    var endPoint = builder.Configuration["AzureVision:Endpoint"]
        ?? throw new InvalidOperationException("Vision Endpoint missing");

    var key = builder.Configuration["AzureVision:Key"]
        ?? throw new InvalidOperationException("Vision Key missing");

    return new ImageAnalysisClient(new Uri(endPoint), new AzureKeyCredential(key));
});


builder.Services.AddSingleton<IChatClient>(sp =>
{
    var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]
        ?? throw new InvalidOperationException("AzureOpenAI:Endpoint is missing in configuration.");

    var apiKey = builder.Configuration["AzureOpenAI:ApiKey"]
        ?? throw new InvalidOperationException("AzureOpenAI:ApiKey is missing in configuration.");

    var deployment = builder.Configuration["AzureOpenAI:DeploymentName"]
        ?? throw new InvalidOperationException("AzureOpenAI:DeploymentName is missing in configuration.");

    return new AzureChatClient(endpoint, apiKey, deployment);
});


builder.Services.AddDbContext<Note2QuizDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IVisionService, VisionService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IQuizService, QuizService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<Note2QuizDbContext>();
        await SeedData.InitializeAsync(db);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseHttpsRedirection();

app.Run();
