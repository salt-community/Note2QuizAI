using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Note2Quiz.API.Data;
using Note2Quiz.API.Interfaces;
using Note2Quiz.API.Repositories;
using Note2Quiz.API.Services;
using Note2Quiz.API.Services.OpenAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ImageAnalysisClient>(sp =>
{
    var endPoint = builder.Configuration["AzureVision:Endpoint"];
    var key = builder.Configuration["AzureVision:Key"];
    return new ImageAnalysisClient(new Uri(endPoint), new AzureKeyCredential(key));
});
builder.Services.AddScoped<IVisionService, VisionService>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<IQuizService, QuizService>();
// builder.Services.AddScoped<IQuizRepository, QuizRepository>();

builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Clerk:Authority"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Note2QuizDbContext>();
    await SeedData.InitializeAsync(db);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
