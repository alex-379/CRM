using CRM.API.Configuration.Exceptions.Constants;
using CRM.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CRM.API.Configuration;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly Serilog.ILogger _logger = Log.ForContext<GlobalExceptionHandler>();

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.Error(
            exception, GlobalExceptions.LoggerError, exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = exception.GetType().Name,
            Title = GlobalExceptions.InternalServerErrorException,
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        switch (problemDetails.Type)
        {
            case nameof(ConflictException):
                {
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Title = GlobalExceptions.ConflictException;
                };

                break;

            case nameof(NotFoundException):
                {
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = GlobalExceptions.NotFoundException;
                };

                break;

            case nameof(UnauthorizedException):
                {
                    problemDetails.Status = StatusCodes.Status403Forbidden;
                    problemDetails.Title = GlobalExceptions.UnauthorizedException;
                };

                break;

            case nameof(UnauthenticatedException):
                {
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = GlobalExceptions.UnauthenticatedException;
                };

                break;

            default:
                {
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Title = GlobalExceptions.ConflictException;
                };
                
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
