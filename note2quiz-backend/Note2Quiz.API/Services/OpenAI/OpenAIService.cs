using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;
using Note2Quiz.API.Services.OpenAI.Models;

namespace Note2Quiz.API.Services.OpenAI;

public class OpenAIService : IOpenAIService
{
    private readonly IChatClient _chatClient;

    private const int MaxSourceChars = 4000;

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

        var trimmed = TextTruncator.Truncate(sourceText, MaxSourceChars);

        var systemPrompt = "You are a quiz generator. Return ONLY valid JSON. No markdown.";

        var userPrompt = OpenAIPrompts.BuildUserPrompt(trimmed, difficulty);

        var settings = new ChatSettings
        {
            ForceJson = true,
            MaxTokens = 1500
        };

        var json = await _chatClient.GetCompletionAsync(systemPrompt, userPrompt, settings, ct);

        var model = OpenAIResponseParser.DeserializeStrict(json);

        OpenAIValidator.Validate(model);

        return model.Questions.Select(q => new GeneratedQuestion(
            Text: q.Question,
            Options: q.Options,
            CorrectOptionIndex: q.CorrectOptionIndex
        )).ToList();
    }
}