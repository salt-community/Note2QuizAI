using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;

namespace Note2Quiz.API.Interfaces;

public interface IQuizService
{
    Task<QuizResponse> CreateQuizAsync(
        string userId,
        CreateQuizRequest request,
        CancellationToken ct
    );

    Task<List<QuizHistoryItemDto>> GetQuizzesAsync(string userId, CancellationToken ct);

    Task<SubmitQuizResponse> SubmitQuizAsync(
        string userId,
        SubmitQuizRequest request,
        CancellationToken ct
    );
    Task<QuizResponse> GetQuizzAsync(string userId, int quizSessionId, CancellationToken ct);
}
