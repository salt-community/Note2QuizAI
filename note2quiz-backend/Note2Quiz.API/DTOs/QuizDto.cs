namespace Note2Quiz.API.DTOs;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public record CreateQuizRequest(
    IFormFile Image,
    Difficulty Difficulty
);

public record QuizResponse(
    int QuizSessionId,
    List<QuestionDto> Questions
);

public record QuestionDto(
    int Id,
    string Text,
    List<OptionDto> Options
);

public record OptionDto(
    int OptionId,
    string OptionText
);

public record SubmitQuizRequest(
    int QuizSessionId,
    List<AnswerDto> Answers
);

public record AnswerDto(
    int QuestionId,
    int SelectedOptionId
);

public record SubmitQuizResponse(
    int QuizSessionId,
    int Score,
    int TotalQuestions,
    List<QuestionResultDto> Results
);

public record QuestionResultDto(
    int QuestionId,
    int SelectedOptionId,
    int CorrectOptionId,
    bool IsCorrect
);

public record QuizHistoryItemDto(
    int QuizSessionId,
    DateTime CreatedAt,
    int QuestionCount,
    int? Score
);