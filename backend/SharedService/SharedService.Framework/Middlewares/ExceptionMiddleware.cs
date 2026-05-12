using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SharedService.SharedKernel;
using SharedService.SharedKernel.Exceptions;

namespace SharedService.Framework.Middlewares;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError("Message: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        (int statusCode, Error error) = exception switch
        {
            ValidationException ex => (StatusCodes.Status400BadRequest, ex.Error),
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Error),
            ConflictException ex => (StatusCodes.Status409Conflict, ex.Error),
            FailureException ex => (StatusCodes.Status500InternalServerError, ex.Error),
            _ => (StatusCodes.Status500InternalServerError, Error.Failure("failure", exception.Message))
        };

        var envelope = Envelope.Error(error);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(envelope);
    }
}