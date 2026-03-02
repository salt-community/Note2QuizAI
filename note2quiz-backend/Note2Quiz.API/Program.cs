using Note2Quiz.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

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

app.Run();