// POST /api/quiz/submit
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Interfaces;

namespace Note2Quiz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpPost]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<ActionResult<QuizResponse>> Create(
        [FromForm] IFormFile file,
        [FromForm] Difficulty difficulty,
        CancellationToken ct
    )
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        if (file.Length > 20 * 1024 * 1024)
            return BadRequest("File too large. Max size is 20MB.");

        if (file.ContentType is not ("image/jpeg" or "image/png"))
            return BadRequest("Only jpeg or png allowed.");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var request = new CreateQuizRequest(file, difficulty);

        var result = await _quizService.CreateQuizAsync(userId, request, ct);

        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<QuizHistoryItemDto>>> GetQuizHistory(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var result = await _quizService.GetQuizzesAsync(userId, ct);
        return Ok(result);
    }


    [HttpGet("{quizSessionId}")]
    public async Task<ActionResult<QuizResponse>> GetQuiz(int quizSessionId, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var result = await _quizService.GetQuizAsync(userId, quizSessionId, ct);

        return Ok(result);
    }

    [HttpPost("submit")]
    public async Task<ActionResult<SubmitQuizResponse>> Submit(
        [FromBody] SubmitQuizRequest request,
        CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        if (request == null)
            return BadRequest("Request is required.");

        if (request.QuizSessionId <= 0)
            return BadRequest("QuizSessionId is invalid.");

        if (request.Answers == null || request.Answers.Count == 0)
            return BadRequest("Answers are required.");

        var result = await _quizService.SubmitQuizAsync(userId, request, ct);

        return Ok(result);
    }
}
