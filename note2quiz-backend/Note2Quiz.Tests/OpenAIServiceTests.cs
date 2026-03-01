using Moq;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Services;
using Xunit;

namespace Note2Quiz.Tests.Services;

public class OpenAIServiceTests
{
    [Fact]
    public async Task GenerateQuizAsync_ReturnsQuestions_WhenJsonIsValid()
    {
        var chat = new Mock<IChatClient>();

        chat.Setup(x => x.GetCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(
                """
                {
                  "questions": [
                    { "question": "Q1", "options": ["A","B","C","D"], "correctOptionIndex": 1 },
                    { "question": "Q2", "options": ["A","B","C","D"], "correctOptionIndex": 2 },
                    { "question": "Q3", "options": ["A","B","C","D"], "correctOptionIndex": 0 },
                    { "question": "Q4", "options": ["A","B","C","D"], "correctOptionIndex": 3 },
                    { "question": "Q5", "options": ["A","B","C","D"], "correctOptionIndex": 1 }
                  ]
                }
                """
            );

        var sut = new OpenAIService(chat.Object);

        var result = await sut.GenerateQuizAsync("some text", Difficulty.Easy, CancellationToken.None);

        Assert.Equal(5, result.Count);

        foreach (var q in result)
        {
            Assert.False(string.IsNullOrWhiteSpace(q.Text));
            Assert.Equal(4, q.Options.Count);
            Assert.InRange(q.CorrectOptionIndex, 0, 3);
        }
    }

    [Fact]
    public async Task GenerateQuizAsync_Throws_WhenOptionsNotFour()
    {
        var chat = new Mock<IChatClient>();

        chat.Setup(x => x.GetCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(
                """
                {
                  "questions": [
                    { "question": "Q1", "options": ["A","B","C"], "correctOptionIndex": 1 },
                    { "question": "Q2", "options": ["A","B","C","D"], "correctOptionIndex": 2 },
                    { "question": "Q3", "options": ["A","B","C","D"], "correctOptionIndex": 0 },
                    { "question": "Q4", "options": ["A","B","C","D"], "correctOptionIndex": 3 },
                    { "question": "Q5", "options": ["A","B","C","D"], "correctOptionIndex": 1 }
                  ]
                }
                """
            );

        var sut = new OpenAIService(chat.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.GenerateQuizAsync("some text", Difficulty.Easy, CancellationToken.None));
    }

    [Fact]
    public async Task GenerateQuizAsync_Throws_WhenCorrectIndexOutOfRange()
    {
        var chat = new Mock<IChatClient>();

        chat.Setup(x => x.GetCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(
                """
                {
                  "questions": [
                    { "question": "Q1", "options": ["A","B","C","D"], "correctOptionIndex": 9 },
                    { "question": "Q2", "options": ["A","B","C","D"], "correctOptionIndex": 2 },
                    { "question": "Q3", "options": ["A","B","C","D"], "correctOptionIndex": 0 },
                    { "question": "Q4", "options": ["A","B","C","D"], "correctOptionIndex": 3 },
                    { "question": "Q5", "options": ["A","B","C","D"], "correctOptionIndex": 1 }
                  ]
                }
                """
            );

        var sut = new OpenAIService(chat.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.GenerateQuizAsync("some text", Difficulty.Medium, CancellationToken.None));
    }

    [Fact]
    public async Task GenerateQuizAsync_Throws_WhenJsonIsInvalid()
    {
        var chat = new Mock<IChatClient>();

        chat.Setup(x => x.GetCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync("not-json");

        var sut = new OpenAIService(chat.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.GenerateQuizAsync("some text", Difficulty.Easy, CancellationToken.None));
    }
}