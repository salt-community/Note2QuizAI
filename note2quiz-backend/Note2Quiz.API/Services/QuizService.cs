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

    public async Task<QuizResponse> CreateQuizAsync(
        string userId,
        CreateQuizRequest request,
        CancellationToken ct
    )
    {
        await using var stream = request.Image.OpenReadStream();
        string text;

        using (stream)
        {
            text = await _vision.ExtractTextFromImageAsync(stream, ct);
        }

        if (string.IsNullOrWhiteSpace(text) || text.Length < 50)
        {
            throw new InvalidOperationException(
                "The image does not contain enough readable text to generate a quiz."
            );
        }

        var aiQuestions = await _openAi.GenerateQuizAsync(text, request.Difficulty, ct);

        var session = new QuizSession
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Difficulty = request.Difficulty,
            Questions = aiQuestions
                .Select(ai => new Question
                {
                    Text = ai.Text.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    Options = ai
                        .Options.Select(
                            (optText, index) =>
                                new Option
                                {
                                    Text = optText.Trim(),
                                    IsCorrect = index == ai.CorrectOptionIndex,
                                }
                        )
                        .ToList(),
                })
                .ToList(),
        };
        var saved = await _repo.CreateQuizSessionAsync(session, ct);

        return new QuizResponse(
            QuizSessionId: saved.Id,
            Questions: saved
                .Questions.Select(q => new QuestionDto(
                    Id: q.Id,
                    Text: q.Text,
                    Options: q.Options.Select(o => new OptionDto(o.Id, o.Text))
                        .ToList()
                ))
                .ToList()
        );
    }

    public async Task<QuizResponse> GetQuizAsync(
        string userId,
        int quizSessionId,
        CancellationToken ct
    )
    {
        var session = await _repo.GetQuizSessionById(userId, quizSessionId, ct);

        if (session == null)
            throw new Exception("Quiz not found");

        var questions = session
            .Questions.Select(q => new QuestionDto(
                q.Id,
                q.Text,
                q.Options.Select(o => new OptionDto(o.Id, o.Text)).ToList()
            ))
            .ToList();

        return new QuizResponse(session.Id, questions);
    }

    public async Task<List<QuizHistoryItemDto>> GetQuizzesAsync(string userId, CancellationToken ct)
    {
        var session = await _repo.GetQuizSessionsByUserIdAsync(userId, ct);
        return session
            .Select(s => new QuizHistoryItemDto(
                QuizSessionId: s.Id,
                CreatedAt: s.CreatedAt,
                QuestionCount: s.Questions.Count,
                Score: s.UserAnswers.Any() ? s.UserAnswers.Count(ua => ua.Option.IsCorrect) : null,
                Difficulty: s.Difficulty
            ))
            .ToList();
    }

    public async Task<SubmitQuizResponse> SubmitQuizAsync(
    string userId,
    SubmitQuizRequest request,
    CancellationToken ct)
    {
        ValidateSubmitRequest(request);

        var session = await _repo.GetQuizSessionForSubmitAsync(request.QuizSessionId, ct);

        ValidateSession(session, userId);
        ValidateAnswers(request, session!);

        var evaluation = EvaluateAnswers(session!, request.Answers);

        await _repo.ReplaceUserAnswersAsync(session!.Id, evaluation.UserAnswers, ct);

        return new SubmitQuizResponse(
            QuizSessionId: session.Id,
            Score: evaluation.Score,
            TotalQuestions: session.Questions.Count,
            Results: evaluation.Results.OrderBy(r => r.QuestionId).ToList()
        );
    }

    private static void ValidateSubmitRequest(SubmitQuizRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.QuizSessionId <= 0)
            throw new ArgumentException("QuizSessionId is invalid.", nameof(request.QuizSessionId));

        if (request.Answers == null || request.Answers.Count == 0)
            throw new ArgumentException("Answers are required.", nameof(request.Answers));
    }

    private static void ValidateSession(QuizSession? session, string userId)
    {
        if (session == null)
            throw new InvalidOperationException("Quiz session not found.");

        if (session.UserId != userId)
            throw new UnauthorizedAccessException("This quiz session does not belong to the current user.");
    }

    private static void ValidateAnswers(SubmitQuizRequest request, QuizSession session)
    {
        if (request.Answers.Count != session.Questions.Count)
            throw new InvalidOperationException("Answers must include exactly one answer per question.");

        var duplicateQuestionIds = request.Answers
            .GroupBy(a => a.QuestionId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateQuestionIds.Count > 0)
            throw new InvalidOperationException("Duplicate answers for the same question are not allowed.");

        var questionIds = session.Questions
            .Select(q => q.Id)
            .ToHashSet();

        var answerQuestionIds = request.Answers
            .Select(a => a.QuestionId)
            .ToHashSet();

        if (!questionIds.SetEquals(answerQuestionIds))
            throw new InvalidOperationException("Answers must include exactly one answer per question.");
    }

    private static SubmitEvaluation EvaluateAnswers(
        QuizSession session,
        List<AnswerDto> answers)
    {
        var userAnswers = new List<UserAnswer>();
        var results = new List<QuestionResultDto>();
        var score = 0;

        foreach (var answer in answers)
        {
            var question = session.Questions.First(q => q.Id == answer.QuestionId);

            var selected = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
            if (selected == null)
                throw new InvalidOperationException("Selected option does not belong to the question.");

            var correct = question.Options.FirstOrDefault(o => o.IsCorrect);
            if (correct == null)
                throw new InvalidOperationException("Question has no correct option configured.");

            var isCorrect = selected.Id == correct.Id;

            if (isCorrect)
            {
                score++;
            }

            userAnswers.Add(new UserAnswer
            {
                QuizSessionId = session.Id,
                QuestionId = question.Id,
                OptionId = selected.Id
            });

            results.Add(new QuestionResultDto(
                QuestionId: question.Id,
                SelectedOptionId: selected.Id,
                CorrectOptionId: correct.Id,
                IsCorrect: isCorrect
            ));
        }

        return new SubmitEvaluation
        {
            Score = score,
            UserAnswers = userAnswers,
            Results = results
        };
    }

    private sealed class SubmitEvaluation
    {
        public int Score { get; set; }
        public List<UserAnswer> UserAnswers { get; set; } = new();
        public List<QuestionResultDto> Results { get; set; } = new();
    }
}
