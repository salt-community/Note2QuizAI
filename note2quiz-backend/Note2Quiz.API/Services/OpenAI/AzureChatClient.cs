using Azure;
using Azure.AI.OpenAI;
using Note2Quiz.API.Services.OpenAI.Models;
using OpenAI.Chat;
using System.ClientModel;

namespace Note2Quiz.API.Services.OpenAI;

public class AzureChatClient : IChatClient
{
    private readonly ChatClient _chatClient;

    public AzureChatClient(string endpoint, string apiKey, string deploymentName)
    {
        AzureOpenAIClient azureClient = new(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey));

        _chatClient = azureClient.GetChatClient(deploymentName);
    }

    public async Task<string> GetCompletionAsync(
        string systemPrompt,
        string userPrompt,
        ChatSettings settings,
        CancellationToken ct)
    {
        List<ChatMessage> messages = [
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        ];

        //JSON-mode + Token-max
        ChatCompletionOptions options = new()
        {
            ResponseFormat = settings.ForceJson
                ? ChatResponseFormat.CreateJsonObjectFormat()
                : ChatResponseFormat.CreateTextFormat(),

            MaxOutputTokenCount = settings.MaxTokens ?? 1000,
            Temperature = 0.7f
        };

        try
        {
            ClientResult<ChatCompletion> completion = await _chatClient.CompleteChatAsync(messages, options, ct);
            return completion.Value.Content[0].Text;
        }
        catch (RequestFailedException ex) when (ex.Status == 429)
        {
            //Retry-logic
            await Task.Delay(2000, ct);

            ClientResult<ChatCompletion> retryCompletion = await _chatClient.CompleteChatAsync(messages, options, ct);
            return retryCompletion.Value.Content[0].Text;
        }
    }
}
