using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;

namespace Note2Quiz.API.Services;

public interface IOpenAIService
{
    Task<List<GeneratedQuestion>> GenerateQuizAsync(
        string sourceText,
        Difficulty difficulty,
        CancellationToken ct
    );
}
