using System.Text.Json;

namespace Note2Quiz.API.Services.OpenAI;

public static class OpenAIResponseParser
{
    public static QuizGenResponse DeserializeStrict(string json)
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

}