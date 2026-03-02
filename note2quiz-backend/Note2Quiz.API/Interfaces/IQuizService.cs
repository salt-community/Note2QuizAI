using Note2Quiz.API.Models;

namespace Note2Quiz.API.Interfaces;

public interface IQuizService
{
    Task<QuizSession> StartNewQuizAsync(string userId);

    Task<List<QuizSession>> GetUserQuizzesAsync(string userId);
}
