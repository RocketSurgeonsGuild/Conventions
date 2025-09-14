using FakeItEasy;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration.Json;
using Rocket.Surgery.Conventions.Configuration.Yaml;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting.AspNetCore.Tests.Startups;

using Xunit.Abstractions;

namespace Rocket.Surgery.Hosting.AspNetCore.Tests;

public class RocketWebApplicationTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper))
{
    [Fact]
    public async Task Should_Start_Application()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        await using var host = await builder.ConfigureRocketSurgery();

        new SimpleStartup().Configure(host);
        await host.StartAsync();
        var server = host.GetTestServer();
        var response = await server
                            .CreateRequest("/")
                            .GetAsync();

        var content = await response.Content.ReadAsStringAsync();
        content.ShouldBe("SimpleStartup -> Configure");
        await host.StopAsync();
    }

    [Fact]
    public async Task Creates_RocketHost_WithConfiguration()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery();
        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        #if NET10_0_OR_GREATER
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(8);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #else
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(6);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #endif
    }

    [Fact]
    public async Task Should_Build_The_Host_Correctly()
    {
        var @delegate = A.Fake<Func<WebApplication, CancellationToken, ValueTask>>();
        var delegate2 = A.Fake<Func<IHost, CancellationToken, ValueTask>>();
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(z => z.OnHostCreated(@delegate).OnHostCreated(delegate2));

        A.CallTo(() => @delegate.Invoke(A<WebApplication>._, A<CancellationToken>._)).MustHaveHappened();
        A.CallTo(() => delegate2.Invoke(A<IHost>._, A<CancellationToken>._)).MustHaveHappened();
        host.Services.ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_ConfigureHosting()
    {
        var convention = A.Fake<HostApplicationConvention<IHostApplicationBuilder>>();
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(rb => rb.ConfigureApplication(convention));

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<IHostApplicationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Should_ConfigureHosting_HostApplication()
    {
        var convention = A.Fake<HostApplicationConvention<WebApplicationBuilder>>();
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(
                                         rb => rb
                                            .ConfigureApplication(convention)
                                     );

        A.CallTo(() => convention.Invoke(A<IConventionContext>._, A<WebApplicationBuilder>._)).MustHaveHappened();
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Json()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(z => z.ExceptConvention(typeof(YamlConvention)));
        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        #if NET10_0_OR_GREATER
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(8);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(0);
        #else
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(6);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(0);
        #endif
    }

    [Fact]
    public async Task Creates_RocketHost_WithModifiedConfiguration_Yaml()
    {
        await using var host = await WebApplication
                                    .CreateBuilder()
                                    .ConfigureRocketSurgery(z => z.ExceptConvention(typeof(JsonConvention)));

        var configuration = (IConfigurationRoot)host.Services.GetRequiredService<IConfiguration>();

        #if NET10_0_OR_GREATER
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(2);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #else
        configuration.Providers.OfType<JsonConfigurationProvider>().Count().ShouldBe(0);
        configuration.Providers.OfType<YamlConfigurationProvider>().Count().ShouldBe(12);
        #endif
    }
}
