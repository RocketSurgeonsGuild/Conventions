using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions.DependencyInjection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionTests : AutoFakeTest
    {
        [Fact]
        public void ConventionAttributeThrowsIfNonConventionGiven()
        {
            Action a = () => new ConventionAttribute(typeof(object));
            a.Should().Throw<NotSupportedException>();
        }

        public ConventionTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}