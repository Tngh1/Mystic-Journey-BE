using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mystic_Journey_API.Extensions;
using Npgsql;

namespace Mystic_Journey_API.Filters
{
    // Executes i exception filter operation.
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        // Initializes a new instance of ApiExceptionFilter with dependencies: logger.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        // Map the exception to an HTTP response, sanitize the message, build a failed ApiResponse, assign the ObjectResult status, and mark the exception handled.
        public void OnException(ExceptionContext context)
        {
            var (statusCode, errorCode) = MapException(context.Exception);

            _logger.LogError(context.Exception, "Unhandled exception: {Message}", context.Exception.Message);

            var rawMessage = context.Exception.Message;
            if (context.Exception.InnerException != null)
            {
                rawMessage += $" | Inner: {context.Exception.InnerException.Message}";
            }

            var cleanMessage = SanitizeErrorMessage(rawMessage, statusCode);

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = cleanMessage,
                ErrorCode = errorCode
            };

            context.Result = new ObjectResult(response) { StatusCode = statusCode };
            context.ExceptionHandled = true;
        }

        // Reject empty or raw HTTP status messages, map known status codes to safe user-facing text, and preserve valid backend messages.
        private static string SanitizeErrorMessage(string? message, int statusCode)
        {
            if (statusCode >= StatusCodes.Status500InternalServerError)
                return GetDefaultStatusMessage(statusCode);

            if (string.IsNullOrWhiteSpace(message))  // Mandatory string argument is blank — fail fast
            {
                return GetDefaultStatusMessage(statusCode);
            }

            var trimmed = message.Trim();

            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^\d{3}$"))
            {
                if (int.TryParse(trimmed, out var code))
                {
                    return GetDefaultStatusMessage(code);
                }
                return GetDefaultStatusMessage(statusCode);
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"(?i)(request failed with status code|status code \d{3}|error \d{3}|^\d{3}\s+)"))
            {
                return GetDefaultStatusMessage(statusCode);
            }

            return trimmed;
        }

        // Map each supported HTTP status code to its safe user-facing message and use the unexpected-error message as the fallback.
        public static string GetDefaultStatusMessage(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "Invalid request. Please check your input parameters.",
            StatusCodes.Status401Unauthorized => "Unauthorized access. Please log in to continue.",
            StatusCodes.Status403Forbidden => "Access denied. You do not have permission to access this resource.",
            StatusCodes.Status404NotFound => "The requested resource was not found.",
            StatusCodes.Status409Conflict => "A conflict occurred with the current state of the resource.",
            StatusCodes.Status422UnprocessableEntity => "Unprocessable entity. Please verify your data input.",
            StatusCodes.Status429TooManyRequests => "Too many requests. Please try again later.",
            StatusCodes.Status500InternalServerError => "An internal server error occurred. Please try again later.",
            StatusCodes.Status503ServiceUnavailable => "The service is temporarily unavailable. Please try again later.",
            _ => "An unexpected error occurred."
        };

        // Translate known exception types into the corresponding HTTP status and stable API error code, falling back to an internal error.
        private static (int StatusCode, string ErrorCode) MapException(Exception ex) => ex switch
        {
            AccountNotFoundException => (StatusCodes.Status404NotFound, ErrorCodes.AccountNotFound),
            KeyNotFoundException => (StatusCodes.Status404NotFound, ErrorCodes.NotFound),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized),
            BadRequestException => (StatusCodes.Status400BadRequest, ErrorCodes.BadRequest),
            ArgumentException => (StatusCodes.Status400BadRequest, ErrorCodes.BadRequest),
            RetryLimitExceededException => (StatusCodes.Status503ServiceUnavailable, ErrorCodes.InternalError),
            DbUpdateException => (StatusCodes.Status500InternalServerError, ErrorCodes.InternalError),
            NpgsqlException => (StatusCodes.Status503ServiceUnavailable, ErrorCodes.InternalError),
            InvalidOperationException => (StatusCodes.Status400BadRequest, ErrorCodes.InvalidOperation),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.InternalError)
        };
    }
}
