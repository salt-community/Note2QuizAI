using Note2Quiz.API.Models;

namespace Note2Quiz.API.Interfaces;

public interface IQuizRepository
{
    Task<QuizSession> CreateQuizSessionAsync(QuizSession session, CancellationToken ct);
}
