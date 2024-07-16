using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController(IAuthorService authorService, ILogger<AuthorController> logger) : ControllerBase
    {
        private readonly IAuthorService _authorService = authorService;
        private readonly ILogger<AuthorController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            try
            {
                var result = await _authorService.GetAllAuthorsAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                var result = await _authorService.GetAuthorByIdAsync(id)
                    ?? throw new KeyNotFoundException("Author not found!");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                if (currentUserId != request.ApplicationUserId)
                {
                    throw new UnauthorizedAccessException("User not authorized!");
                }

                var newAuthor = await _authorService.AddAuthorAsync(request)
                    ?? throw new InvalidOperationException("Cannot add Author!");

                return CreatedAtAction(nameof(CreateAuthor), new { id = newAuthor.Id }, newAuthor);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(id)
                    ?? throw new KeyNotFoundException("Author not found!");

                if (currentUserId != authorUserId)
                {
                    throw new UnauthorizedAccessException("User not authorized!");
                }

                var updatedAuthor = await _authorService.UpdateAuthorAsync(id, request)
                    ?? throw new InvalidOperationException("Cannot update Author!");

                return Ok(updatedAuthor);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPost("{id}/follow")]
        public async Task<IActionResult> FollowAuthor(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var followerId = await _authorService.GetAuthorIdByApplicationUserIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("Follower not found!");

                var result = await _authorService.AddAuthorFollowerAsync(id, followerId);
                if (!result)
                {
                    throw new InvalidOperationException("Cannot follow Author!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPost("{id}/unfollow")]
        public async Task<IActionResult> UnfollowAuthor(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var followerId = await _authorService.GetAuthorIdByApplicationUserIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("Follower not found!");

                var result = await _authorService.RemoveAuthorFollowerAsync(id, followerId);

                if (!result)
                {
                    throw new InvalidOperationException("Cannot unfollow Author!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }
    }

}