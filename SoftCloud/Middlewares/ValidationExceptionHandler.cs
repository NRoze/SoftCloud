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

        //TBD: Add more details about the validation errors, such as the specific fields that failed validation and the error messages.
        context.ProblemDetails.Extensions.Add("errors", validationException.ValidationResult.MemberNames);

        return await problemDetailsService.TryWriteAsync(context);
    }
}
