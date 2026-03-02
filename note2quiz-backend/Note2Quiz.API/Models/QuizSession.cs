namespace Note2Quiz.API.Models;

public class QuizSession
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Question> Questions { get; set; }
}