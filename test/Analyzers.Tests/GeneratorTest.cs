using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public abstract class GeneratorTest(ITestOutputHelper outputHelper) : LoggerTest(outputHelper, LogLevel.Trace), IAsyncLifetime
{
    protected GeneratorTestContextBuilder Builder { get; private set; } = null!;

    protected GeneratorTestContextBuilder WithSharedDeps()
    {
        return Builder = Builder.AddSharedDeps();
    }

    protected GeneratorTestContextBuilder WithGenericSharedDeps()
    {
        return Builder = Builder.AddSharedGenericDeps();
    }

    protected GeneratorTestContextBuilder Configure(Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder> builder)
    {
        return Builder = builder(Builder);
    }

    public virtual Task InitializeAsync()
    {
        Builder = GeneratorTestContextBuilder
                 .Create()
                 .AddCommonReferences()
                 .AddCommonGenerators();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}