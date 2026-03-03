using Note2Quiz.API.Models;

namespace Note2Quiz.API.Interfaces;

public interface IQuizRepository
{
    Task<QuizSession> GetQuizSessionAsync(int id);
    Task<List<QuizSession>> GetAllQuizSessionsAsync(string userId);
    Task<QuizSession> CreateQuizSessionAsync(QuizSession session);

    Task<Question> GetQuestionAsync(int id);
    Task<List<Question>> GetQuestionsByQuizSessionIdAsync(int quizSessionId);
    Task<Question> AddQuestionAsync(Question question);

    Task<Option> AddOptionAsync(Option option);
    Task<List<Option>> GetOptionsByQuestionIdAsync(int questionId);
}
