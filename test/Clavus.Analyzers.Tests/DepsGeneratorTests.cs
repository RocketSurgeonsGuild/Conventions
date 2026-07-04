namespace Clavus.Analyzers.Tests;

public class DepsGeneratorTests : GeneratorTest
{
    [Test]
    public async Task ShouldGenerateClass1() => await Verify(GenerationHelpers.Class1(Builder, TestContext.CancellationToken));

    [Test]
    public async Task ShouldGenerateClass2() => await Verify(GenerationHelpers.Class2(Builder, TestContext.CancellationToken));

    [Test]
    [DependsOn(nameof(ShouldGenerateClass1))]
    public async Task ShouldGenerateClass3()
    {
        var class1 = await GenerationHelpers.Class1(Builder, TestContext.CancellationToken);
        await Verify(GenerationHelpers.Class3(Builder, class1, TestContext.CancellationToken));
    }

    [Test]
    public async Task ShouldGenerateGenericClass1() => await Verify(GenerationHelpers.GenericClass1(Builder, TestContext.CancellationToken));

    [Test]
    public async Task ShouldGenerateGenericClass2() => await Verify(GenerationHelpers.GenericClass2(Builder, TestContext.CancellationToken));

    [Test]
    [DependsOn(nameof(ShouldGenerateGenericClass1))]
    public async Task ShouldGenerateGenericClass3()
    {
        var class1 = await GenerationHelpers.GenericClass1(Builder, TestContext.CancellationToken);
        await Verify(GenerationHelpers.GenericClass3(Builder, class1, TestContext.CancellationToken));
    }
}
