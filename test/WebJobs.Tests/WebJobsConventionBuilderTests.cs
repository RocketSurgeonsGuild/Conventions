using System;
using System.Collections.Generic;
using System.Diagnostics;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.WebJobs.Tests;
using Xunit;
using Xunit.Abstractions;

[assembly: Convention(typeof(WebJobsConventionBuilderTests.AbcConvention))]

namespace Rocket.Surgery.Extensions.WebJobs.Tests
{
    public class WebJobsConventionBuilderTests : AutoTestBase
    {
        public WebJobsConventionBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AutoFake.Provide<DiagnosticSource>(new DiagnosticListener("Test"));
        }

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

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IEnumerable<IWebJobsConvention>>._)).MustHaveHappened();
        }

        public interface IAbc { }
        public interface IAbc2 { }
        public interface IAbc3 { }
        public interface IAbc4 { }

        public class AbcConvention : IWebJobsConvention
        {
            public void Register(IWebJobsConventionContext context)
            {
                context.AddExtension(typeof(IAbc));
                context.Services.AddSingleton(A.Fake<IAbc>());
                context.Services.AddSingleton(A.Fake<IAbc2>());
            }
        }
    }
}
