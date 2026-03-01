namespace Note2Quiz.API.Models;

public record GeneratedQuestion(
    string Text,
    List<string> Options,
    int CorrectOptionIndex
);