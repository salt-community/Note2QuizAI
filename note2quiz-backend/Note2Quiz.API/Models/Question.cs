namespace Note2Quiz.API.Models;

public class Question
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Option> Options { get; set; }
    public int QuizSessionId { get; set; }
    public QuizSessions? QuizSession { get; set; }
}
