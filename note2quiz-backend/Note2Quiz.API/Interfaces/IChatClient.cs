using Note2Quiz.API.Services.OpenAI.Models;

public interface IChatClient
{
    Task<string> GetCompletionAsync(
        string systemPrompt,
        string userPrompt,
        ChatSettings settings,
        CancellationToken ct
    );
}