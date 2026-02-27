namespace Note2Quiz.API.Models;

public class Option
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; }
    public QuizSession? QuizSession { get; set; }
}
