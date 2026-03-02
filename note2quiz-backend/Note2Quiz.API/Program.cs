using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Note2Quiz.API.Interfaces;
using Note2Quiz.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ImageAnalysisClient>(sp =>
{
    var endPoint = builder.Configuration["AzureVision:Endpoint"];
    var key = builder.Configuration["AzureVision:Key"];
    return new ImageAnalysisClient(new Uri(endPoint), new AzureKeyCredential(key));
});
builder.Services.AddScoped<IVisionService, VisionService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
