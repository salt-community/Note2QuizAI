namespace Note2Quiz.API.Services.OpenAI;

public static class OpenAIValidator
{
    public static void Validate(QuizGenResponse model)
    {
        if (model?.Questions == null || model.Questions.Count == 0)
            throw new InvalidOperationException("AI response missing questions.");

        foreach (var q in model.Questions.ToList())
        {
            if (string.IsNullOrWhiteSpace(q.Question) || q.Options == null || q.Options.Count < 2) //Accept at least 2 options to save the call
            {
                model.Questions.Remove(q);
                continue;
            }

            // Index validation
            if (q.CorrectOptionIndex < 0 || q.CorrectOptionIndex >= q.Options.Count)
            {
                model.Questions.Remove(q);
                continue;
            }

            var distinctCount = q
                .Options.Select(x => x?.Trim() ?? "")
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            if (distinctCount != q.Options.Count)
            {
                model.Questions.Remove(q);
            }
        }

        if (model.Questions.Count == 0)
            throw new InvalidOperationException("No valid questions remained after validation.");
    }
}
