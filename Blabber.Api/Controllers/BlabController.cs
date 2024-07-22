using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/blabs")]
    public class BlabController(IBlabService blabService, IAuthorService authorService, IAuthorizationService authorizationService, ILogger<BlabController> logger) : ControllerBase
    {
        private readonly IBlabService _blabService = blabService;
        private readonly IAuthorService _authorService = authorService;
        private readonly IAuthorizationService _authorizationService = authorizationService;
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

        [HttpGet("author/{authorId}")]
        public async Task<IActionResult> GetBlabs(int authorId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _blabService.GetBlabFeedAsync(pageNumber, pageSize, authorId);
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
        [Authorize]
        public async Task<IActionResult> CreateBlab([FromBody] BlabCreateRequest request)
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
                    throw new UnauthorizedAccessException("User not authorized to create Blab!");
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
        [Authorize]
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

                var applicationUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(blab.Author!.Id)
                    ?? throw new KeyNotFoundException("Author not found!");

                var checkAuthorUser = await _authorizationService.AuthorizeAsync(User, applicationUserId, "AuthorUser");

                if (!checkAuthorUser.Succeeded)
                {
                    throw new UnauthorizedAccessException("User not authorized to update Blab!");
                }

                var checkBlabUpdateTimeLimit = await _authorizationService.AuthorizeAsync(User, blab.CreatedAt, "BlabUpdateTimeLimit");

                if (!checkBlabUpdateTimeLimit.Succeeded)
                {
                    throw new UnauthorizedAccessException("Blab update time limit expired!");
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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlab(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var blab = await _blabService.GetBlabByIdAsync(id)
                    ?? throw new KeyNotFoundException("Blab not found!");

                var applicationUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(blab.Author!.Id)
                    ?? throw new KeyNotFoundException("Author not found!");

                var checkAuthorUser = await _authorizationService.AuthorizeAsync(User, applicationUserId, "AuthorUser");

                if (!checkAuthorUser.Succeeded)
                {
                    throw new UnauthorizedAccessException("User not authorized to delete Blab!");
                }

                var deletedBlab = await _blabService.DeleteBlabAsync(id)
                    ?? throw new InvalidOperationException("Cannot delete Blab!");

                return Ok(deletedBlab);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }


        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikeBlab(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorId = await _authorService.GetAuthorIdByApplicationUserIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("Author not found!");

                var result = await _blabService.AddBlabLikeAsync(id, authorId);

                if (!result)
                {
                    throw new InvalidOperationException("Cannot like Blab!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPost("{id}/unlike")]
        [Authorize]
        public async Task<IActionResult> UnLikeBlab(int id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new AuthenticationException("User not authenticated!");

                var authorId = await _authorService.GetAuthorIdByApplicationUserIdAsync(currentUserId)
                    ?? throw new KeyNotFoundException("Author not found!");

                var result = await _blabService.RemoveBlabLikeAsync(id, authorId);

                if (!result)
                {
                    throw new InvalidOperationException("Cannot unlike Blab!");
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