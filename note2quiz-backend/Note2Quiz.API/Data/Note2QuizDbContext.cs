using Microsoft.EntityFrameworkCore;
using Note2Quiz.API.Models;

namespace Note2Quiz.API.Data;

public class Note2QuizDbContext : DbContext
{
    public Note2QuizDbContext(DbContextOptions<Note2QuizDbContext> options)
        : base(options)
    {
    }

    public DbSet<QuizSession> QuizSessions { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // QuizSession → Questions
        modelBuilder.Entity<QuizSession>()
            .HasMany(qs => qs.Questions)
            .WithOne(q => q.QuizSession)
            .HasForeignKey(q => q.QuizSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Question → Options
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserAnswer → Question
        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.Question)
            .WithMany(q => q.UserAnswers)
            .HasForeignKey(ua => ua.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserAnswer → Option
        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.Option)
            .WithMany()
            .HasForeignKey(ua => ua.OptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserAnswer → QuizSession
        modelBuilder.Entity<UserAnswer>()
            .HasOne(ua => ua.QuizSession)
            .WithMany(qs => qs.UserAnswers)
            .HasForeignKey(ua => ua.QuizSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Required fields
        modelBuilder.Entity<QuizSession>()
            .Property(x => x.UserId)
            .IsRequired();

        modelBuilder.Entity<Question>()
            .Property(x => x.Text)
            .IsRequired();

        modelBuilder.Entity<Option>()
            .Property(x => x.Text)
            .IsRequired();
            
    }
}