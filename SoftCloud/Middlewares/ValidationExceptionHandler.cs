using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoftCloud.Middlewares;
internal sealed class ValidationExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        logger.LogError(exception, "An unhandled exception occurred");

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred",
            }
        };

        context.ProblemDetails.Extensions.Add("errorMessage", validationException.ValidationResult.ErrorMessage);
        context.ProblemDetails.Extensions.Add("errors", validationException.ValidationResult.MemberNames);
        context.ProblemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);

        return await problemDetailsService.TryWriteAsync(context);
    }
}
