namespace Note2Quiz.API.Interfaces;

public interface IOpenAIService
{
    Task<List<string>> GenerateQuestionsFromTextAsync(string text, int numberOfQuestions = 5);
}
