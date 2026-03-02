using Note2Quiz.API.Services.OpenAI.Models;

namespace Note2Quiz.API.Services.OpenAI;

public class QuizGenResponse
{
    public List<QuizGenQuestion> Questions { get; set; } = new();
}