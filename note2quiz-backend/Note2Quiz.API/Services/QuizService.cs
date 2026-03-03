using Microsoft.EntityFrameworkCore;
using Note2Quiz.API.Data;
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
        var text = await _vision.ExtractTextFromImageAsync(request.ImageStream, ct);
        var aiQuestions = await _openAi.GenerateQuizAsync(text, request.Difficulty, ct);

        var session = new QuizSession
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Questions = new List<Question>()
        };

        foreach (var ai in aiQuestions)
        {
            var question = new Question
            {
                Text = ai.Text.Trim(),
                CreatedAt = DateTime.UtcNow,
                Options = new List<Option>()
            };

            for (var index = 0; index < 4; index++)
            {
                question.Options.Add(new Option
                {
                    Text = ai.Options[index].Trim(),
                    IsCorrect = index == ai.CorrectOptionIndex
                });
            }

            session.Questions.Add(question);
        }

        // var saved = await _repo.CreateQuizSessionAsync(session, ct);
        var saved = session;

        var dto = new QuizResponse(
            QuizSessionId: saved.Id,
            Questions: saved.Questions
                .OrderBy(q => q.Id)
                .Select(q => new QuestionDto(
                    Id: q.Id,
                    Text: q.Text,
                    Options: q.Options
                        .OrderBy(o => o.Id)
                        .Select(o => new OptionDto(o.Id, o.Text))
                        .ToList()
                ))
                .ToList()
        );

        return dto;
    }

}