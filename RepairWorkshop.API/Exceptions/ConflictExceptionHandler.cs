using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RepairWorkshop.BLL.Exceptions;

namespace RepairWorkshop.API.Exceptions
{
    public class ConflictExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ConflictExceptionHandler> _logger;

        public ConflictExceptionHandler(ILogger<ConflictExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ConflictException conflictException)
            {
                return false;
            }

            _logger.LogError(
                conflictException,
                "Exception occurred: {Message}",
                conflictException.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Cannot delete entity with dependencies",
                Detail = conflictException.Message
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
