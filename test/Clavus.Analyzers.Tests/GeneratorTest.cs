using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.Testing.SourceGenerators;

namespace Clavus.Analyzers.Tests;

public abstract class GeneratorTest() : LoggerTest<TestRecord>(TestRecord.Create())
{
    // Raw builder without the default Clavus configuration. Used to build the shared dependency
    // compilations (which supply their own configuration) and by configuration-override tests.
    protected GeneratorTestContextBuilder Builder { get; } = GeneratorTestContextBuilder
                                                                         .Create()
                                                                         .AddCommonReferences()
                                                                         .AddCommonGenerators()
                                                                         .AddClavusConfiguration("", "");

    protected GeneratorTestContextBuilder WithSharedDeps() => Builder.AddSharedDeps(TestContext.CancellationToken);

    protected GeneratorTestContextBuilder WithGenericSharedDeps() => Builder.AddSharedGenericDeps(TestContext.CancellationToken);

    protected GeneratorTestContextBuilder Configure(Func<GeneratorTestContextBuilder, GeneratorTestContextBuilder> builder) => builder(Builder);
}
