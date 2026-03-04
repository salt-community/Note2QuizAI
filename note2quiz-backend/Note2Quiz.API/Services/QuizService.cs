using Note2Quiz.API.DTOs;
using Note2Quiz.API.Interfaces;
using Note2Quiz.API.Models;

namespace Note2Quiz.API.Services;

public class QuizService : IQuizService
{

    private readonly IQuizRepository _repo;
    private readonly IOpenAIService _openAi;
    private readonly IVisionService _vision;

    public QuizService(IQuizRepository repo, IOpenAIService openAi, IVisionService vision)
    {
        _repo = repo;
        _openAi = openAi;
        _vision = vision;
    }

    public async Task<QuizResponse> CreateQuizAsync(string userId, CreateQuizRequest request, CancellationToken ct)
    {
        await using var stream = request.Image.OpenReadStream();
        string text;

        using (stream)
        {
            text = await _vision.ExtractTextFromImageAsync(stream, ct);
        }

        if (string.IsNullOrWhiteSpace(text) || text.Length < 50)
        {
            throw new InvalidOperationException("The image does not contain enough readable text to generate a quiz.");
        }

        var aiQuestions = await _openAi.GenerateQuizAsync(text, request.Difficulty, ct);

        var session = new QuizSession
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Questions = aiQuestions.Select(ai => new Question
            {
                Text = ai.Text.Trim(),
                CreatedAt = DateTime.UtcNow,
                Options = ai.Options.Select((optText, index) => new Option
                {
                    Text = optText.Trim(),
                    IsCorrect = index == ai.CorrectOptionIndex
                }).ToList()
            }).ToList()
        };

        var saved = await _repo.CreateQuizSessionAsync(session, ct);

        return new QuizResponse(
            QuizSessionId: saved.Id,
            Questions: saved.Questions
                .Select(q => new QuestionDto(
                    Id: q.Id,
                    Text: q.Text,
                    Options: q.Options
                        .Select(o => new OptionDto(o.Id, o.Text))
                        .ToList()
                ))
                .ToList()
        );
    }

}