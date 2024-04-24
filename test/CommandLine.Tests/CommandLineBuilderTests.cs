using System.ComponentModel;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.CommandLine;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Testing;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;
using Spectre.Console.Cli;
using Xunit.Abstractions;

#pragma warning disable CA1034
#pragma warning disable CA1062
#pragma warning disable CA1822
#pragma warning disable CA2000

namespace Rocket.Surgery.Extensions.CommandLine.Tests;

public interface IService
{
    int ReturnCode { get; }
}

public interface IService2
{
    string SomeValue { get; }
}

public class CommandLineBuilderTests : AutoFakeTest
{
    [Fact]
    public void Constructs()
    {
        var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
        var builder = AutoFake.Resolve<ConventionContextBuilder>().UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        var a = () => { builder.PrependConvention(A.Fake<ICommandLineConvention>()); };
        a.Should().NotThrow();
        a = () => { builder.AppendConvention(A.Fake<ICommandLineConvention>()); };
        a.Should().NotThrow();
        a = () => { builder.PrependDelegate(new ServiceConvention((context, configuration, services) => { })); };
        a.Should().NotThrow();
        a = () => { builder.AppendDelegate(new ServiceConvention((context, configuration, services) => { })); };
        a.Should().NotThrow();
    }

    [Fact]
    public async Task ShouldNotBeEnabledIfNoCommandsAreConfigured()
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        using var host = await Host
                              .CreateApplicationBuilder(new[] { "remote", "add", "-v", })
                              .ConfigureRocketSurgery(builder);
        await host.StartAsync();
        host.Services.GetService<ConsoleResult>().Should().BeNull();
    }

    [Fact]
    public async Task ExecuteWorks()
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, lineContext) => lineContext.AddDelegate<AppSettings>("test", (context, state) => (int)( state.LogLevel ?? LogLevel.Information ))
        );

        using var host = await Host
                              .CreateApplicationBuilder(new[] { "test", })
                              .ConfigureRocketSurgery(builder);

        ( await host.RunConsoleAppAsync() ).Should().Be((int)LogLevel.Information);
    }

    [Fact]
    public async Task SupportsApplicationStateWithCustomDependencyInjection()
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        var service = A.Fake<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);

        var serviceProvider = A.Fake<IServiceProvider>();

        A.CallTo(() => serviceProvider.GetService(A<Type>.Ignored)).Returns(null!);
        A.CallTo(() => serviceProvider.GetService(typeof(IService))).Returns(service).NumberOfTimes(2);
        builder.ConfigureCommandLine(
            (context, lineContext) =>
            {
                lineContext.AddDelegate<AppSettings>(
                    "test",
                    (context, state) =>
                    {
                        state.LogLevel.Should().Be(LogLevel.Error);
                        return 1000;
                    }
                );
            }
        );
        using var host = await Host
                              .CreateApplicationBuilder(new[] { "test", "--log", "error", })
                              .ConfigureRocketSurgery(builder);

        var result = await host.RunConsoleAppAsync();

        result.Should().Be(1000);
    }

    [Fact]
    public async Task SupportsInjection_Creating_On_Construction()
    {
        var service = AutoFake.Resolve<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .ConfigureServices(
                          z => { z.AddSingleton(service); }
                      );

        builder.ConfigureCommandLine((context, builder) => { builder.AddCommand<InjectionConstructor>("constructor"); });

        using var host = await Host.CreateApplicationBuilder(new[] { "constructor", }).ConfigureRocketSurgery(builder);

        var result = await host.RunConsoleAppAsync();
        result.Should().Be(1000);
        A.CallTo(() => service.ReturnCode).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public async Task Sets_Values_In_Commands()
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        builder.ConfigureCommandLine((context, builder) => builder.AddCommand<CommandWithValues>("cwv"));
        using var host = await Host
                              .CreateApplicationBuilder(
                                   new[]
                                   {
                                       "cwv",
                                       "--api-domain",
                                       "mydomain.com",
                                       "--origin",
                                       "origin1",
                                       "--origin",
                                       "origin2",
                                       "--client-name",
                                       "client1",
                                   }
                               )
                              .ConfigureRocketSurgery(builder);
        await host.RunAsync(
        );
    }

    [Fact]
    public async Task Can_Add_A_Command_With_A_Name_Using_Context()
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .ConfigureCommandLine(
                          (context, lineContext) => lineContext.AddDelegate<AppSettings>(
                              "test",
                              (context, state) => (int)( state.LogLevel ?? LogLevel.Information )
                          )
                      );

        using var host = await Host.CreateApplicationBuilder(new[] { "test", }).ConfigureRocketSurgery(builder);

        ( await host.RunConsoleAppAsync() ).Should().Be((int)LogLevel.Information);
    }

    [Fact]
    public async Task Should_Configure_Logging_Correctly()
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .ConfigureCommandLine((context, builder) => builder.AddCommand<LoggerInjection>("logger"));

        using var host = await Host
                              .CreateApplicationBuilder(new[] { "logger", })
                              .ConfigureRocketSurgery(builder);

        var result = await host.RunConsoleAppAsync();
        result.Should().Be(0);
    }

    public CommandLineBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

    private sealed class Add : Command
    {
        public override int Execute(CommandContext context)
        {
            return 1;
        }
    }

    private sealed class Origin : Command
    {
        public override int Execute(CommandContext context)
        {
            return 1;
        }
    }

//
    [Theory]
    [InlineData("--verbose", LogLevel.Debug)]
    [InlineData("--trace", LogLevel.Trace)]
    public async Task ShouldAllVerbosity(string command, LogLevel level)
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state) => (int)( state.LogLevel ?? LogLevel.Information ))
        );
        using var host = await Host
                              .CreateApplicationBuilder(new[] { "test", command, })
                              .ConfigureRocketSurgery(builder);

        var result = (LogLevel)await host.RunConsoleAppAsync();
        result.Should().Be(level);
    }

    [Theory]
    [InlineData("-l debug", LogLevel.Debug)]
    [InlineData("-l nonE", LogLevel.None)]
    [InlineData("-l Information", LogLevel.Information)]
    [InlineData("-l Error", LogLevel.Error)]
    [InlineData("-l WARNING", LogLevel.Warning)]
    [InlineData("-l critical", LogLevel.Critical)]
    public async Task ShouldAllowLogLevelIn(string command, LogLevel level)
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state) => (int)( state.LogLevel ?? LogLevel.Information ))
        );

        using var host = await Host
                              .CreateApplicationBuilder(new[] { "test", }.Concat(command.Split(' ')).ToArray())
                              .ConfigureRocketSurgery(builder);

        var result = (LogLevel)await host.RunConsoleAppAsync();
        result.Should().Be(level);
    }

    private sealed class SubCmd : Command
    {
        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            return -1;
        }
    }

    [Theory]
//    [InlineData("--version")]
    [InlineData("--help")]
    [InlineData("cmd1 --help")]
    [InlineData("cmd1 a --help")]
    [InlineData("cmd2 --help")]
    [InlineData("cmd2 a --help")]
    [InlineData("cmd3 --help")]
    [InlineData("cmd3 a --help")]
    [InlineData("cmd4 --help")]
    [InlineData("cmd4 a --help")]
    [InlineData("cmd5 --help")]
    [InlineData("cmd5 a --help")]
    public async Task StopsForHelp(string command)
    {
        var builder = ConventionContextBuilder
                     .Create()
                     .ForTesting(new TestAssemblyProvider().GetAssemblies(), LoggerFactory)
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, builder) =>
            {
                builder.AddBranch("cmd1", z => z.AddCommand<SubCmd>("a"));
                builder.AddBranch("cmd2", z => z.AddCommand<SubCmd>("a"));
                builder.AddBranch("cmd3", z => z.AddCommand<SubCmd>("a"));
                builder.AddBranch("cmd4", z => z.AddCommand<SubCmd>("a"));
                builder.AddBranch("cmd5", z => z.AddCommand<SubCmd>("a"));
            }
        );

        using var host = await Host
                              .CreateApplicationBuilder(command.Split(' ').ToArray())
                              .ConfigureRocketSurgery(builder);
        var result = await host.RunConsoleAppAsync();
        result.Should().BeGreaterOrEqualTo(0);
    }

    public class InjectionConstructor : AsyncCommand
    {
        private readonly IService _service;

        public InjectionConstructor(IService service)
        {
            _service = service;
        }

        [UsedImplicitly]
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            await Task.Yield();
            return _service.ReturnCode;
        }
    }

    private sealed class CommandWithValues : Command
    {
        [CommandOption("--api-domain")]
        [Description("The auth0 Domain")]
        [UsedImplicitly]
        public string? ApiDomain { get; }

        [CommandOption("--client-name")]
        [Description("The client name to create or update")]
        [UsedImplicitly]
        public string? ClientName { get; }

        [CommandOption("--origin")]
        [Description("The origins that are allowed to access the client")]
        [UsedImplicitly]
        public IEnumerable<string> Origins { get; } = Enumerable.Empty<string>();

        public override int Execute(CommandContext context)
        {
            ApiDomain.Should().Be("mydomain.com");
            ClientName.Should().Be("client1");
            Origins.Should().Contain("origin1");
            Origins.Should().Contain("origin2");
            return -1;
        }
    }

    public class ServiceInjection : Command
    {
        private readonly IService2 _service2;

        public ServiceInjection(IService2 service2)
        {
            _service2 = service2;
        }

        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            return _service2.SomeValue == "Service2" ? 0 : 1;
        }
    }

    public class LoggerInjection : Command
    {
        private readonly ILogger<LoggerInjection> _logger;

        public LoggerInjection(ILogger<LoggerInjection> logger)
        {
            _logger = logger;
        }

        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            _logger.LogInformation(nameof(LoggerInjection));
            return 0;
        }
    }

    public class ServiceInjection2 : Command
    {
        private readonly IService2 _service2;

        public ServiceInjection2(IService2 service2)
        {
            _service2 = service2;
        }

        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            return _service2.SomeValue == "Service2" ? 0 : 1;
        }
    }
}

internal sealed class TestServiceProviderFactory(IServiceProvider serviceProvider) : IServiceProviderFactory<IServiceCollection>
{
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return serviceProvider;
    }
}