using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;
using Note2Quiz.API.Interfaces;

namespace Note2Quiz.API.Services;

public class QuizServiceTests
{
    [Fact]
    public async Task CreateQuizAsync_MapsAiQuestions_ToSessionAndDto()
    {
        // arrange
        var vision = new Mock<IVisionService>();
        var openAi = new Mock<IOpenAIService>();
        var repo = new Mock<IQuizRepository>();

        var file = CreateTestFormFile();

        vision
            .Setup(v => v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("source text");

        var ai = new List<GeneratedQuestion>
        {
            new(" Q1 ", new() { " A ", "B", "C", "D" }, 0),
            new("Q2", new() { "A", " B ", "C", "D" }, 1),
            new("Q3", new() { "A", "B", " C ", "D" }, 2),
            new("Q4", new() { "A", "B", "C", " D " }, 3),
            new("Q5", new() { "A", "B", "C", "D" }, 0),
        };

        openAi
            .Setup(o => o.GenerateQuizAsync("source text", Difficulty.Easy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ai);

        QuizSession? captured = null;

        repo.Setup(r => r.CreateQuizSessionAsync(It.IsAny<QuizSession>()))
            .Callback<QuizSession>(s => captured = s)
            .ReturnsAsync(() =>
            {
                // simulate DB assigning IDs
                var saved = captured!;
                saved.Id = 123;

                var qId = 10;
                var oId = 100;

                foreach (var q in saved.Questions)
                {
                    q.Id = qId++;
                    foreach (var o in q.Options)
                        o.Id = oId++;
                }

                return saved;
            });

        var sut = new QuizService(repo.Object, openAi.Object, vision.Object);

        // act
        var dto = await sut.CreateQuizAsync("user-1", file, Difficulty.Easy, CancellationToken.None);

        // assert - calls
        vision.Verify(v => v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
        openAi.Verify(o => o.GenerateQuizAsync("source text", Difficulty.Easy, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.CreateQuizSessionAsync(It.IsAny<QuizSession>()), Times.Once);

        // assert - session mapping
        Assert.NotNull(captured);
        Assert.Equal("user-1", captured!.UserId);
        Assert.Equal(5, captured.Questions.Count);

        foreach (var q in captured.Questions)
        {
            Assert.False(string.IsNullOrWhiteSpace(q.Text));
            Assert.Equal(4, q.Options.Count);
            Assert.Equal(1, q.Options.Count(o => o.IsCorrect));
            Assert.All(q.Options, o => Assert.False(string.IsNullOrWhiteSpace(o.Text)));
        }

        // assert - dto shape
        Assert.Equal(123, dto.QuizSessionId);
        Assert.Equal(5, dto.Questions.Count);
        Assert.All(dto.Questions, q =>
        {
            Assert.Equal(4, q.Options.Count);
            Assert.All(q.Options, o => Assert.True(o.OptionId > 0));
        });
    }

    private static IFormFile CreateTestFormFile(string content = "fake", string fileName = "test.png")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };
    }
}