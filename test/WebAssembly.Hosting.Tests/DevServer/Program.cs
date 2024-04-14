using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests.DevServer;

/// <summary>
///     Intended for framework test use only.
/// </summary>
public class DevHostServerProgram
{
    /// <summary>
    ///     Intended for framework test use only.
    /// </summary>
    public static IHost BuildWebHost(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        {
            var applicationPath = args.SkipWhile(a => a != "--applicationpath").Skip(1).First();
            var applicationDirectory = Path.GetDirectoryName(applicationPath)!;
            var name = Path.ChangeExtension(applicationPath, ".staticwebassets.runtime.json");
            name = !File.Exists(name) ? Path.ChangeExtension(applicationPath, ".StaticWebAssets.xml") : name;

            var inMemoryConfiguration = new Dictionary<string, string?>
            {
                [WebHostDefaults.EnvironmentKey] = "Development",
                ["Logging:LogLevel:Microsoft"] = "Warning",
                ["Logging:LogLevel:Microsoft.Hosting.Lifetime"] = "Information",
                [WebHostDefaults.StaticWebAssetsKey] = name,
                ["ApplyCopHeaders"] = args.Contains("--apply-cop-headers").ToString(),
            };

            builder.Configuration.AddInMemoryCollection(inMemoryConfiguration);
            builder.Configuration.AddJsonFile(Path.Combine(applicationDirectory, "blazor-devserversettings.json"), true, true);
        }
        builder.Services.AddRouting();

        builder.WebHost.UseStaticWebAssets();


        var app = builder.Build();
        app.UseDeveloperExceptionPage();
        EnableConfiguredPathbase(app, builder.Configuration);

        app.UseWebAssemblyDebugging();

        var applyCopHeaders = builder.Configuration.GetValue<bool>("ApplyCopHeaders");

        if (applyCopHeaders)
        {
            app.Use(
                async (ctx, next) =>
                {
                    if (ctx.Request.Path.StartsWithSegments("/_framework")
                     && !ctx.Request.Path.StartsWithSegments("/_framework/blazor.server.js")
                     && !ctx.Request.Path.StartsWithSegments("/_framework/blazor.web.js"))
                    {
                        var fileExtension = Path.GetExtension(ctx.Request.Path);
                        if (string.Equals(fileExtension, ".js"))
                        {
                            // Browser multi-threaded runtime requires cross-origin policy headers to enable SharedArrayBuffer.
                            ApplyCrossOriginPolicyHeaders(ctx);
                        }
                    }

                    await next(ctx);
                }
            );
        }

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles(
            new StaticFileOptions
            {
                // In development, serve everything, as there's no other way to configure it.
                // In production, developers are responsible for configuring their own production server
                ServeUnknownFileTypes = true,
            }
        );

        app.UseRouting();
        app.MapFallbackToFile(
            "index.html",
            new StaticFileOptions
            {
                OnPrepareResponse = fileContext =>
                                    {
                                        if (applyCopHeaders)
                                        {
                                            // Browser multi-threaded runtime requires cross-origin policy headers to enable SharedArrayBuffer.
                                            ApplyCrossOriginPolicyHeaders(fileContext.Context);
                                        }
                                    },
            }
        );

        return app;
    }

    private static void EnableConfiguredPathbase(IApplicationBuilder app, IConfiguration configuration)
    {
        var pathBase = configuration.GetValue<string>("pathbase");
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);

            // To ensure consistency with a production environment, only handle requests
            // that match the specified pathbase.
            app.Use(
                (context, next) =>
                {
                    if (context.Request.PathBase == pathBase)
                    {
                        return next(context);
                    }

                    context.Response.StatusCode = 404;
                    return context.Response.WriteAsync($"The server is configured only to " + $"handle request URIs within the PathBase '{pathBase}'.");
                }
            );
        }
    }

    private static void ApplyCrossOriginPolicyHeaders(HttpContext httpContext)
    {
        httpContext.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
        httpContext.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
    }
}