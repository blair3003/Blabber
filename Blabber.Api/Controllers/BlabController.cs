using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlabController(IBlabService blabService, IAuthorService authorService, ILogger<BlabController> logger) : ControllerBase
    {
        private readonly IBlabService _blabService = blabService;
        private readonly IAuthorService _authorService = authorService;
        private readonly ILogger<BlabController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetBlabs(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _blabService.GetBlabFeedAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Blabs.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlab(int id)
        {
            try
            {
                var result = await _blabService.GetBlabByIdAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Blab.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlab([FromBody] BlabCreateRequest request)
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

                var newBlab = await _blabService.AddBlabAsync(request)
                    ?? throw new InvalidOperationException("AddBlabAsync returned null.");

                return CreatedAtAction(nameof(CreateBlab), new { id = newBlab.Id }, newBlab);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Blab.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlab(int id, [FromBody] BlabUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var blab = await _blabService.GetBlabByIdAsync(id);

                if (blab == null)
                {
                    return NotFound();
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated.");

                var authorUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(blab.Author!.Id)
                    ?? throw new InvalidOperationException("GetApplicationUserIdByAuthorIdAsync returned null.");

                if (currentUserId != authorUserId)
                {
                    return Forbid("Access denied.");
                }

                var updatedBlab = await _blabService.UpdateBlabAsync(id, request)
                    ?? throw new InvalidOperationException("UpdateBlabAsync returned null.");

                return Ok(updatedBlab);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Blab.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }

}