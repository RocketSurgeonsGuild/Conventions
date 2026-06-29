using System.ComponentModel;

using FakeItEasy;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Rocket.Surgery.CommandLine;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Spectre.Console.Cli;



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

[Timeout(10000)]
public class CommandLineBuilderTests() : AutoFakeTest<TestRecord>(TestRecord.Create())
{
    [Test]
    public void Constructs(CancellationToken cancellationToken)
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

    [Test]
    public async Task ShouldNotBeEnabledIfNoCommandsAreConfigured(CancellationToken cancellationToken)
    {
        using var host = await Host
                              .CreateApplicationBuilder(["remote", "add", "-v"])
                              .ConfigureRocketSurgery(b => b.UseLogger(Logger));
        await host.StartAsync(TestContext.CancellationToken);
        host.Services.GetService<ConsoleResult>().ShouldBeNull();
    }

    [Test]
    public async Task ExecuteWorks(CancellationToken cancellationToken)
    {
        using var host = await Host
                              .CreateApplicationBuilder(["test"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            async (context, lineContext, ct) => lineContext.AddDelegate<AppSettings>("test", (context, state, ct) => (int)( state.LogLevel ?? LogLevel.Information ))
                                        )
                               );

        ( await host.RunConsoleAppAsync(TestContext.CancellationToken) ).ShouldBe((int)LogLevel.Information);
    }

    [Test]
    public async Task SupportsApplicationStateWithCustomDependencyInjection(CancellationToken cancellationToken)
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
                                                    (context, state, ct) =>
                                                    {
                                                        state.LogLevel.ShouldBe(LogLevel.Error);
                                                        return 1000;
                                                    }
                                                );
                                            }
                                        )
                               );

        var result = await host.RunConsoleAppAsync(TestContext.CancellationToken);

        result.ShouldBe(1000);
    }

    [Test]
    public async Task SupportsInjection_Creating_On_Construction(CancellationToken cancellationToken)
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

        var result = await host.RunConsoleAppAsync(TestContext.CancellationToken);
        result.ShouldBe(1000);
        A.CallTo(() => service.ReturnCode).MustHaveHappened(1, Times.Exactly);
    }

    [Test]
    public async Task Sets_Values_In_Commands(CancellationToken cancellationToken)
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
        await host.RunAsync(TestContext.CancellationToken);
    }

    [Test]
    public async Task Can_Add_A_Command_With_A_Name_Using_Context(CancellationToken cancellationToken)
    {
        using var host = await Host
                              .CreateApplicationBuilder(["test"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, lineContext) => lineContext.AddDelegate<AppSettings>(
                                                "test",
                                                (context, state, ct) => (int)( state.LogLevel ?? LogLevel.Information )
                                            )
                                        )
                               );

        ( await host.RunConsoleAppAsync(TestContext.CancellationToken) ).ShouldBe((int)LogLevel.Information);
    }

    [Test]
    public async Task Should_Configure_Logging_Correctly(CancellationToken cancellationToken)
    {
        using var host = await Host
                              .CreateApplicationBuilder(["logger"])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine((context, builder) => builder.AddCommand<LoggerInjection>("logger"))
                               );

        var result = await host.RunConsoleAppAsync(TestContext.CancellationToken);
        result.ShouldBe(0);
    }

    //
    [Test]
    [Arguments("--verbose", LogLevel.Debug)]
    [Arguments("--trace", LogLevel.Trace)]
    public async Task ShouldAllVerbosity(string command, LogLevel level, CancellationToken cancellationToken)
    {
        using var host = await Host
                              .CreateApplicationBuilder(["test", command])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state, ct) => (int)( state.LogLevel ?? LogLevel.Information ))
                                        )
                               );

        var result = (LogLevel)await host.RunConsoleAppAsync(TestContext.CancellationToken);
        result.ShouldBe(level);
    }

    [Test]
    [Arguments("-l debug", LogLevel.Debug)]
    [Arguments("-l nonE", LogLevel.None)]
    [Arguments("-l Information", LogLevel.Information)]
    [Arguments("-l Error", LogLevel.Error)]
    [Arguments("-l WARNING", LogLevel.Warning)]
    [Arguments("-l critical", LogLevel.Critical)]
    public async Task ShouldAllowLogLevelIn(string command, LogLevel level, CancellationToken cancellationToken)
    {
        using var host = await Host
                              .CreateApplicationBuilder(["test", .. command.Split(' ')])
                              .ConfigureRocketSurgery(
                                   b => b
                                       .UseLogger(Logger)
                                       .ConfigureCommandLine(
                                            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state, ct) => (int)( state.LogLevel ?? LogLevel.Information ))
                                        )
                               );

        var result = (LogLevel)await host.RunConsoleAppAsync(TestContext.CancellationToken);
        result.ShouldBe(level);
    }

    [Test]
    //    [Arguments("--version")]
    [Arguments("--help")]
    [Arguments("cmd1 --help")]
    [Arguments("cmd1 a --help")]
    [Arguments("cmd2 --help")]
    [Arguments("cmd2 a --help")]
    [Arguments("cmd3 --help")]
    [Arguments("cmd3 a --help")]
    [Arguments("cmd4 --help")]
    [Arguments("cmd4 a --help")]
    [Arguments("cmd5 --help")]
    [Arguments("cmd5 a --help")]
    public async Task StopsForHelp(string command, CancellationToken cancellationToken)
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
        var result = await host.RunConsoleAppAsync(TestContext.CancellationToken);
        result.ShouldBeGreaterThanOrEqualTo(0);
    }

    private sealed class Add : Command
    {
        protected override int Execute(CommandContext context, CancellationToken token) => 1;
    }

    private sealed class Origin : Command
    {
        protected override int Execute(CommandContext context, CancellationToken token) => 1;
    }

    private sealed class SubCmd : Command
    {
        [UsedImplicitly]
        protected override int Execute(CommandContext context, CancellationToken token) => -1;
    }

    public class InjectionConstructor(IService service, ILogger<InjectionConstructor> logger) : AsyncCommand
    {
        [UsedImplicitly]
        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken token)
        {
            await Task.Yield();
            return _service.ReturnCode;
        }

        private readonly IService _service = service;
    }

    private sealed class CommandWithValues : Command
    {
        protected override int Execute(CommandContext context, CancellationToken token)
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
        protected override int Execute(CommandContext context, CancellationToken token)
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
        protected override int Execute(CommandContext context, CancellationToken token)
        {
            _logger.LogInformation(nameof(LoggerInjection));
            return 0;
        }

        private readonly ILogger<LoggerInjection> _logger = logger;
    }

    public class ServiceInjection2(IService2 service2) : Command
    {
        [UsedImplicitly]
        protected override int Execute(CommandContext context, CancellationToken token) => _service2.SomeValue == "Service2" ? 0 : 1;

        private readonly IService2 _service2 = service2;
    }
}
