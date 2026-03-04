using Note2Quiz.API.Models;

public class UserAnswer
{
    public int Id { get; set; }

    public int QuizSessionId { get; set; }
    public QuizSession QuizSession { get; set; } = null!;

    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int OptionId { get; set; }
    public Option Option { get; set; } = null!;
}
