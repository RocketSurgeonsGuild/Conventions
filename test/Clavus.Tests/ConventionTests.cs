using Rocket.Surgery.Extensions.Testing;


// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806

namespace Clavus.Tests;

public class ConventionTests() : AutoFakeTest<TestRecord>(TestRecord.Create()) { }
