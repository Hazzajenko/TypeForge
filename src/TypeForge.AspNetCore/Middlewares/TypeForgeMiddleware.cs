using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TypeForge.AspNetCore.Services;

namespace TypeForge.AspNetCore.Middlewares;

public class TypeForgeMiddleware
{
    private readonly RequestDelegate _next;

    public TypeForgeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, TypeForgeUiService typeForgeUiService)
    {
        if (httpContext.Request.Path.StartsWithSegments("/typeforge"))
        {
            var data = typeForgeUiService.FetchHtmlFromDocument();
            await httpContext.Response.WriteAsync(data);
            return;
        }
        await _next(httpContext);
    }
}

public static class TypeForgeMiddlewareExtensions
{
    public static IApplicationBuilder UseTypeForgeMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TypeForgeMiddleware>();
    }
}
