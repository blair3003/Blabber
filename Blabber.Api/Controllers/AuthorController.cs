using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                _logger.LogError(ex, "Error retrieving Authors.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                var result = await _authorService.GetAuthorByIdAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Author.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
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
                    ?? throw new AuthenticationException("User not authenticated.");

                if (currentUserId != request.ApplicationUserId)
                {
                    return Forbid("Access denied.");
                }

                var newAuthor = await _authorService.AddAuthorAsync(request)
                    ?? throw new InvalidOperationException("AddAuthorAsync returned null.");

                return CreatedAtAction(nameof(CreateAuthor), new { id = newAuthor.Id }, newAuthor);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                _logger.LogError(ex, "Author creation failed due to unique constraint violation.");
                return Conflict("An Author for this user already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Author.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
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
                    ?? throw new AuthenticationException("User not authenticated.");

                var authorUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(id)
                    ?? throw new InvalidOperationException("GetApplicationUserIdByAuthorIdAsync returned null.");

                if (currentUserId != authorUserId)
                {
                    return Forbid("Access denied.");
                }

                var updatedAuthor = await _authorService.UpdateAuthorAsync(id, request)
                    ?? throw new InvalidOperationException("UpdateAuthorAsync returned null.");

                return Ok(updatedAuthor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Author.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }

}