public interface IChatClient
{
    Task<string> GetCompletionAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken ct
    );
}