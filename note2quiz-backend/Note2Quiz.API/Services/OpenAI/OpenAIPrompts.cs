using Note2Quiz.API.DTOs;

namespace Note2Quiz.API.Services.OpenAI;

public static class OpenAIPrompts
{
  public static string BuildUserPrompt(string text, Difficulty difficulty)
  {
    return $"Gen 5 MCQs, 4 opt each. JSON: {{questions:[{{question:'',options:['','','',''],correctOptionIndex:0}}]}}. Diff: {difficulty}. Text: {text}";
  }
}