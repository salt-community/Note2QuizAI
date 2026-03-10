using Note2Quiz.API.DTOs;
using Note2Quiz.API.Services.OpenAI;

namespace Note2Quiz.API.Services;

public interface IOpenAIService
{
    Task<QuizGenResponse> GenerateQuizAsync(
        string sourceText,
        Difficulty difficulty,
        CancellationToken ct
    );
}
