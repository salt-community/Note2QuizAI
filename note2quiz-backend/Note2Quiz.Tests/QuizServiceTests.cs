using Moq;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Models;
using Note2Quiz.API.Interfaces;

namespace Note2Quiz.API.Services;

public class QuizServiceTests
{
    [Fact]
    public async Task CreateQuizAsync_ValidText_MapsEverythingCorrectly()
    {
        // arrange
        var vision = new Mock<IVisionService>();
        var openAi = new Mock<IOpenAIService>();
        var repo = new Mock<IQuizRepository>();

        var validText = "This is a long enough source text that should pass the 50 characters validation gatekeeper.";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("fake image stream"));
        var request = new CreateQuizRequest(stream, Difficulty.Easy);

        vision
            .Setup(v => v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validText);

        var aiQuestions = new List<GeneratedQuestion>
        {
            new(" Q1 ", new() { " A ", "B", "C", "D" }, 0),
            new("Q2", new() { "A", " B ", "C", "D" }, 1),
            new("Q3", new() { "A", "B", " C ", "D" }, 2),
            new("Q4", new() { "A", "B", "C", " D " }, 3),
            new("Q5", new() { "A", "B", "C", "D" }, 0),
        };

        openAi
            .Setup(o => o.GenerateQuizAsync(validText, Difficulty.Easy, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiQuestions);

        QuizSession? captured = null;

        repo.Setup(r => r.CreateQuizSessionAsync(It.IsAny<QuizSession>()))
            .Callback<QuizSession>(s => captured = s)
            .ReturnsAsync((QuizSession s) =>
            {
                s.Id = 123;
                int qId = 10, oId = 100;
                foreach (var q in s.Questions)
                {
                    q.Id = qId++;
                    foreach (var o in q.Options) o.Id = oId++;
                }
                return s;
            });

        var sut = new QuizService(repo.Object, openAi.Object, vision.Object);

        // act
        var dto = await sut.CreateQuizAsync("user-1", request, CancellationToken.None);

        // assert - Verify calls
        vision.Verify(v => v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
        openAi.Verify(o => o.GenerateQuizAsync(validText, Difficulty.Easy, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.CreateQuizSessionAsync(It.IsAny<QuizSession>()), Times.Once);

        // assert - Mapping and Trimming logic
        Assert.NotNull(captured);
        Assert.Equal("user-1", captured!.UserId);
        Assert.Equal(5, captured.Questions.Count);
        Assert.Equal("Q1", captured.Questions.First().Text);
        Assert.Equal("A", captured.Questions.First().Options.First().Text);

        // assert - DTO structure
        Assert.Equal(123, dto.QuizSessionId);
        Assert.Equal(5, dto.Questions.Count);
        Assert.All(dto.Questions, q => Assert.Equal(4, q.Options.Count));
    }

    [Fact]
    public async Task CreateQuizAsync_TooShortText_ThrowsInvalidOperationException()
    {
        // arrange
        var vision = new Mock<IVisionService>();
        var openAi = new Mock<IOpenAIService>();
        var repo = new Mock<IQuizRepository>();

        var shortText = "Too short"; // Under 50 char
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("fake image"));
        var request = new CreateQuizRequest(stream, Difficulty.Easy);

        vision
            .Setup(v => v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(shortText);

        var sut = new QuizService(repo.Object, openAi.Object, vision.Object);

        // act & assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CreateQuizAsync("user-1", request, CancellationToken.None));

        Assert.Contains("enough readable text", ex.Message);

        openAi.Verify(o => o.GenerateQuizAsync(It.IsAny<string>(), It.IsAny<Difficulty>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}