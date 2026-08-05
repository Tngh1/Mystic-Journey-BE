using BLL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mystic_Journey_API.Extensions;

namespace Mystic_Journey_API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

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

        private static string SanitizeErrorMessage(string? message, int statusCode)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return GetDefaultStatusMessage(statusCode);
            }

            var trimmed = message.Trim();

            // If message is purely digits (e.g. "404", "401", "500")
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^\d{3}$"))
            {
                if (int.TryParse(trimmed, out var code))
                {
                    return GetDefaultStatusMessage(code);
                }
                return GetDefaultStatusMessage(statusCode);
            }

            // If message matches status code string patterns
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"(?i)(request failed with status code|status code \d{3}|error \d{3}|^\d{3}\s+)"))
            {
                return GetDefaultStatusMessage(statusCode);
            }

            return trimmed;
        }

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
            _ => "An unexpected error occurred."
        };

        private static (int StatusCode, string ErrorCode) MapException(Exception ex) => ex switch
        {
            AccountNotFoundException => (StatusCodes.Status404NotFound, ErrorCodes.AccountNotFound),
            KeyNotFoundException => (StatusCodes.Status404NotFound, ErrorCodes.NotFound),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized),
            BadRequestException => (StatusCodes.Status400BadRequest, ErrorCodes.BadRequest),
            ArgumentException => (StatusCodes.Status400BadRequest, ErrorCodes.BadRequest),
            InvalidOperationException => (StatusCodes.Status400BadRequest, ErrorCodes.InvalidOperation),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.InternalError)
        };
    }
}
