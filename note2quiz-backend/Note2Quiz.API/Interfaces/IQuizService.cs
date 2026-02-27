namespace Note2Quiz.API.Interfaces;

public interface IQuizService
{
    Task<QuizSessions> StartNewQuizAsync(string userId);

    Task<List<QuizSessions>> GetUserQuizzesAsync(string userId);
}
