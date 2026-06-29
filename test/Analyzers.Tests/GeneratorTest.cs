using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Rocket.Surgery.Conventions.Analyzers.Tests;

public abstract class GeneratorTest() : LoggerTest<TestRecord>(TestRecord.Create())
{
    protected GeneratorTestContextBuilder Builder { get; } = GeneratorTestContextBuilder
                                                                         .Create()
                                                                         .AddCommonReferences()
                                                                         .AddCommonGenerators();

    protected GeneratorTestContextBuilder WithSharedDeps() => Builder.AddSharedDeps(TestContext.CancellationToken);

    protected GeneratorTestContextBuilder WithGenericSharedDeps() => Builder.AddSharedGenericDeps(TestContext.CancellationToken);

    protected GeneratorTestContextBuilder Configure(Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder> builder) => builder(Builder);
}
