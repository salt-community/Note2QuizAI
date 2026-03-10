using Note2Quiz.API.Models;
using Microsoft.EntityFrameworkCore;
using Note2Quiz.API.DTOs;

namespace Note2Quiz.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(Note2QuizDbContext db)
    {
        if (await db.QuizSessions.AnyAsync())
            return;

        var session = new QuizSession
        {
            UserId = "seed-user",
            CreatedAt = DateTime.UtcNow,
            Difficulty = Difficulty.Easy,
            Title = "General Knowledge Quiz",

            Questions = new List<Question>
            {
                new Question
                {
                    Text = "What is the capital of Sweden?",
                    CreatedAt = DateTime.UtcNow,
                    Options = new List<Option>
                    {
                        new Option { Text = "Stockholm", IsCorrect = true },
                        new Option { Text = "Gothenburg", IsCorrect = false },
                        new Option { Text = "Malmö", IsCorrect = false },
                        new Option { Text = "Västerås", IsCorrect = false }
                    }
                },
                new Question
                {
                    Text = "Which planet is known as the Red Planet?",
                    CreatedAt = DateTime.UtcNow,
                    Options = new List<Option>
                    {
                        new Option { Text = "Mars", IsCorrect = true },
                        new Option { Text = "Venus", IsCorrect = false },
                        new Option { Text = "Jupiter", IsCorrect = false },
                        new Option { Text = "Tellus", IsCorrect = false }
                    }
                }
            }
        };

        db.QuizSessions.Add(session);
        await db.SaveChangesAsync();
    }
}