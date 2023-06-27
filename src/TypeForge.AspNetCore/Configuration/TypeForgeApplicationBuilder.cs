using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.FileProviders;
using TypeForge.AspNetCore.Middlewares;

namespace TypeForge.AspNetCore.Configuration;

public static class TypeForgeApplicationBuilder
{
    public static IApplicationBuilder UseTypeForge(this IApplicationBuilder app)
    {
        app.UseStaticFiles();
        /*app.UseFileServer(
            new FileServerOptions
            {
                RequestPath = new PathString("/typeforge"),
                FileProvider = new EmbeddedFileProvider(
                    typeof(TypeForgeApplicationBuilder).GetTypeInfo().Assembly,
                    "TypeForge.AspNetCore.Ui"
                )
            }
        );*/

        app.UseTypeForgeMiddleware();
        //
        // app.UseEndpoints(builder =>
        // {
        //     // builder.MapRazorPages();
        //     builder.MapGet(
        //         "/typeforge", context =>
        //         {
        //             context.Response.Redirect("/Ui/index.html");
        //             return Task.CompletedTask;
        //         }
        //     );
        // });

        // app.MapRazorPages();

        return app;
    }

    // public static IApplicationBuilder MapTypeForgeRazorPage(this IEndpointRouteBuilder endpoints)
    // {
    //     endpoints.MapRazorPages();
    //
    // }
}
