namespace Note2Quiz.API.Services;

using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;

public static class QuizSubmissionValidator
{
    public static void ValidateSubmitRequest(SubmitQuizRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.QuizSessionId <= 0)
            throw new ArgumentException("QuizSessionId is invalid.", nameof(request.QuizSessionId));

        if (request.Answers == null || request.Answers.Count == 0)
            throw new ArgumentException("Answers are required.", nameof(request.Answers));
    }

    public static void ValidateSession(QuizSession? session, string userId)
    {
        if (session == null)
            throw new InvalidOperationException("Quiz session not found.");

        if (session.UserId != userId)
            throw new UnauthorizedAccessException("This quiz session does not belong to the current user.");
    }

    public static void ValidateAnswers(SubmitQuizRequest request, QuizSession session)
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
}