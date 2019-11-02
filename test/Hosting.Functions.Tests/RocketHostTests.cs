using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Extensions.Configuration;
using Xunit;

namespace Rocket.Surgery.Hosting.Functions.Tests
{
    public class RocketHostTests
    {
        [Fact]
        public void Creates_RocketHost_ForAppDomain()
        {
            var host = new WebJobsBuilder().UseRocketBooster(new Startup(), RocketBooster.For(AppDomain.CurrentDomain), rb => { });
            host.Should().BeAssignableTo<IWebJobsBuilder>();
        }

        [Fact]
        public void Creates_RocketHost_ForAssemblies()
        {
            var host = new WebJobsBuilder().UseRocketBooster(new Startup(), RocketBooster.For(new[] { typeof(RocketHostTests).Assembly }),
                rb => { });
            host.Should().BeAssignableTo<IWebJobsBuilder>();
        }
    }
}
