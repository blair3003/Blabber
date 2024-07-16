using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
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
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(request.AuthorId)
                    ?? throw new KeyNotFoundException("Author not found!");

                if (currentUserId != authorUserId)
                {
                    throw new UnauthorizedAccessException("User not authorized!");
                }

                var newComment = await _commentService.AddCommentAsync(request)
                    ?? throw new InvalidOperationException("Cannot add Comment!");

                return CreatedAtAction(nameof(CreateComment), new { id = newComment.Id }, newComment);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }
    }

}