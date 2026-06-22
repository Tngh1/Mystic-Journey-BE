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

            var response = new ApiResponse<object>
            {
                Success = false,
                Message = context.Exception.Message,
                ErrorCode = errorCode
            };

            context.Result = new ObjectResult(response) { StatusCode = statusCode };
            context.ExceptionHandled = true;
        }

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
