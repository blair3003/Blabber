using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/blabs")]
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
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlab(int id)
        {
            try
            {
                var result = await _blabService.GetBlabByIdAsync(id)
                    ?? throw new KeyNotFoundException("Blab not found!");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
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
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(request.AuthorId)
                    ?? throw new KeyNotFoundException("Author not found!");

                if (currentUserId != authorUserId)
                {
                    throw new UnauthorizedAccessException("User not authorized!");
                }

                var newBlab = await _blabService.AddBlabAsync(request)
                    ?? throw new InvalidOperationException("Cannot add Blab!");

                return CreatedAtAction(nameof(CreateBlab), new { id = newBlab.Id }, newBlab);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
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
                var blab = await _blabService.GetBlabByIdAsync(id)
                    ?? throw new KeyNotFoundException("Blab not found!");

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(blab.Author!.Id)
                    ?? throw new KeyNotFoundException("Author not found!");

                if (currentUserId != authorUserId)
                {
                    throw new UnauthorizedAccessException("User not authorized!");
                }

                var updatedBlab = await _blabService.UpdateBlabAsync(id, request)
                    ?? throw new InvalidOperationException("Cannot update Blab!");

                return Ok(updatedBlab);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikeBlab(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorId = await _authorService.GetAuthorIdByApplicationUserIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("Author not found!");

                var like = await _blabService.AddBlabLikeAsync(id, authorId);

                return Ok(like);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPost("{id}/unlike")]
        public async Task<IActionResult> UnLikeBlab(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorId = await _authorService.GetAuthorIdByApplicationUserIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("Author not found!");

                var unlike = await _blabService.RemoveBlabLikeAsync(id, authorId);

                return Ok(unlike);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }
    }

}