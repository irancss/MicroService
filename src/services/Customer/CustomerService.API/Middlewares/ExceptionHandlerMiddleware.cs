using CustomerService.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using BuildingBlocks.Application.Exceptions;

namespace CustomerService.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            object response;

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new { errors = validationException.Errors };
                    break;
                case CustomerNotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    response = new { error = exception.Message };
                    break;
                case DuplicateEmailException _:
                    statusCode = HttpStatusCode.Conflict; // 409 Conflict مناسب‌تر است
                    response = new { error = exception.Message };
                    break;
                default:
                    response = new { error = "An unexpected error occurred." };
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
