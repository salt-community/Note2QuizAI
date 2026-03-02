using System.Text.Json;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;


namespace Note2Quiz.API.Services.OpenAI;

public class OpenAIService : IOpenAIService
{
    private readonly IChatClient _chatClient;

    public OpenAIService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<List<GeneratedQuestion>> GenerateQuizAsync(
        string sourceText,
        Difficulty difficulty,
        CancellationToken ct
    )
    {
        if (string.IsNullOrWhiteSpace(sourceText))
            throw new ArgumentException("sourceText is required.");

        var trimmed = Truncate(sourceText, 8000);

        var systemPrompt =
            "You generate quizzes. Return ONLY valid JSON. No markdown. No extra text.";

        var userPrompt = OpenAIPrompts.BuildUserPrompt(trimmed, difficulty);

        var json = await _chatClient.GetCompletionAsync(systemPrompt, userPrompt, ct);

        var model = OpenAIResponseParser.DeserializeStrict(json);

        OpenAIValidator.Validate(model);

        var result = new List<GeneratedQuestion>();

        for (var i = 0; i < model.Questions.Count; i++)
        {
            var q = model.Questions[i];

            result.Add(new GeneratedQuestion(
                Text: q.Question,
                Options: q.Options,
                CorrectOptionIndex: q.CorrectOptionIndex
            ));
        }

        return result;
    }



    private static string Truncate(string text, int maxChars)
    {
        if (text.Length <= maxChars) return text;

        return text.Substring(0, maxChars);
    }

}