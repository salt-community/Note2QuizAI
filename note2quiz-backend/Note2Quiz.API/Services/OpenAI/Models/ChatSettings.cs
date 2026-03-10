namespace Note2Quiz.API.Services.OpenAI.Models;

public class ChatSettings
{
    public bool ForceJson { get; set; }
    public int? MaxTokens { get; set; }
    public float Temperature { get; set; } = 0.7f;
}
