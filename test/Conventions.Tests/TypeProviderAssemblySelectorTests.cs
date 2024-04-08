using FakeItEasy;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Tests;

public class TypeProviderAssemblySelectorTests
{
    private readonly TypeProviderAssemblySelector _typeSelector = new();

    [Fact]
    public async Task FromAssembly_ReturnsSameInstance()
    {
        var result = _typeSelector.FromAssembly();
        await Verify(_typeSelector);
    }

    [Fact]
    public async Task FromAssemblies_ReturnsSameInstance()
    {
        var result = _typeSelector.FromAssemblies();
        await Verify(_typeSelector);
    }

    [Fact]
    public async Task FromAssemblyDependenciesOfGeneric_ReturnsSameInstance()
    {
        var result = _typeSelector.FromAssemblyDependenciesOf<TypeProviderAssemblySelectorTests>();
        await Verify(_typeSelector);
    }

    [Fact]
    public async Task FromAssemblyDependenciesOfType_ReturnsSameInstance()
    {
        var result = _typeSelector.FromAssemblyDependenciesOf(typeof(TypeProviderAssemblySelectorTests));
        await Verify(_typeSelector);
    }

    [Fact]
    public async Task FromAssemblyOfGeneric_ReturnsSameInstance()
    {
        var result = _typeSelector.FromAssemblyOf<TypeProviderAssemblySelectorTests>();
        await Verify(_typeSelector);
    }

    [Fact]
    public async Task FromAssemblyOfType_ReturnsSameInstance()
    {
        var result = _typeSelector.FromAssemblyOf(typeof(TypeProviderAssemblySelectorTests));
        await Verify(_typeSelector);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetTypes_WithPublicOnly_ReturnsExpectedTypes(bool publicOnly)
    {
        _typeSelector.FromAssembly();
        await Verify(_typeSelector.GetTypes(publicOnly)).UseParameters(publicOnly);
    }

    [Fact]
    public async Task GetTypes_WithFilterAction_ReturnsExpectedTypes()
    {
        _typeSelector.FromAssembly();
        Action<ITypeFilter> action = filter => filter.StartsWith("I");

        var result = _typeSelector.GetTypes(action);
        await Verify(_typeSelector.GetTypes(action));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetTypes_WithPublicOnlyAndFilterAction_ReturnsExpectedTypes(bool publicOnly)
    {
        _typeSelector.FromAssembly();
        Action<ITypeFilter> action = filter => filter.EndsWith("Tests");

        await Verify(_typeSelector.GetTypes(publicOnly, action)).UseParameters(publicOnly);
    }
}
