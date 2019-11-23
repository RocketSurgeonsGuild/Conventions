using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.WebJobs.Tests;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CA1034
#pragma warning disable CA1040
#pragma warning disable CA2000

[assembly: Convention(typeof(WebJobsConventionBuilderTests.AbcConvention))]

namespace Rocket.Surgery.Extensions.WebJobs.Tests
{
    public class WebJobsConventionBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Constructs()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            A.CallTo(() => AutoFake.Resolve<IWebJobsBuilder>().Services).Returns(services);
            var webJobsConventionBuilder = AutoFake.Resolve<WebJobsConventionBuilder>();

            webJobsConventionBuilder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            webJobsConventionBuilder.AssemblyCandidateFinder.Should().NotBeNull();
            webJobsConventionBuilder.Services.Should().BeSameAs(services);
            webJobsConventionBuilder.Configuration.Should().NotBeNull();
            webJobsConventionBuilder.Environment.Should().NotBeNull();

            Action a = () => { webJobsConventionBuilder.PrependConvention(A.Fake<IWebJobsConvention>()); };
            a.Should().NotThrow();
            a = () => { webJobsConventionBuilder.PrependDelegate(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void StoresAndReturnsItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var webJobsConventionBuilder = AutoFake.Resolve<WebJobsConventionBuilder>();

            var value = new object();
            webJobsConventionBuilder[string.Empty] = value;
            webJobsConventionBuilder[string.Empty].Should().BeSameAs(value);
        }

        [Fact]
        public void IgnoreNonExistentItems()
        {
            AutoFake.Provide<IDictionary<object, object>>(new Dictionary<object, object>());
            var webJobsConventionBuilder = AutoFake.Resolve<WebJobsConventionBuilder>();

            webJobsConventionBuilder[string.Empty].Should().BeNull();
        }

        [Fact]
        public void AddConventions()
        {
            var webJobsConventionBuilder = AutoFake.Resolve<WebJobsConventionBuilder>();

            var convention = A.Fake<IWebJobsConvention>();

            webJobsConventionBuilder.PrependConvention(convention);

            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IEnumerable<IWebJobsConvention>>._)
            ).MustHaveHappened();
        }

        [Fact]
        public void Removes_Hosted_Services_Since_They_Cannot_Be_Added_for_functions()
        {
            var services = AutoFake.Provide<IServiceCollection>(new ServiceCollection());
            A.CallTo(() => AutoFake.Resolve<IWebJobsBuilder>().Services).Returns(services);
            services.AddHostedService<Abc>();
            var webJobsConventionBuilder = AutoFake.Resolve<WebJobsConventionBuilder>();

            var convention = A.Fake<IWebJobsConvention>();

            webJobsConventionBuilder.PrependConvention(convention);

            webJobsConventionBuilder.Build();
            webJobsConventionBuilder.Services.Should().Contain(
                x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(Abc)
            );
            webJobsConventionBuilder.Services.Should().NotContain(
                x => x.ServiceType == typeof(IHostedService) && x.ImplementationType == typeof(Abc2)
            );
        }

        public WebJobsConventionBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
            => AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));

        public interface IAbc { }

        public class Abc : IHostedService
        {
            public Task StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

            public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        public interface IAbc2 : IHostedService { }

        public class Abc2 : IAbc2
        {
            public Task StartAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

            public Task StopAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        public interface IAbc3 { }

        public interface IAbc4 { }

        public class AbcConvention : IWebJobsConvention
        {
            public void Register(IWebJobsConventionContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                context.AddExtension(typeof(IAbc));
                context.Services.AddSingleton(A.Fake<IAbc>());
                context.Services.AddSingleton(A.Fake<IAbc2>());
                context.Services.AddHostedService<Abc2>();
            }
        }
    }
}