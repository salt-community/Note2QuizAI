namespace Note2Quiz.API.Models;

public class QuizSession
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Question> Questions { get; set; } = new();
    public List<UserAnswer> UserAnswers { get; set; } = new();
}
