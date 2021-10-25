﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

#pragma warning disable CA1822
namespace Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;

internal class TestStartup
{
    public TestStartup(
        IWebHostEnvironment environment,
        IConfiguration configuration
    )
    {
        Environment = environment;
        Configuration = configuration;
    }

    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }

    public void Configure(IApplicationBuilder app)
    {
        app.Run(context => context.Response.WriteAsync("TestStartup -> Compose"));
    }
}
