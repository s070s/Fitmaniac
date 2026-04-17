namespace Fitmaniac.Api.Middleware;

public sealed class DisabledUserMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        if (ctx.Items.TryGetValue("UserDisabled", out var flag) && flag is true)
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            ctx.Response.Headers["X-User-Disabled"] = "1";
            await ctx.Response.WriteAsync("Account is disabled.");
            return;
        }

        await next(ctx);
    }
}
