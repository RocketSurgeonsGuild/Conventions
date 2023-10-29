using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

#pragma warning disable CA1822
namespace Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;

internal sealed class TestStartup(IWebHostEnvironment environment, IConfiguration configuration)
{
    public IWebHostEnvironment Environment { get; } = environment;
    public IConfiguration Configuration { get; } = configuration;

    public void Configure(IApplicationBuilder app)
    {
        app.Run(context => context.Response.WriteAsync("TestStartup -> Compose"));
    }
}
