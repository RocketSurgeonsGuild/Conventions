using System.ComponentModel;

using FakeItEasy;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Rocket.Surgery.CommandLine;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Hosting;

using Spectre.Console.Cli;

using Xunit.Abstractions;

#pragma warning disable CA1034, CA1062, CA1822, CA2000

namespace Rocket.Surgery.Extensions.CommandLine.Tests;

public interface IService
{
    int ReturnCode { get; }
}

public interface IService2
{
    string SomeValue { get; }
}

public class CommandLineBuilderTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper))
{
    [Fact]
    public void Constructs()
    {
        var builder = ConventionContextBuilder.Create(_ => []);

        var a = () => builder.PrependConvention(A.Fake<ICommandLineConvention>());
        a.ShouldNotThrow();
        a = () => builder.AppendConvention(A.Fake<ICommandLineConvention>());
        a.ShouldNotThrow();
        a = () => builder.PrependDelegate(new ServiceConvention((context, configuration, services) => { }), null, null);
        a.ShouldNotThrow();
        a = () => builder.AppendDelegate(new ServiceConvention((context, configuration, services) => { }), null, null);
        a.ShouldNotThrow();
    }

    [Fact]
    public async Task ShouldNotBeEnabledIfNoCommandsAreConfigured()
    {
        using var host = await Host
                              .CreateApplicationBuilder(["remote", "add", "-v"])
                              .ConfigureRocketSurgery(b => b.UseLogger(Logger));
        await host.StartAsync();
        host.Services.GetService<ConsoleResult>().ShouldBeNull();
    }

    [Fact]
    public async Task ExecuteWorks()
    {
        using var host = await Host
                              .CreateApplicationBuilder(["test"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, lineContext) => lineContext.AddDelegate<AppSettings>("test", (context, state) => (int)( state.LogLevel ?? LogLevel.Information ))
                                        )
                               );

        ( await host.RunConsoleAppAsync() ).ShouldBe((int)LogLevel.Information);
    }

    [Fact]
    public async Task SupportsApplicationStateWithCustomDependencyInjection()
    {
        var service = A.Fake<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);

        var serviceProvider = A.Fake<IServiceProvider>();

        A.CallTo(() => serviceProvider.GetService(A<Type>.Ignored)).Returns(null);
        A.CallTo(() => serviceProvider.GetService(typeof(IService))).Returns(service).NumberOfTimes(2);
        using var host = await Host
                              .CreateApplicationBuilder(["test", "--log", "error"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, lineContext) =>
                                            {
                                                lineContext.AddDelegate<AppSettings>(
                                                    "test",
                                                    (context, state) =>
                                                    {
                                                        state.LogLevel.ShouldBe(LogLevel.Error);
                                                        return 1000;
                                                    }
                                                );
                                            }
                                        )
                               );

        var result = await host.RunConsoleAppAsync();

        result.ShouldBe(1000);
    }

    [Fact]
    public async Task SupportsInjection_Creating_On_Construction()
    {
        var service = AutoFake.Resolve<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);

        using var host = await Host
                              .CreateApplicationBuilder(["constructor"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureServices(
                                            z => z.AddSingleton(service)
                                        )
                                       .ConfigureCommandLine((context, builder) => builder.AddCommand<InjectionConstructor>("constructor"))
                               );

        var result = await host.RunConsoleAppAsync();
        result.ShouldBe(1000);
        A.CallTo(() => service.ReturnCode).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public async Task Sets_Values_In_Commands()
    {
        using var host = await Host
                              .CreateApplicationBuilder(
                                   [
                                       "cwv",
                                       "--api-domain",
                                       "mydomain.com",
                                       "--origin",
                                       "origin1",
                                       "--origin",
                                       "origin2",
                                       "--client-name",
                                       "client1",
                                   ]
                               )
                              .ConfigureRocketSurgery(b => b.UseLogger(Logger).ConfigureCommandLine((context, builder) => builder.AddCommand<CommandWithValues>("cwv")));
        await host.RunAsync(
        );
    }

    [Fact]
    public async Task Can_Add_A_Command_With_A_Name_Using_Context()
    {
        using var host = await Host
                              .CreateApplicationBuilder(["test"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, lineContext) => lineContext.AddDelegate<AppSettings>(
                                                "test",
                                                (context, state) => (int)( state.LogLevel ?? LogLevel.Information )
                                            )
                                        )
                               );

        ( await host.RunConsoleAppAsync() ).ShouldBe((int)LogLevel.Information);
    }

    [Fact]
    public async Task Should_Configure_Logging_Correctly()
    {
        using var host = await Host
                              .CreateApplicationBuilder(["logger"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine((context, builder) => builder.AddCommand<LoggerInjection>("logger"))
                               );

        var result = await host.RunConsoleAppAsync();
        result.ShouldBe(0);
    }

    //
    [Theory]
    [InlineData("--verbose", LogLevel.Debug)]
    [InlineData("--trace", LogLevel.Trace)]
    public async Task ShouldAllVerbosity(string command, LogLevel level)
    {
        using var host = await Host
                              .CreateApplicationBuilder(["test", command])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state) => (int)( state.LogLevel ?? LogLevel.Information ))
                                        )
                               );

        var result = (LogLevel)await host.RunConsoleAppAsync();
        result.ShouldBe(level);
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
        using var host = await Host
                              .CreateApplicationBuilder(["test", .. command.Split(' ')])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state) => (int)( state.LogLevel ?? LogLevel.Information ))
                                        )
                               );

        var result = (LogLevel)await host.RunConsoleAppAsync();
        result.ShouldBe(level);
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
        using var host = await Host
                              .CreateApplicationBuilder([.. command.Split(' ')])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, builder) =>
                                            {
                                                builder.AddBranch("cmd1", z => z.AddCommand<SubCmd>("a"));
                                                builder.AddBranch("cmd2", z => z.AddCommand<SubCmd>("a"));
                                                builder.AddBranch("cmd3", z => z.AddCommand<SubCmd>("a"));
                                                builder.AddBranch("cmd4", z => z.AddCommand<SubCmd>("a"));
                                                builder.AddBranch("cmd5", z => z.AddCommand<SubCmd>("a"));
                                            }
                                        )
                               );
        var result = await host.RunConsoleAppAsync();
        result.ShouldBeGreaterThanOrEqualTo(0);
    }

    private sealed class Add : Command
    {
        public override int Execute(CommandContext context) => 1;
    }

    private sealed class Origin : Command
    {
        public override int Execute(CommandContext context) => 1;
    }

    private sealed class SubCmd : Command
    {
        [UsedImplicitly]
        public override int Execute(CommandContext context) => -1;
    }

    public class InjectionConstructor(IService service, ILogger<InjectionConstructor> logger) : AsyncCommand
    {
        [UsedImplicitly]
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            await Task.Yield();
            return _service.ReturnCode;
        }

        private readonly IService _service = service;
    }

    private sealed class CommandWithValues : Command
    {
        public override int Execute(CommandContext context)
        {
            ApiDomain.ShouldBe("mydomain.com");
            ClientName.ShouldBe("client1");
            Origins.ShouldContain("origin1");
            Origins.ShouldContain("origin2");
            return -1;
        }

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
        public IEnumerable<string> Origins { get; } = [];
    }

    public class ServiceInjection(IService2 service2, ILogger<ServiceInjection> logger) : Command
    {
        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            _logger.LogInformation(nameof(ServiceInjection));
            return _service2.SomeValue == "Service2" ? 0 : 1;
        }

        private readonly ILogger<ServiceInjection> _logger = logger;
        private readonly IService2 _service2 = service2;
    }

    public class LoggerInjection(ILogger<LoggerInjection> logger) : Command
    {
        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            _logger.LogInformation(nameof(LoggerInjection));
            return 0;
        }

        private readonly ILogger<LoggerInjection> _logger = logger;
    }

    public class ServiceInjection2(IService2 service2) : Command
    {
        [UsedImplicitly]
        public override int Execute(CommandContext context) => _service2.SomeValue == "Service2" ? 0 : 1;

        private readonly IService2 _service2 = service2;
    }
}
