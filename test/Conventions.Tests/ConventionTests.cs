using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionTests : AutoFakeTest
    {
        public ConventionTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}