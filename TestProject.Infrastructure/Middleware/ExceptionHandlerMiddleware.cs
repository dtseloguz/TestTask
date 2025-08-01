using MeteoriteSync.Core.Results;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using TestProject.Core.Interfaces;

namespace TestProject.Infrastructure.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogRepository _logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await _logger.LogErrorAsync("Unhandled exception", ex, "ExceptionHandlerMiddleware");

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var result = Result.Failure(Error.ServerError());
            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }
    }
}