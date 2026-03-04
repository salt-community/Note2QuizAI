// using Note2Quiz.API.Data;
// using Note2Quiz.API.Interfaces;
// using Note2Quiz.API.Models;

// namespace Note2Quiz.API.Repositories;

// public class QuizRepository : IQuizRepository
// {
//     private readonly Note2QuizDbContext _db;

//     public QuizRepository(Note2QuizDbContext db)
//     {
//         _db = db;
//     }

//     public async Task<QuizSession> CreateQuizSessionAsync(QuizSession session, CancellationToken ct)
//     {
//         await _db.AddAsync(session, ct);
//         await _db.SaveChangesAsync(ct);

//         return session;
//     }
// }