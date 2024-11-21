using System.Diagnostics;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Diagnostics;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class DiagnosticLoggerTests(ITestOutputHelper outputHelper) : AutoFakeTest<XUnitTestContext>(XUnitTestContext.Create(outputHelper))
{
    [Fact]
    public void LogTrace()
    {
        var logger = AutoFake.Resolve<DiagnosticLogger>();
        logger.LogTrace("Test!~");
        A
           .CallTo(
                () => AutoFake
                     .Resolve<DiagnosticSource>()
                     .Write(
                          "LogLevel.Trace",
                          A<object>.That.Matches(
                              x =>
                                  x.GetType().GetProperty("logLevel") != null
                               && x.GetType().GetProperty("eventId") != null
                               && x.GetType().GetProperty("state") != null
                               && x.GetType().GetProperty("exception") != null
                               && x.GetType().GetProperty("message") != null
                          )
                      )
            )
           .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LogDebug()
    {
        var logger = AutoFake.Resolve<DiagnosticLogger>();
        logger.LogDebug("Test!~");
        A
           .CallTo(
                () => AutoFake
                     .Resolve<DiagnosticSource>()
                     .Write(
                          "LogLevel.Debug",
                          A<object>.That.Matches(
                              x =>
                                  x.GetType().GetProperty("logLevel") != null
                               && x.GetType().GetProperty("eventId") != null
                               && x.GetType().GetProperty("state") != null
                               && x.GetType().GetProperty("exception") != null
                               && x.GetType().GetProperty("message") != null
                          )
                      )
            )
           .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LogInformation()
    {
        var logger = AutoFake.Resolve<DiagnosticLogger>();
        logger.LogInformation("Test!~");
        A
           .CallTo(
                () => AutoFake
                     .Resolve<DiagnosticSource>()
                     .Write(
                          "LogLevel.Information",
                          A<object>.That.Matches(
                              x =>
                                  x.GetType().GetProperty("logLevel") != null
                               && x.GetType().GetProperty("eventId") != null
                               && x.GetType().GetProperty("state") != null
                               && x.GetType().GetProperty("exception") != null
                               && x.GetType().GetProperty("message") != null
                          )
                      )
            )
           .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LogCritical()
    {
        var logger = AutoFake.Resolve<DiagnosticLogger>();
        logger.LogCritical("Test!~");
        A
           .CallTo(
                () => AutoFake
                     .Resolve<DiagnosticSource>()
                     .Write(
                          "LogLevel.Critical",
                          A<object>.That.Matches(
                              x =>
                                  x.GetType().GetProperty("logLevel") != null
                               && x.GetType().GetProperty("eventId") != null
                               && x.GetType().GetProperty("state") != null
                               && x.GetType().GetProperty("exception") != null
                               && x.GetType().GetProperty("message") != null
                          )
                      )
            )
           .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LogError()
    {
        var logger = AutoFake.Resolve<DiagnosticLogger>();
        logger.LogError("Test!~");
        A
           .CallTo(
                () => AutoFake
                     .Resolve<DiagnosticSource>()
                     .Write(
                          "LogLevel.Error",
                          A<object>.That.Matches(
                              x =>
                                  x.GetType().GetProperty("logLevel") != null
                               && x.GetType().GetProperty("eventId") != null
                               && x.GetType().GetProperty("state") != null
                               && x.GetType().GetProperty("exception") != null
                               && x.GetType().GetProperty("message") != null
                          )
                      )
            )
           .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LogWarning()
    {
        var logger = AutoFake.Resolve<DiagnosticLogger>();
        logger.LogWarning("Test!~");
        A
           .CallTo(
                () => AutoFake
                     .Resolve<DiagnosticSource>()
                     .Write(
                          "LogLevel.Warning",
                          A<object>.That.Matches(
                              x =>
                                  x.GetType().GetProperty("logLevel") != null
                               && x.GetType().GetProperty("eventId") != null
                               && x.GetType().GetProperty("state") != null
                               && x.GetType().GetProperty("exception") != null
                               && x.GetType().GetProperty("message") != null
                          )
                      )
            )
           .MustHaveHappenedOnceExactly();
    }
}
