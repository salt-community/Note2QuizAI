using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;

namespace Note2Quiz.API.Interfaces;

public interface IQuizService
{
    Task<QuizResponse> CreateQuizAsync(string userId, IFormFile file, Difficulty difficulty, CancellationToken ct);

    Task<List<QuizSession>> GetUserQuizzesAsync(string userId);
}
