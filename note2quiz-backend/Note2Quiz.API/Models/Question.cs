namespace Note2Quiz.API.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Option> Options { get; set; } = new();
    public int QuizSessionId { get; set; }
    public QuizSession? QuizSession { get; set; }
}
