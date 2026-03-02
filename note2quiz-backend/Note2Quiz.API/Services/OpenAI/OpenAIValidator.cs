namespace Note2Quiz.API.Services.OpenAI;

public static class OpenAIValidator
{
    public static void Validate(QuizGenResponse model)
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
}