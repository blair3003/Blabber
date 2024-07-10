using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;

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

        [HttpPost]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var applicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated.");

                var newAuthor = await _authorService.AddAuthorAsync(request, applicationUserId)
                    ?? throw new InvalidOperationException("AddAuthorAsync returned null.");

                return CreatedAtAction(nameof(CreateAuthor), new { id = newAuthor.Id }, newAuthor);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                _logger.LogError(ex, "Author creation failed due to unique constraint violation.");
                return Conflict("An Author with the same ApplicationUserId already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Author.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }


        }
    }

}