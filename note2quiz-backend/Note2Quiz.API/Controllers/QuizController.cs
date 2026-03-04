// GET /api/quiz/history
// POST /api/quiz/submit
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
        CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        if (file.Length > 20 * 1024 * 1024)
            return BadRequest("File too large. Max size is 20MB.");

        if (file.ContentType is not ("image/jpeg" or "image/png"))
            return BadRequest("Only jpeg or png allowed.");

        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
            return Unauthorized();

        var request = new CreateQuizRequest(file, difficulty);

        var result = await _quizService.CreateQuizAsync(userId, request, ct);

        return Ok(result);
    }

}