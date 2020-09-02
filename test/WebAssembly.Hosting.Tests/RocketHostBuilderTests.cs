using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.JSInterop;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Configuration;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;
using WebAssemblyHostEnvironment = Rocket.Surgery.WebAssembly.Hosting.Internals.WebAssemblyHostEnvironment;
using LoggingBuilder = Rocket.Surgery.WebAssembly.Hosting.Internals.LoggingBuilder;

namespace Rocket.Surgery.WebAssembly.Hosting.Tests
{
    public class RocketHostBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Should_Call_Through_To_Delegate_Methods()
        {
            AutoFake.Provide(Array.Empty<string>());
            LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .PrependDelegate(new Action(() => { }))
                       .AppendDelegate(new Action(() => { }))
                );
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void Should_Call_Through_To_Convention_Methods()
        {
            AutoFake.Provide(Array.Empty<string>());
            var convention = AutoFake.Resolve<IConvention>();
            LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .PrependConvention(convention)
                       .AppendConvention(convention)
                );
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IConvention>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendConvention(A<IConvention>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void Should_UseAppDomain()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseAppDomain(AppDomain.CurrentDomain)
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseAssemblies()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseRocketBooster()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .UseRocketBooster(RocketBooster.For(AppDomain.CurrentDomain));

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseDiagnosticLogging()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .UseRocketBooster(
                    RocketBooster.For(AppDomain.CurrentDomain),
                    x => x.UseDiagnosticLogging(c => c.AddSerilog())
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseDependencyContext()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        [Fact]
        public void Should_ConfigureServices()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureServices(x => { })
                );

            builder.Build();
            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(ServiceConventionDelegate))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureConfiguration()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureConfiguration(x => { })
                );

            builder.Build();
            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(ConfigConventionDelegate))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureHosting()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureHosting(x => { })
                );

            builder.Build();
            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(WebAssemblyHostingConventionDelegate))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public void Should_ConfigureLogging()
        {
            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScannerUnsafe(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureLogging(x => { })
                );

            builder.Build();
            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(LoggingConventionDelegate))
                )
            ).MustHaveHappened();
        }

        [Fact]
        public void Should_Build_The_Host_Correctly()
        {
            var serviceConventionFake = A.Fake<IServiceConvention>();
            var configurationConventionFake = A.Fake<IConfigConvention>();

            var builder = LocalWebAssemblyHostBuilder.CreateDefault()
               .ConfigureRocketSurgery(
                    rb => rb
                       .UseScannerUnsafe(
                            new BasicConventionScanner(
                                A.Fake<IServiceProviderDictionary>(),
                                serviceConventionFake,
                                configurationConventionFake
                            )
                        )
                       .UseAssemblyCandidateFinder(
                            new DefaultAssemblyCandidateFinder(new[] { typeof(RocketHostBuilderTests).Assembly })
                        )
                       .UseAssemblyProvider(
                            new DefaultAssemblyProvider(new[] { typeof(RocketHostBuilderTests).Assembly })
                        )
                );

            var host = builder.Build();
            host.Services.Should().NotBeNull();
        }

        public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }


    /// <summary>
    /// A builder for configuring and creating a <see cref="WebAssemblyHost"/>.
    /// </summary>
    class LocalWebAssemblyHostBuilder : IWebAssemblyHostBuilder
    {
        private Func<IServiceProvider> _createServiceProvider;

        /// <summary>
        /// Creates an instance of <see cref="WebAssemblyHostBuilder"/> using the most common
        /// conventions and settings.
        /// </summary>
        /// <param name="args">The argument passed to the application's main method.</param>
        /// <returns>A <see cref="WebAssemblyHostBuilder"/>.</returns>
        public static LocalWebAssemblyHostBuilder CreateDefault(string[] args = default)
        {
            // We don't use the args for anything right now, but we want to accept them
            // here so that it shows up this way in the project templates.
            args ??= Array.Empty<string>();
            var builder = new LocalWebAssemblyHostBuilder();

            // Right now we don't have conventions or behaviors that are specific to this method
            // however, making this the default for the template allows us to add things like that
            // in the future, while giving `new LocalWebAssemblyHostBuilder` as an opt-out of opinionated
            // settings.
            return builder;
        }

        /// <summary>
        /// Creates an instance of <see cref="LocalWebAssemblyHostBuilder"/> with the minimal configuration.
        /// </summary>
        internal LocalWebAssemblyHostBuilder()
        {
            // Private right now because we don't have much reason to expose it. This can be exposed
            // in the future if we want to give people a choice between CreateDefault and something
            // less opinionated.
            Configuration = new WebAssemblyHostConfiguration();
            RootComponents = new RootComponentMappingCollection();
            Services = new ServiceCollection();
            Logging = new LoggingBuilder(Services);

            var hostEnvironment = InitializeEnvironment();
            HostEnvironment = hostEnvironment;

            _createServiceProvider = () => { return Services.BuildServiceProvider(validateScopes: WebAssemblyHostEnvironmentExtensions.IsDevelopment(hostEnvironment)); };
        }

        private IWebAssemblyHostEnvironment InitializeEnvironment()
        {
            var hostEnvironment = new WebAssemblyHostEnvironment("applicationEnvironment", "http://localhost/");

            Services.AddSingleton<IWebAssemblyHostEnvironment>(hostEnvironment);

            return hostEnvironment;
        }

        /// <summary>
        /// Gets an <see cref="WebAssemblyHostConfiguration"/> that can be used to customize the application's
        /// configuration sources and read configuration attributes.
        /// </summary>
        public WebAssemblyHostConfiguration Configuration { get; }

        /// <summary>
        /// Gets the collection of root component mappings configured for the application.
        /// </summary>
        public RootComponentMappingCollection RootComponents { get; }

        /// <summary>
        /// Gets the service collection.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Gets information about the app's host environment.
        /// </summary>
        public IWebAssemblyHostEnvironment HostEnvironment { get; }

        /// <summary>
        /// Gets the logging builder for configuring logging services.
        /// </summary>
        public Microsoft.Extensions.Logging.ILoggingBuilder Logging { get; }

        /// <summary>
        /// Registers a <see cref="IServiceProviderFactory{TBuilder}" /> instance to be used to create the <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="factory">The <see cref="IServiceProviderFactory{TBuilder}" />.</param>
        /// <param name="configure">
        /// A delegate used to configure the <typeparamref T="TBuilder" />. This can be used to configure services using
        /// APIS specific to the <see cref="IServiceProviderFactory{TBuilder}" /> implementation.
        /// </param>
        /// <typeparam name="TBuilder">The type of builder provided by the <see cref="IServiceProviderFactory{TBuilder}" />.</typeparam>
        /// <remarks>
        /// <para>
        /// <see cref="ConfigureContainer{TBuilder}(IServiceProviderFactory{TBuilder}, Action{TBuilder})"/> is called by <see cref="Build"/>
        /// and so the delegate provided by <paramref name="configure"/> will run after all other services have been registered.
        /// </para>
        /// <para>
        /// Multiple calls to <see cref="ConfigureContainer{TBuilder}(IServiceProviderFactory{TBuilder}, Action{TBuilder})"/> will replace
        /// the previously stored <paramref name="factory"/> and <paramref name="configure"/> delegate.
        /// </para>
        /// </remarks>
        public void ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure = null)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _createServiceProvider = () =>
            {
                var container = factory.CreateBuilder(Services);
                configure?.Invoke(container);
                return factory.CreateServiceProvider(container);
            };
        }

        /// <summary>
        /// Builds a <see cref="WebAssemblyHost"/> instance based on the configuration of this builder.
        /// </summary>
        /// <returns>A <see cref="WebAssemblyHost"/> object.</returns>
        public WebAssemblyHost Build()
        {
            // Intentionally overwrite configuration with the one we're creating.
            Services.AddSingleton<IConfiguration>(Configuration);

            // A Blazor application always runs in a scope. Since we want to make it possible for the user
            // to configure services inside *that scope* inside their startup code, we create *both* the
            // service provider and the scope here.
            var services = _createServiceProvider();
            var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            return Activator.CreateInstance(
                typeof(WebAssemblyHost),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                null,
                new object[] { services, scope, Configuration, RootComponents.ToArray() },
                null
            ) as WebAssemblyHost;
        }
    }
}