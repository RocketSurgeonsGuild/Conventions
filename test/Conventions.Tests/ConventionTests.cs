using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionTests(ITestOutputHelper outputHelper) : AutoFakeTest<LocalTestContext>(LocalTestContext.Create(outputHelper))
{
}
