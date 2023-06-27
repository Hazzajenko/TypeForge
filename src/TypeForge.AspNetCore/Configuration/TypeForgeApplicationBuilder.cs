using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using TypeForge.AspNetCore.Middlewares;
using TypeForge.AspNetCore.Services;

namespace TypeForge.AspNetCore.Configuration;

public static class TypeForgeApplicationBuilder
{
    public static IApplicationBuilder UseTypeForge(this IApplicationBuilder app)
    {
        app.UseEndpoints(builder =>
        {
            // builder.MapRazorPages();
            builder.MapGet(
                "/typeforge",
                async (HttpContext context, TypeForgeUiService typeForgeUiService) =>
                {
                    var result = typeForgeUiService.FetchHtmlFromDocument();
                    await context.Response.WriteAsync(result);
                }
            );
        });

        return app;
    }
}
