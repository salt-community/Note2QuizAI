using Moq;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Services.OpenAI;
using Note2Quiz.API.Services.OpenAI.Models;

namespace Note2Quiz.Tests.Services;

public class OpenAIServiceTests
{
    [Fact]
    public async Task GenerateQuizAsync_ReturnsQuestions_WhenJsonIsValid()
    {
        // Arrange
        var chat = new Mock<IChatClient>();

        chat.Setup(x =>
                x.GetCompletionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ChatSettings>(),
                    It.IsAny<CancellationToken>()
                )
            )
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

        // Act
        var result = await sut.GenerateQuizAsync(
            "some text",
            Difficulty.Easy,
            CancellationToken.None
        );

        // Assert
        Assert.Equal(5, result.Questions.Count);
        foreach (var q in result.Questions)
        {
            Assert.False(string.IsNullOrWhiteSpace(q.Question));
            Assert.Equal(4, q.Options.Count);
            Assert.InRange(q.CorrectOptionIndex, 0, 3);
        }
    }

    [Fact]
    public async Task GenerateQuizAsync_SalvagesValidQuestions_WhenSomeAreInvalid()
    {
        // Arrange
        var chat = new Mock<IChatClient>();

        chat.Setup(x =>
                x.GetCompletionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ChatSettings>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                """
                {
                  "questions": [
                    { "question": "Broken Q", "options": ["OnlyOne"], "correctOptionIndex": 0 },
                    { "question": "Valid Q", "options": ["A","B","C","D"], "correctOptionIndex": 1 }
                  ]
                }
                """
            );

        var sut = new OpenAIService(chat.Object);

        // Act
        var result = await sut.GenerateQuizAsync(
            "some text",
            Difficulty.Easy,
            CancellationToken.None
        );

        // Assert
        // remove Q instead of thowing exception
        Assert.Single(result.Questions);
        Assert.Equal("Valid Q", result.Questions[0].Question);
    }

    [Fact]
    public async Task GenerateQuizAsync_Throws_WhenNoValidQuestionsRemain()
    {
        // Arrange
        var chat = new Mock<IChatClient>();

        chat.Setup(x =>
                x.GetCompletionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ChatSettings>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                """
                {
                  "questions": [
                    { "question": "Bad Q", "options": ["A"], "correctOptionIndex": 99 }
                  ]
                }
                """
            );

        var sut = new OpenAIService(chat.Object);

        // Act & Assert
        // Throws exception only when the list is completely empty after filtering
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.GenerateQuizAsync("some text", Difficulty.Easy, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GenerateQuizAsync_Throws_WhenJsonIsInvalid()
    {
        // Arrange
        var chat = new Mock<IChatClient>();

        chat.Setup(x =>
                x.GetCompletionAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ChatSettings>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync("not-json");

        var sut = new OpenAIService(chat.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.GenerateQuizAsync("some text", Difficulty.Easy, CancellationToken.None)
        );
    }
}
