using System;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests.Logging
{
    public class LoggingBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Constructs()
        {
            var assemblyProvider = AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var builder = AutoFake.Resolve<LoggingBuilder>();

            builder.AssemblyProvider.Should().BeSameAs(assemblyProvider);
            builder.AssemblyCandidateFinder.Should().NotBeNull();
            builder.Services.Should().NotBeNull();
            builder.Configuration.Should().NotBeNull();
            builder.Environment.Should().NotBeNull();
            Action a = () => { builder.PrependConvention(A.Fake<ILoggingConvention>()); };
            a.Should().NotThrow();
            a = () => { builder.PrependDelegate(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void BuildsALogger()
        {
            AutoFake.Provide<IAssemblyProvider>(new TestAssemblyProvider());
            var builder = AutoFake.Resolve<LoggingBuilder>();

            Action a = () => builder.Build();
            a.Should().NotThrow();
        }

        public LoggingBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}