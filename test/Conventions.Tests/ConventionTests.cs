using Rocket.Surgery.Extensions.Testing;

// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionTests() : AutoFakeTest<TUnitTestRecord>(TUnitDefaults.CreateTestContext(TUnit.Core.TestContext.Current!)) { }
