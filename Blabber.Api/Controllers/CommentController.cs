using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController(ICommentService commentService, IAuthorService authorService, ILogger<CommentController> logger) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;
        private readonly IAuthorService _authorService = authorService;
        private readonly ILogger<CommentController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated.");

                var authorUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(request.AuthorId)
                    ?? throw new InvalidOperationException("GetApplicationUserIdByAuthorIdAsync returned null.");

                if (currentUserId != authorUserId)
                {
                    return Forbid("Access denied.");
                }

                var newComment = await _commentService.AddCommentAsync(request)
                    ?? throw new InvalidOperationException("AddCommentAsync returned null.");

                return CreatedAtAction(nameof(CreateComment), new { id = newComment.Id }, newComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Comment.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }

}