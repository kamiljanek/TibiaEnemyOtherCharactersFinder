using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    public GlobalExceptionHandlingMiddleware()
    {
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var details = new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error.",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Detail = "An error occurred while processing your request.",
            };

            string json = JsonSerializer.Serialize(details);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
}