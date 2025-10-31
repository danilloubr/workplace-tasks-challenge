// Em: backend/src/WorkplaceTasks.Api/Middleware/GlobalErrorHandlingMiddleware.cs
using System.Net;
using System.Text.Json;
using WorkplaceTasks.Application.Exceptions;

namespace WorkplaceTasks.Api.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            HttpStatusCode statusCode;
            string message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404
                    message = exception.Message;
                    break;
                case ForbiddenException:
                    statusCode = HttpStatusCode.Forbidden; // 403
                    message = exception.Message;
                    break;
                case BadRequestException:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    message = exception.Message;
                    break;
                // ---------------------------------

                default:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    message = "Ocorreu um erro interno no servidor.";
                    logger.LogError(exception, "Erro inesperado capturado");
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(result);
        }
    }
}