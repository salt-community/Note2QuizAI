using Note2Quiz.API.Models;

namespace Note2Quiz.API.Interfaces;

public interface IQuizRepository
{
    Task<QuizSession> CreateQuizSessionAsync(QuizSession session, CancellationToken ct);
    Task<List<QuizSession>> GetQuizSessionsByUserIdAsync(string userId, CancellationToken ct);
    Task<QuizSession?> GetQuizSessionForSubmitAsync(int quizSessionId, CancellationToken ct);
    Task ReplaceUserAnswersAsync(int quizSessionId, List<UserAnswer> userAnswers, CancellationToken ct);
}
