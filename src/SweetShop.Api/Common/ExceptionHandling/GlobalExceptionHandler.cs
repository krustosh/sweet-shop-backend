using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SweetShop.Api.Common.Models;

namespace SweetShop.Api.Common.ExceptionHandling;

/// <summary>
/// Represents a global exception handler that captures unhandled exceptions and returns standardized API error responses.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private static readonly Action<ILogger, Exception?> LogUnhandledException =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(LogUnhandledException)),
            "Unhandled exception occurred while processing the request.");

    private readonly ILogger<GlobalExceptionHandler> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class with the specified logger.
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }
/// <summary>
/// Attempts to handle an unhandled exception by logging the error and returning a standardized API error response.
/// </summary>
/// <param name="httpContext"></param>
/// <param name="exception"></param>
/// <param name="cancellationToken"></param>
/// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        LogUnhandledException(logger, exception);

        var (statusCode, errorCode, message) = exception switch
        {
            ArgumentException =>
                (
                    StatusCodes.Status400BadRequest,
                    "INVALID_REQUEST",
                    exception.Message
                ),

            InvalidOperationException =>
                (
                    StatusCodes.Status400BadRequest,
                    "BUSINESS_RULE_VIOLATION",
                    exception.Message
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "INTERNAL_SERVER_ERROR",
                    "An unexpected error occurred."
                )
        };

        httpContext.Response.StatusCode = statusCode;

        var response = new ApiErrorResponse(
            new ApiError(
                errorCode,
                message,
                Array.Empty<ApiErrorDetail>()));

        await httpContext.Response.WriteAsJsonAsync(
            response,
            cancellationToken);

        return true;
    }
}