﻿using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
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

        Action a = () => { builder.PrependConvention(A.Fake<ICommandLineConvention>()); };
        a.Should().NotThrow();
        a = () => { builder.AppendConvention(A.Fake<ICommandLineConvention>()); };
        a.Should().NotThrow();
        a = () => { builder.PrependDelegate(new ServiceConvention((context, configuration, services) => { })); };
        a.Should().NotThrow();
        a = () => { builder.AppendDelegate(new ServiceConvention((context, configuration, services) => { })); };
        a.Should().NotThrow();
    }

    [Fact]
    public void ShouldEnableHelpOnAllCommands()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .ConfigureCommandLine(
                          (conventionContext, lineContext) =>
                          {
                              lineContext.AddCommand<Remote>();
                              lineContext.AddCommand<Fetch>();
                          }
                      );
        var response = ConventionContext.From(builder).CreateCommandLine();

        response.Application.OptionHelp.Should().NotBeNull();

        response.Execute(AutoFake.Resolve<IServiceProvider>(), "remote", "add", "-v").Should().Be(1);
        Logger.LogInformation(response.Application.Commands.Find(x => x.Name == "remote")!.GetHelpText());
        response.Application.Commands.Find(x => x.Name == "fetch")!.GetHelpText().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ShouldGetVersion()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        Action a = () => response.Application.ShowVersion();
        a.Should().NotThrow();
    }

    [Fact]
    public void ExecuteWorks()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, lineContext) => { lineContext.OnRun(state => Task.FromResult((int)( state.GetLogLevel() ?? LogLevel.Information ))); }
        );

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        response.Execute(AutoFake.Resolve<IServiceProvider>()).Should().Be((int)LogLevel.Information);
    }

    [Fact]
    public void RunWorks()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        builder.ConfigureCommandLine(
            (context, lineContext) => { lineContext.OnRun(state => (int)( state.GetLogLevel() ?? LogLevel.Information )); }
        );

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        response.Execute(AutoFake.Resolve<IServiceProvider>(), "run").Should().Be((int)LogLevel.Information);
    }

    [Fact]
    public void SupportsAppllicationStateWithCustomDependencyInjection()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        var service = A.Fake<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);

        var serviceProvider = A.Fake<IServiceProvider>();

        A.CallTo(() => serviceProvider.GetService(A<Type>.Ignored)).Returns(null!);
        A.CallTo(() => serviceProvider.GetService(typeof(IService))).Returns(service).NumberOfTimes(2);
        builder.ConfigureCommandLine(
            (context, lineContext) =>
            {
                lineContext.OnRun(
                    state =>
                    {
                        state.GetLogLevel().Should().Be(LogLevel.Error);
                        return 1000;
                    }
                );
            }
        );
        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = response.Execute(AutoFake.Resolve<IServiceProvider>(), "--log", "error");

        result.Should().Be(1000);
    }

    [Fact]
    public void SupportsInjection_Without_Creating_The_SubContainer()
    {
        var sp = AutoFake.Provide(A.Fake<IServiceProvider>());
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, builder) =>
            {
                builder.AddCommand<InjectionConstructor>("constructor");
                builder.AddCommand<InjectionExecute>("execute");
                builder.OnParse(state => { state.IsDefaultCommand.Should().BeFalse(); });
            }
        );

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        response.Parse("constructor");
        A.CallTo(() => sp.GetService(A<Type>._)).MustNotHaveHappened();
    }

    [Fact]
    public void SupportsInjection_Creating_On_Construction()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        builder.ConfigureCommandLine(
            (context, builder) =>
            {
                builder.AddCommand<InjectionConstructor>("constructor");
                builder.AddCommand<InjectionExecute>("execute");
                builder.OnParse(state => { state.IsDefaultCommand.Should().BeFalse(); });
            }
        );

        var service = AutoFake.Resolve<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = response.Execute(ServiceProvider, "constructor");
        result.Should().Be(1000);
        A.CallTo(() => service.ReturnCode).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public void SupportsInjection_Creating_On_Execute()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        builder.ConfigureCommandLine(
            (context, builder) =>
            {
                builder.AddCommand<InjectionConstructor>("constructor");
                builder.AddCommand<InjectionExecute>("execute");
            }
        );

        var service = AutoFake.Resolve<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = response.Execute(ServiceProvider, "constructor");
        result.Should().Be(1000);
        A.CallTo(() => service.ReturnCode).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public void Sets_Values_In_Commands()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        builder.ConfigureCommandLine((context, builder) => builder.AddCommand<CommandWithValues>("cwv"));
        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);
        response.Execute(
            ServiceProvider,
            "cwv",
            "--api-domain",
            "mydomain.com",
            "--origin",
            "origin1",
            "--origin",
            "origin2",
            "--client-name",
            "client1"
        );
    }

    [Fact]
    public void Can_Add_A_Command_Without_A_Name()
    {
        var service = A.Fake<IService2>();
        A.CallTo(() => service.SomeValue).Returns("Service2");
        AutoFake.Provide(service);

        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        builder.ConfigureCommandLine((context, builder) => builder.AddCommand<ServiceInjection>());

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = response.Execute(ServiceProvider, "serviceinjection");

        result.Should().Be(0);
    }

    [Fact]
    public void Can_Add_A_Command_Without_A_Name_Using_Context()
    {
        var service = A.Fake<IService2>();
        A.CallTo(() => service.SomeValue).Returns("Service2");
        AutoFake.Provide(service);

        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, builder) => { builder.AddCommand<ServiceInjection>(); }
        );

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = response.Execute(ServiceProvider, "serviceinjection");

        result.Should().Be(0);
    }

    [Fact]
    public void Can_Add_A_Command_With_A_Name_Using_Context()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        var service = A.Fake<IService2>();
        A.CallTo(() => service.SomeValue).Returns("Service2");
        AutoFake.Provide(service);

        builder.ConfigureCommandLine(
            (c, builder) =>
            {
                var context = builder;
                context.AddCommand<ServiceInjection2>("si");
            }
        );

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = response.Execute(ServiceProvider, "si");

        result.Should().Be(0);
    }

    [Fact]
    public void Should_Call_OnParse_Delegates()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        var context = builder as ICommandLineContext;

        var onParseBuilder = A.Fake<OnParseDelegate>();
        var onParseContext = A.Fake<OnParseDelegate>();

        builder.ConfigureCommandLine(
            (context, builder) =>
            {
                builder.OnParse(onParseBuilder);
                builder.OnParse(onParseContext);
            }
        );

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        response.Execute(ServiceProvider, "si");

        A.CallTo(() => onParseBuilder(A<IApplicationState>._)).MustHaveHappened(1, Times.Exactly);
        A.CallTo(() => onParseContext(A<IApplicationState>._)).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public void Should_Call_Default_Command()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        builder.ConfigureCommandLine((context, builder) => builder.OnRun<Default>());
        var result = ConventionContext.From(builder).CreateCommandLine().Execute(ServiceProvider);
        result.Should().Be(-1);
    }

    [Fact]
    public void Should_Call_Default_Command_Context()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        var context = builder as ICommandLineContext;

        builder.ConfigureCommandLine((context, builder) => builder.OnRun<Default>());
        var result = ConventionContext.From(builder).CreateCommandLine().Execute(ServiceProvider);
        result.Should().Be(-1);
    }

    public CommandLineBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Command("remote")]
    [Subcommand(typeof(Add))]
    private class Remote
    {
        [UsedImplicitly]
        public int OnExecute()
        {
            return 1;
        }
    }

    [Command("add")]
    private class Add
    {
        [UsedImplicitly]
        public int OnExecute()
        {
            return 1;
        }
    }

    [Command("fetch")]
    [Subcommand(typeof(Origin))]
    private class Fetch
    {
        [UsedImplicitly]
        public int OnExecute()
        {
            return 2;
        }
    }

    [Command("origin")]
    private class Origin
    {
        [UsedImplicitly]
        public int OnExecute()
        {
            return 2;
        }
    }

    [Theory]
    [InlineData("-v", LogLevel.Trace)]
    [InlineData("-t", LogLevel.Trace)]
    [InlineData("-d", LogLevel.Debug)]
    public void ShouldAllVerbosity(string command, LogLevel level)
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine((context, builder) => builder.OnRun(state => (int)( state.GetLogLevel() ?? LogLevel.Information )));
        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = (LogLevel)response.Execute(AutoFake.Resolve<IServiceProvider>(), command);
        result.Should().Be(level);
    }

    [Theory]
    [InlineData("-l debug", LogLevel.Debug)]
    [InlineData("-l nonE", LogLevel.None)]
    [InlineData("-l Information", LogLevel.Information)]
    [InlineData("-l Error", LogLevel.Error)]
    [InlineData("-l WARNING", LogLevel.Warning)]
    [InlineData("-l critical", LogLevel.Critical)]
    public void ShouldAllowLogLevelIn(string command, LogLevel level)
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine((context, builder) => builder.OnRun(state => (int)( state.GetLogLevel() ?? LogLevel.Information )));

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        var result = (LogLevel)response.Execute(AutoFake.Resolve<IServiceProvider>(), command.Split(' '));
        result.Should().Be(level);
    }

    [Theory]
    [InlineData("-l invalid")]
    [InlineData("-l ")]
    public void ShouldDisallowInvalidLogLevels(string command)
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);

        Action a = () => response.Execute(AutoFake.Resolve<IServiceProvider>(), command.Split(' '));
        a.Should().Throw<CommandParsingException>();
    }

    [Command]
    [Subcommand(typeof(SubCmd))]
    private class Cmd
    {
        [UsedImplicitly]
        public int OnExecute()
        {
            return -1;
        }
    }

    [Command("a")]
    private class SubCmd
    {
        [UsedImplicitly]
        public int OnExecute()
        {
            return -1;
        }
    }

    [Theory]
    [InlineData("--version")]
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
    public void StopsForHelp(string command)
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
           .UseAssemblies(new TestAssemblyProvider().GetAssemblies());
        builder.ConfigureCommandLine(
            (context, builder) =>
            {
                builder.AddCommand<Cmd>("cmd1");
                builder.AddCommand<Cmd>("cmd2");
                builder.AddCommand<Cmd>("cmd3");
                builder.AddCommand<Cmd>("cmd4");
                builder.AddCommand<Cmd>("cmd5");
            }
        );

        var response = ConventionContext.From(builder).CreateCommandLine(typeof(CommandLineBuilderTests).GetTypeInfo().Assembly);
        var result = response.Execute(AutoFake.Resolve<IServiceProvider>(), command.Split(' '));
        result.Should().BeGreaterOrEqualTo(0);
    }

    [Command]
    public class InjectionConstructor
    {
        private readonly IService _service;

        public InjectionConstructor(IService service)
        {
            _service = service;
        }

        [UsedImplicitly]
        public async Task<int> OnExecuteAsync()
        {
            await Task.Yield();
            return _service.ReturnCode;
        }
    }

    [Command]
    public class InjectionExecute
    {
        [UsedImplicitly]
        public async Task<int> OnExecuteAsync(IService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            await Task.Yield();
            return service.ReturnCode;
        }
    }

    [Command]
    private class CommandWithValues
    {
        [Option("--api-domain", CommandOptionType.SingleValue, Description = "The auth0 Domain")]
        [UsedImplicitly]
        public string? ApiDomain { get; }

        [Option(
            "--client-name",
            CommandOptionType.SingleValue,
            Description = "The client name to create or update"
        )]
        [UsedImplicitly]
        public string? ClientName { get; }

        [Option(
            "--origin",
            CommandOptionType.MultipleValue,
            Description =
                "The origins that are allowed to access the client"
        )]
        [UsedImplicitly]
        public IEnumerable<string> Origins { get; } = Enumerable.Empty<string>();

        [UsedImplicitly]
        public int OnExecute()
        {
            ApiDomain.Should().Be("mydomain.com");
            ClientName.Should().Be("client1");
            Origins.Should().Contain("origin1");
            Origins.Should().Contain("origin2");
            return -1;
        }
    }

    [Command("ServiceInjection")]
    private class ServiceInjection
    {
        private readonly IService2 _service2;

        private ServiceInjection(IService2 service2)
        {
            _service2 = service2;
        }

        [UsedImplicitly]
        public int OnExecute()
        {
            return _service2.SomeValue == "Service2" ? 0 : 1;
        }
    }

    [Command]
    private class ServiceInjection2
    {
        private readonly IService2 _service2;

        private ServiceInjection2(IService2 service2)
        {
            _service2 = service2;
        }

        [UsedImplicitly]
        public int OnExecute()
        {
            return _service2.SomeValue == "Service2" ? 0 : 1;
        }
    }

    private class Default : IDefaultCommand
    {
        public int Run(IApplicationState state)
        {
            return -1;
        }
    }
}
