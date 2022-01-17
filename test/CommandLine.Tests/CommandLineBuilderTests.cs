﻿using System.ComponentModel;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Spectre.Console.Cli;
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
    public void ShouldEnableHelpOnAllCommands()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider))
                     .ConfigureCommandLine(
                          (conventionContext, lineContext) =>
                          {
                              lineContext.AddBranch("remote", z => { z.AddCommand<Add>("add"); });
                              lineContext.AddBranch("fetch", configurator => { configurator.AddCommand<Origin>("origin"); });
                          }
                      );
        var response = App.Create(ConventionContext.From(builder));
        response.Run(new[] { "remote", "add", "-v" }).Should().Be(1);
    }

    [Fact]
    public void ExecuteWorks()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));
        builder.ConfigureCommandLine(
            (context, lineContext) => lineContext.AddDelegate<AppSettings>("test", (context, state) => (int)( state.LogLevel ?? LogLevel.Information ))
        );

        var response = App.Create(ConventionContext.From(builder));

        response.Run(new[] { "test" }).Should().Be((int)LogLevel.Information);
    }

    [Fact]
    public void SupportsAppllicationStateWithCustomDependencyInjection()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));

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
        var response = App.Create(ConventionContext.From(builder));

        var result = response.Run(new[] { "test", "--log", "error" });

        result.Should().Be(1000);
    }

    [Fact]
    public void SupportsInjection_Creating_On_Construction()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));

        builder.ConfigureCommandLine(
            (context, builder) => { builder.AddCommand<InjectionConstructor>("constructor"); }
        );

        var service = AutoFake.Resolve<IService>();
        A.CallTo(() => service.ReturnCode).Returns(1000);

        var response = App.Create(ConventionContext.From(builder));

        var result = response.Run(new[] { "constructor" });
        result.Should().Be(1000);
        A.CallTo(() => service.ReturnCode).MustHaveHappened(1, Times.Exactly);
    }

    [Fact]
    public void Sets_Values_In_Commands()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));

        builder.ConfigureCommandLine((context, builder) => builder.AddCommand<CommandWithValues>("cwv"));
        var response = App.Create(ConventionContext.From(builder));
        response.Run(
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
                "client1"
            }
        );
    }

    [Fact]
    public void Can_Add_A_Command_With_A_Name_Using_Context()
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));

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

        var response = App.Create(ConventionContext.From(builder));

        var result = response.Run(new[] { "si" });

        result.Should().Be(0);
    }

    private class Add : Command
    {
        public override int Execute(CommandContext context)
        {
            return 1;
        }
    }

    private class Origin : Command
    {
        public override int Execute(CommandContext context)
        {
            return 1;
        }
    }

//
    [Theory]
    [InlineData("-v", LogLevel.Debug)]
    [InlineData("-t", LogLevel.Trace)]
    public void ShouldAllVerbosity(string command, LogLevel level)
    {
        var builder = new ConventionContextBuilder(new Dictionary<object, object?>())
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));
        builder.ConfigureCommandLine(
            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state) => (int)( state.LogLevel ?? LogLevel.Information ))
        );
        var response = App.Create(ConventionContext.From(builder));

        var result = (LogLevel)response.Run(new[] { "test", command });
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
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));
        builder.ConfigureCommandLine(
            (context, builder) => builder.AddDelegate<AppSettings>("test", (c, state) => (int)( state.LogLevel ?? LogLevel.Information ))
        );

        var response = App.Create(ConventionContext.From(builder));

        var result = (LogLevel)response.Run(new[] { "test" }.Concat(command.Split(' ')));
        result.Should().Be(level);
    }

    private class SubCmd : Command
    {
        [UsedImplicitly]
        public override int Execute(CommandContext context)
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
                     .UseAssemblies(new TestAssemblyProvider().GetAssemblies())
                     .UseServiceProviderFactory(new TestServiceProviderFactory(ServiceProvider));
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

        var response = App.Create(ConventionContext.From(builder));
        var result = response.Run(command.Split(' '));
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

    private class CommandWithValues : Command
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

    private class ServiceInjection : Command
    {
        private readonly IService2 _service2;

        private ServiceInjection(IService2 service2)
        {
            _service2 = service2;
        }

        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            return _service2.SomeValue == "Service2" ? 0 : 1;
        }
    }

    private class ServiceInjection2 : Command
    {
        private readonly IService2 _service2;

        private ServiceInjection2(IService2 service2)
        {
            _service2 = service2;
        }

        [UsedImplicitly]
        public override int Execute(CommandContext context)
        {
            return _service2.SomeValue == "Service2" ? 0 : 1;
        }
    }

    public CommandLineBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }
}

internal class TestServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    private readonly IServiceProvider _serviceProvider;

    public TestServiceProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return _serviceProvider;
    }
}
