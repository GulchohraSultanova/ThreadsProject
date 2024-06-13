
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using ThreadsProject.Bussiness.GlobalException;
using ThreadsProject.Core.Entities;



namespace ThreadsProject.Bussiness.GlobalException
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError($"A known error occurred: {ex.Message}");
                await HandleGlobalAppExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleGlobalAppExceptionAsync(HttpContext context, GlobalAppException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // BadRequest: 400

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Error = exception.Message,
            
            }.ToString());
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // InternalServerError: 500

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Error = "Internal Server Error from the custom middleware."
            }.ToString());
        }
    }
}