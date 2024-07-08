using Blabber.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blabber.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlabController(IBlabService blabService, ILogger<BlabController> logger) : ControllerBase
    {
        private readonly IBlabService _blabService = blabService;
        private readonly ILogger<BlabController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetBlabs(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _blabService.GetBlabPageAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Blabs.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }

}