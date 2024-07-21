using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/comments")]
    [Authorize]
    public class CommentController(ICommentService commentService, IAuthorService authorService, IAuthorizationService authorizationService, ILogger<CommentController> logger) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;
        private readonly IAuthorService _authorService = authorService;
        private readonly IAuthorizationService _authorizationService = authorizationService;
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
                var applicationUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(request.AuthorId)
                    ?? throw new KeyNotFoundException("Author not found!");

                var checkAuthorUser = await _authorizationService.AuthorizeAsync(User, applicationUserId, "AuthorUser");

                if (!checkAuthorUser.Succeeded)
                {
                    throw new UnauthorizedAccessException("User not authorized to create Comment!");
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id)
                    ?? throw new KeyNotFoundException("Comment not found!");

                var applicationUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(comment.Author!.Id)
                    ?? throw new KeyNotFoundException("Author not found!");

                var checkAuthorUser = await _authorizationService.AuthorizeAsync(User, applicationUserId, "AuthorUser");

                if (!checkAuthorUser.Succeeded)
                {
                    throw new UnauthorizedAccessException("User not authorized to update Comment!");
                }

                var checkCommentUpdateTimeLimit = await _authorizationService.AuthorizeAsync(User, comment.CreatedAt, "CommentUpdateTimeLimit");

                if (!checkCommentUpdateTimeLimit.Succeeded)
                {
                    throw new UnauthorizedAccessException("Comment update time limit expired!");
                }

                var updatedComment = await _commentService.UpdateCommentAsync(id, request)
                    ?? throw new InvalidOperationException("Cannot update Comment!");

                return Ok(updatedComment);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var comment = await _commentService.GetCommentByIdAsync(id)
                    ?? throw new KeyNotFoundException("Comment not found!");

                var applicationUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(comment.Author!.Id)
                    ?? throw new KeyNotFoundException("Author not found!");

                var checkAuthorUser = await _authorizationService.AuthorizeAsync(User, applicationUserId, "AuthorUser");

                if (!checkAuthorUser.Succeeded)
                {
                    throw new UnauthorizedAccessException("User not authorized to delete Comment!");
                }

                var deletedComment = await _commentService.DeleteCommentAsync(id)
                    ?? throw new InvalidOperationException("Cannot delete Comment!");

                return Ok(deletedComment);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }
    }

}