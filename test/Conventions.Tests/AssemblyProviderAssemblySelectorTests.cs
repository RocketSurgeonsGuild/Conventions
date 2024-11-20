using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests;

public class AssemblyProviderAssemblySelectorTests(ITestOutputHelper outputHelper) : LoggerTest<XUnitTestContext>(XUnitDefaults.CreateTestContext(outputHelper))
{
    [Fact]
    public async Task FromAssembly_AddsCallingAssembly()
    {
        var selector = new AssemblyProviderAssemblySelector();
        selector.FromAssembly();
        await Verify(selector);
    }

    [Fact]
    public async Task FromAssemblies_SetsAllAssembliesToTrue()
    {
        var selector = new AssemblyProviderAssemblySelector();
        selector.FromAssemblies();
        await Verify(selector);
    }

    [Fact]
    public async Task FromAssemblyOf_AddsSpecifiedAssembly()
    {
        var selector = new AssemblyProviderAssemblySelector();
        selector.FromAssemblyOf<AssemblyProviderAssemblySelector>();
        await Verify(selector);
    }

    [Fact]
    public async Task FromAssemblyOf_Type_AddsSpecifiedAssembly()
    {
        var selector = new AssemblyProviderAssemblySelector();
        selector.FromAssemblyOf(typeof(AssemblyProviderAssemblySelector));
        await Verify(selector);
    }

    [Fact]
    public async Task FromAssemblyDependenciesOf_AddsSpecifiedAssembly()
    {
        var selector = new AssemblyProviderAssemblySelector();
        selector.FromAssemblyDependenciesOf<AssemblyProviderAssemblySelector>();
        await Verify(selector);
    }

    [Fact]
    public async Task FromAssemblyDependenciesOf_Type_AddsSpecifiedAssembly()
    {
        var selector = new AssemblyProviderAssemblySelector();
        selector.FromAssemblyDependenciesOf(typeof(AssemblyProviderAssemblySelector));
        await Verify(selector);
    }
}
