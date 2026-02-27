namespace Note2Quiz.API.Interfaces;

public interface IQuizRepository
{
    Task<QuizSessions> GetQuizSessionAsync(int id);
    Task<List<QuizSessions>> GetAllQuizSessionsAsync(string userId);
    Task<QuizSessions> CreateQuizSessionAsync(QuizSessions quizSession);

    Task<Question> GetQuestionAsync(int id);
    Task<List<Question>> GetQuestionsByQuizSessionIdAsync(int quizSessionId);
    Task<Question> AddQuestionAsync(Question question);

    Task<Option> AddOptionAsync(Option option);
    Task<List<Option>> GetOptionsByQuestionIdAsync(int questionId);
}
