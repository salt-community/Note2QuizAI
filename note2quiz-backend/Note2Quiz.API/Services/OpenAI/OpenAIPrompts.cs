using Note2Quiz.API.DTOs;

namespace Note2Quiz.API.Services.OpenAI;

public static class OpenAIPrompts
{
    public static string BuildUserPrompt(string text, Difficulty difficulty)
    {
        return
    $$"""
Create exactly 5 multiple choice questions from the text below.

Rules:
- Use ONLY the text as source.
- 4 options per question.
- Exactly one correct option.
- Provide correctOptionIndex as 0-3.
- Keep questions short and unambiguous.
- Options must be distinct.

Return JSON in this schema:
{
  "questions": [
    {
      "question": "string",
      "options": ["string","string","string","string"],
      "correctOptionIndex": 0
    }
  ]
}

Difficulty: {{difficulty}}

Text:
{{text}}
""";
    }

}