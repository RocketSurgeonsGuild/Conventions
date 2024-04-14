# Unit Testing with Conventions

Conventions offers some great magic but without being able to write unit tests, then the magic hurts more than it might help.

Unit Tests can be written many different ways.

## Use <xref:Rocket.Surgery.Conventions.ConventionContextBuilder> directly

You can make use of the <xref:Rocket.Surgery.Conventions.ConventionContextBuilder> by itself, and apply all the conventions. This requires more manual configuration but gives the greatest flexibility.

[!code-c#[Sample Test](../../sample/Sample.Core.Tests/SampleTests.cs?name=codeblock)]

## Use <xref:Rocket.Surgery.Hosting.TestHost>

The <xref:Rocket.Surgery.Hosting.TestHost> can be used to create a `IHostBuilder` configured for testing. This will automatically do the work for setting up Configuration, Services and Logging.

[!code-c#[Sample Test](../../sample/Sample.Tests/SampleTestHostTests.cs?name=codeblock)]

> [!TIP]
> There is <xref:Rocket.Surgery.WebAssembly.Hosting.TestWebAssemblyHost> for WebAssembly / Blazor apps

## Logging

Some unit testing frameworks have the ability to capture log output for a given test, this can be handy for helping identify any problems. If you provide a `ILogger` or a `ILoggerFactory` into the
<xref:Rocket.Surgery.Conventions.ConventionContextBuilder> or <xref:Rocket.Surgery.Conventions.IConventionContext> it will be used during the convention scanning process and spit out diagnostic logs that may be useful.

> [!TIP]
> In XUnit this is the `ITestOutputHelper`
