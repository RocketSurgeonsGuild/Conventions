using FakeItEasy.Creation;
using Rocket.Surgery.Extensions.Testing;
using Serilog.Events;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public class LocalTestContext(ITestOutputHelper testOutputHelper, LogEventLevel logEventLevel = LogEventLevel.Verbose, string? outputTemplate = null)
    : XUnitTestContext(testOutputHelper, logEventLevel, outputTemplate), IAutoFakeTestContext
{
    public static LocalTestContext Create(ITestOutputHelper testOutputHelper, LogEventLevel logEventLevel = LogEventLevel.Verbose, string? outputTemplate = null) => new(testOutputHelper, logEventLevel, outputTemplate);

    public Action<IFakeOptions>? FakeOptionsAction { get; set; }
}
