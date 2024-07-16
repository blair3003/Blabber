using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace Blabber.Api.Controllers
{
    public class ControllerHelper
    {
        public static IActionResult HandleException(Exception ex, ILogger logger)
        {
            switch (ex)
            {
                case AuthenticationException authEx:
                    logger.LogError(authEx, "Authentication error: {Message}", authEx.Message);
                    return new UnauthorizedObjectResult(authEx.Message);

                case UnauthorizedAccessException authEx:
                    logger.LogError(authEx, "Authorization error: {Message}", authEx.Message);
                    return new UnauthorizedObjectResult(authEx.Message);

                case KeyNotFoundException notFoundEx:
                    logger.LogError(notFoundEx, "Not found error: {Message}", notFoundEx.Message);
                    return new NotFoundObjectResult(notFoundEx.Message);

                case InvalidOperationException invalidOpEx:
                    logger.LogError(invalidOpEx, "Invalid operation: {Message}", invalidOpEx.Message);
                    return new BadRequestObjectResult(invalidOpEx.Message);

                default:
                    logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                    return new ObjectResult("An error occurred while processing your request.")
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
            }
        }
    }
}
