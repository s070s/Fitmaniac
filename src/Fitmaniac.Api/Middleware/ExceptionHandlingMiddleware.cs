using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", ctx.Request.Method, ctx.Request.Path);
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";
            var problem = new ProblemDetails
            {
                Status = 500,
                Title = "An unexpected error occurred.",
                Detail = ctx.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() ? ex.ToString() : null
            };
            await ctx.Response.WriteAsJsonAsync(problem);
        }
    }
}
