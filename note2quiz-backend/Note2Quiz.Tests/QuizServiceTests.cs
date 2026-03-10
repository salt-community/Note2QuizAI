using Microsoft.AspNetCore.Http;
using Moq;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Interfaces;
using Note2Quiz.API.Models;
using Note2Quiz.API.Services.OpenAI;
using Note2Quiz.API.Services.OpenAI.Models;
using Microsoft.AspNetCore.Http;

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

        var validText =
            "This is a long enough source text that should pass the 50 characters validation gatekeeper.";

        var file = new Mock<IFormFile>();
        file.SetupGet(f => f.Length).Returns(1);
        file.SetupGet(f => f.ContentType).Returns("image/png");
        file.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

        var request = new CreateQuizRequest(file.Object, Difficulty.Easy);

        vision
            .Setup(v =>
                v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(validText);

        var aiResponse = new QuizGenResponse
        {
            Title = "Test Quiz",
            Questions = new List<QuizGenQuestion>
            {
                new QuizGenQuestion
                {
                    Question = " Q1 ",
                    Options = new() { " A ", "B", "C", "D" },
                    CorrectOptionIndex = 0,
                },
                new QuizGenQuestion
                {
                    Question = "Q2",
                    Options = new() { "A", " B ", "C", "D" },
                    CorrectOptionIndex = 1,
                },
                new QuizGenQuestion
                {
                    Question = "Q3",
                    Options = new() { "A", "B", " C ", "D" },
                    CorrectOptionIndex = 2,
                },
                new QuizGenQuestion
                {
                    Question = "Q4",
                    Options = new() { "A", "B", "C", " D " },
                    CorrectOptionIndex = 3,
                },
                new QuizGenQuestion
                {
                    Question = "Q5",
                    Options = new() { "A", "B", "C", "D" },
                    CorrectOptionIndex = 0,
                },
            },
        };

        openAi
            .Setup(o =>
                o.GenerateQuizAsync(validText, Difficulty.Easy, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(aiResponse);

        QuizSession? captured = null;

        repo.Setup(r =>
                r.CreateQuizSessionAsync(It.IsAny<QuizSession>(), It.IsAny<CancellationToken>())
            )
            .Callback<QuizSession, CancellationToken>((s, _) => captured = s)
            .ReturnsAsync(
                (QuizSession s, CancellationToken _) =>
                {
                    s.Id = 123;
                    int qId = 10,
                        oId = 100;
                    foreach (var q in s.Questions)
                    {
                        q.Id = qId++;
                        foreach (var o in q.Options)
                            o.Id = oId++;
                    }
                    return s;
                }
            );

        var sut = new QuizService(repo.Object, openAi.Object, vision.Object);

        // act
        var dto = await sut.CreateQuizAsync("user-1", request, CancellationToken.None);

        // assert - Verify calls
        vision.Verify(
            v => v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        openAi.Verify(
            o => o.GenerateQuizAsync(validText, Difficulty.Easy, It.IsAny<CancellationToken>()),
            Times.Once
        );
        repo.Verify(
            r => r.CreateQuizSessionAsync(It.IsAny<QuizSession>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

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

        var file = new Mock<IFormFile>();
        file.SetupGet(f => f.Length).Returns(1);
        file.SetupGet(f => f.ContentType).Returns("image/png");
        file.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

        var request = new CreateQuizRequest(file.Object, Difficulty.Easy);

        vision
            .Setup(v =>
                v.ExtractTextFromImageAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(shortText);

        var sut = new QuizService(repo.Object, openAi.Object, vision.Object);

        // act & assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CreateQuizAsync("user-1", request, CancellationToken.None)
        );

        Assert.Contains("enough readable text", ex.Message);

        openAi.Verify(
            o =>
                o.GenerateQuizAsync(
                    It.IsAny<string>(),
                    It.IsAny<Difficulty>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never
        );
    }

    [Fact]
    public async Task GetQuizAsync_ReturnsQuiz_WhenSessionExists()
    {
        // arrange
        var repo = new Mock<IQuizRepository>();
        var openAi = new Mock<IOpenAIService>();
        var vision = new Mock<IVisionService>();

        var session = new QuizSession
        {
            Id = 1,
            UserId = "user-1",
            Questions = new List<Question>
            {
                new Question
                {
                    Id = 10,
                    Text = "What is 2+2?",
                    Options = new List<Option>
                    {
                        new Option
                        {
                            Id = 1,
                            Text = "3",
                            IsCorrect = false,
                        },
                        new Option
                        {
                            Id = 2,
                            Text = "4",
                            IsCorrect = true,
                        },
                        new Option
                        {
                            Id = 3,
                            Text = "5",
                            IsCorrect = false,
                        },
                        new Option
                        {
                            Id = 4,
                            Text = "6",
                            IsCorrect = false,
                        },
                    },
                },
            },
        };

        repo.Setup(r => r.GetQuizSessionById("user-1", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var sut = new QuizService(repo.Object, openAi.Object, vision.Object);

        // act
        var result = await sut.GetQuizAsync("user-1", 1, CancellationToken.None);

        // assert
        Assert.NotNull(result);
        Assert.Equal(1, result.QuizSessionId);
        Assert.Single(result.Questions);

        var question = result.Questions.First();
        Assert.Equal("What is 2+2?", question.Text);
        Assert.Equal(4, question.Options.Count);

        repo.Verify(
            r => r.GetQuizSessionById("user-1", 1, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
