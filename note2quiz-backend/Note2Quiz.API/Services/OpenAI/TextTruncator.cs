namespace Note2Quiz.API.Services.OpenAI;

public static class TextTruncator
{
    public static string Truncate(string text, int maxChars)
    {
        if (text.Length <= maxChars) return text;

        return text.Substring(0, maxChars);
    }
}