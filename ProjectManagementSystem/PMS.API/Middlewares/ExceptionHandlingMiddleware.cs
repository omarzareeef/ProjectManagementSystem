using PMS.Application.DTOs.Responses;
using PMS.Application.Exceptions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMS.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = GetExceptionResponse(exception);

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Failure(message, errors);

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, SerializerOptions));
    }

    private static (
        HttpStatusCode StatusCode,
        string Message,
        List<string>? Errors
        ) GetExceptionResponse(Exception exception) =>
        exception switch
        {
            NotFoundException ex =>
                (HttpStatusCode.NotFound, ex.Message, null),

            BadRequestException ex =>
                (HttpStatusCode.BadRequest, ex.Message, ex.Errors.Count > 0 ? ex.Errors.ToList() : null),

            UnauthorizedException ex =>
                (HttpStatusCode.Unauthorized, ex.Message, null),

            ServiceUnavailableException ex =>
                (HttpStatusCode.ServiceUnavailable, ex.Message, null),

            ArgumentNullException ex =>
                (HttpStatusCode.BadRequest, "A required argument was null.", null),

            ArgumentException ex =>
                (HttpStatusCode.BadRequest, "Invalid argument provided.", null),

            KeyNotFoundException ex =>
                (HttpStatusCode.NotFound, "The requested resource was not found.", new List<string> { ex.Message }),

            UnauthorizedAccessException ex =>
                (HttpStatusCode.Unauthorized, "You are not authorized to access this resource.", new List<string> { ex.Message }),

            InvalidOperationException ex =>
                (HttpStatusCode.BadRequest, "The operation is not valid in the current state.", new List<string> { ex.Message }),

            _ =>
                (HttpStatusCode.InternalServerError, "An unexpected server error occurred.", null)
        };
}
