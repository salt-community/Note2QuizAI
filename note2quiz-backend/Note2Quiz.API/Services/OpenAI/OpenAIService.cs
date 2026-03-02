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

        var model = DeserializeStrict(json);

        Validate(model);

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

    private static QuizGenResponse DeserializeStrict(string json)
    {
        try
        {
            var model = JsonSerializer.Deserialize<QuizGenResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            if (model == null)
                throw new InvalidOperationException("Invalid AI response: null.");

            return model;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid AI response: not valid JSON.", ex);
        }
    }



    private static void Validate(QuizGenResponse model)
    {
        if (model.Questions == null)
            throw new InvalidOperationException("AI response missing questions.");

        if (model.Questions.Count != 5)
            throw new InvalidOperationException("AI must return exactly 5 questions.");

        foreach (var q in model.Questions)
        {
            if (string.IsNullOrWhiteSpace(q.Question))
                throw new InvalidOperationException("Question text is empty.");

            if (q.Options == null || q.Options.Count != 4)
                throw new InvalidOperationException("Each question must have exactly 4 options.");

            for (var i = 0; i < q.Options.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(q.Options[i]))
                    throw new InvalidOperationException("Options cannot be empty.");
            }

            if (q.CorrectOptionIndex < 0 || q.CorrectOptionIndex > 3)
                throw new InvalidOperationException("correctOptionIndex must be 0-3.");

            var distinctCount = q.Options
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            if (distinctCount != 4)
                throw new InvalidOperationException("Options must be distinct.");
        }
    }

    private static string Truncate(string text, int maxChars)
    {
        if (text.Length <= maxChars) return text;

        return text.Substring(0, maxChars);
    }

    private sealed class QuizGenResponse
    {
        public List<QuizGenQuestion> Questions { get; set; } = new();
    }

    private sealed class QuizGenQuestion
    {
        public string Question { get; set; } = "";
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
    }
}