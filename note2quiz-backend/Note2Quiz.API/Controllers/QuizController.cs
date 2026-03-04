// GET /api/quiz/history
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Note2Quiz.API.DTOs;
using Note2Quiz.API.Interfaces;
using Note2Quiz.API.Services;

namespace Note2Quiz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizController : ControllerBase
{
    private IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    // POST /api/quiz/generate
    [HttpPost("generate")]
    public async Task<ActionResult<QuizResponse>> Create(
        [FromForm] CreateQuizRequest request,
        CancellationToken ct
    )
    {
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null)
            return Unauthorized();
        if (request.Image == null || request.Image.Length == 0)
            return BadRequest("Image is Required");
        var result = _quizService.CreateQuizAsync(userId, request, ct);
        return Ok(result);
    }
    
}
