using Microsoft.EntityFrameworkCore;
using Note2Quiz.API.Data;
using Note2Quiz.API.Interfaces;
using Note2Quiz.API.Models;

namespace Note2Quiz.API.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly Note2QuizDbContext _db;

    public QuizRepository(Note2QuizDbContext db)
    {
        _db = db;
    }

    public async Task<QuizSession> CreateQuizSessionAsync(QuizSession session, CancellationToken ct)
    {
        await _db.AddAsync(session, ct);
        await _db.SaveChangesAsync(ct);

        return session;
    }

    public async Task<List<QuizSession>> GetQuizSessionsByUserIdAsync(
        string userId,
        CancellationToken ct
    )
    {
        return await _db
            .QuizSessions.Where(q => q.UserId == userId)
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .Include(q => q.UserAnswers)
                .ThenInclude(ua => ua.Option)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<QuizSession?> GetQuizSessionForSubmitAsync(int quizSessionId, CancellationToken ct)
    {
        return await _db.QuizSessions
            .AsNoTracking()
            .Include(s => s.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(s => s.Id == quizSessionId, ct);
    }

    public async Task SaveUserAnswerAsync(List<UserAnswer> userAnswers, CancellationToken ct)
    {
        _db.Set<UserAnswer>().AddRange(userAnswers);
        await _db.SaveChangesAsync(ct);
    }
}
