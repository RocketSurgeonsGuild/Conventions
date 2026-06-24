using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;
using Serilog.Events;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public abstract class GeneratorTest() : LoggerTest<TUnitTestRecord>(TUnitDefaults.CreateTestContext(TUnit.Core.TestContext.Current!, LogEventLevel.Verbose))
{
    protected GeneratorTestContextBuilder Builder { get; private set; } = null!;

    protected GeneratorTestContextBuilder WithSharedDeps() => Builder.AddSharedDeps();

    protected GeneratorTestContextBuilder WithGenericSharedDeps() => Builder.AddSharedGenericDeps();

    protected GeneratorTestContextBuilder Configure(Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder> builder) => builder(Builder);

    [Before(Test)]
    public virtual void InitializeAsync()
    {
        Builder = GeneratorTestContextBuilder
                 .Create()
                 .AddCommonReferences()
                 .AddCommonGenerators();
    }
}
