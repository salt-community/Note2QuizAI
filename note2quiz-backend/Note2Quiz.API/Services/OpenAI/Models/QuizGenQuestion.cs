namespace Note2Quiz.API.Services.OpenAI.Models;

public class QuizGenQuestion
{
    public string Question { get; set; } = "";
    public List<string> Options { get; set; } = new();
    public int CorrectOptionIndex { get; set; }
}