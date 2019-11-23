using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CA1801

namespace Rocket.Surgery.Hosting.AspNetCore.Tests.Startups
{
    internal class SimpleStartup
    {
        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder app) => app.Use(
            (context, func) => context.Response.WriteAsync("SimpleStartup -> Configure")
        );
    }
}