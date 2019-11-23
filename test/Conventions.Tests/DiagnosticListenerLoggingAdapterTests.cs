#if !NETCOREAPP3_0
using System;
using System.Diagnostics;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;
#pragma warning disable CA2000

namespace Rocket.Surgery.Conventions.Tests
{
    public class DiagnosticListenerLoggingAdapterTests : AutoFakeTest
    {
        [Fact]
        public void LogDebug()
        {
            var adapter = AutoFake.Resolve<DiagnosticListenerLoggingAdapter>();
            var source = new DiagnosticListener("Test");
            using (source.SubscribeWithAdapter(adapter))
            {
                var logger = new DiagnosticLogger(source);

                logger.LogDebug("test");

                A.CallTo(
                        () => Logger.Log(
                            LogLevel.Debug,
                            A<EventId>._,
                            A<object>._,
                            A<Exception>._,
                            A<Func<object, Exception, string>>._
                        )
                    )
                   .MustHaveHappened();
            }
        }

        [Fact]
        public void LogTrace()
        {
            var adapter = AutoFake.Resolve<DiagnosticListenerLoggingAdapter>();
            var source = new DiagnosticListener("Test");
            using (source.SubscribeWithAdapter(adapter))
            {
                var logger = new DiagnosticLogger(source);

                logger.LogTrace("test");

                A.CallTo(
                        () => Logger.Log(
                            LogLevel.Trace,
                            A<EventId>._,
                            A<object>._,
                            A<Exception>._,
                            A<Func<object, Exception, string>>._
                        )
                    )
                   .MustHaveHappened();
            }
        }

        [Fact]
        public void LogInformation()
        {
            var adapter = AutoFake.Resolve<DiagnosticListenerLoggingAdapter>();
            var source = new DiagnosticListener("Test");
            using (source.SubscribeWithAdapter(adapter))
            {
                var logger = new DiagnosticLogger(source);

                logger.LogInformation("test");

                A.CallTo(
                        () => Logger.Log(
                            LogLevel.Information,
                            A<EventId>._,
                            A<object>._,
                            A<Exception>._,
                            A<Func<object, Exception, string>>._
                        )
                    )
                   .MustHaveHappened();
            }
        }

        [Fact]
        public void LogCritical()
        {
            var adapter = AutoFake.Resolve<DiagnosticListenerLoggingAdapter>();
            var source = new DiagnosticListener("Test");
            using (source.SubscribeWithAdapter(adapter))
            {
                var logger = new DiagnosticLogger(source);

                logger.LogCritical("test");

                A.CallTo(
                        () => Logger.Log(
                            LogLevel.Critical,
                            A<EventId>._,
                            A<object>._,
                            A<Exception>._,
                            A<Func<object, Exception, string>>._
                        )
                    )
                   .MustHaveHappened();
            }
        }

        [Fact]
        public void LogError()
        {
            var adapter = AutoFake.Resolve<DiagnosticListenerLoggingAdapter>();
            var source = new DiagnosticListener("Test");
            using (source.SubscribeWithAdapter(adapter))
            {
                var logger = new DiagnosticLogger(source);

                logger.LogError("test");

                A.CallTo(
                        () => Logger.Log(
                            LogLevel.Error,
                            A<EventId>._,
                            A<object>._,
                            A<Exception>._,
                            A<Func<object, Exception, string>>._
                        )
                    )
                   .MustHaveHappened();
            }
        }

        [Fact]
        public void LogWarning()
        {
            var adapter = AutoFake.Resolve<DiagnosticListenerLoggingAdapter>();
            var source = new DiagnosticListener("Test");
            using (source.SubscribeWithAdapter(adapter))
            {
                var logger = new DiagnosticLogger(source);

                logger.LogWarning("test");

                A.CallTo(
                        () => Logger.Log(
                            LogLevel.Warning,
                            A<EventId>._,
                            A<object>._,
                            A<Exception>._,
                            A<Func<object, Exception, string>>._
                        )
                    )
                   .MustHaveHappened();
            }
        }

        public DiagnosticListenerLoggingAdapterTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}
#endif