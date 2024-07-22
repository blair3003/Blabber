using Blabber.Api.Models;
using Blabber.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController(IAuthorService authorService, IAuthorizationService authorizationService, ILogger<AuthorController> logger) : ControllerBase
    {
        private readonly IAuthorService _authorService = authorService;
        private readonly IAuthorizationService _authorizationService = authorizationService;
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

        [HttpGet("{id}/profile")]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
            {
                var result = await _authorService.GetAuthorProfileByIdAsync(id)
                    ?? throw new KeyNotFoundException("Author not found!");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPost]
        [Authorize]
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

                var newAuthor = await _authorService.AddAuthorAsync(request, currentUserId)
                    ?? throw new InvalidOperationException("Cannot add Author!");

                return CreatedAtAction(nameof(CreateAuthor), new { id = newAuthor.Id }, newAuthor);
            }
            catch (Exception ex)
            {
                return ControllerHelper.HandleException(ex, _logger);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var applicationUserId = await _authorService.GetApplicationUserIdByAuthorIdAsync(id)
                    ?? throw new KeyNotFoundException("Author not found!");

                var checkAuthorUser = await _authorizationService.AuthorizeAsync(User, applicationUserId, "AuthorUser");

                if (!checkAuthorUser.Succeeded)
                {
                    throw new UnauthorizedAccessException("User not authorized to update Author!");
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
        [Authorize]
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
        [Authorize]
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