using System.ComponentModel;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Tests;

public class TypeFilterFunctionalityTests
{
    [Fact]
    public async Task AssignableTo_Type_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.AssignableTo(typeof(string));
        var types = new[] { typeof(string), typeof(int), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task AssignableTo_Generic_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.AssignableTo<string>();
        var types = new[] { typeof(string), typeof(int), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task AssignableToAny_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.AssignableToAny(typeof(string), typeof(int));
        var types = new[] { typeof(string), typeof(int), typeof(double), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task EndsWith_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.EndsWith("Class");
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurInterface), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task StartsWith_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.StartsWith("My");
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(MyInterface), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task Contains_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.Contains("Class");
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(ClassInterface), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task InExactNamespaceOf_Type_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.InExactNamespaceOf(typeof(MyClass));
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task InExactNamespaceOf_Generic_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.InExactNamespaceOf<MyClass>();
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task InExactNamespaces_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.InExactNamespaces("MyNamespace", "YourNamespace");
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task InNamespaceOf_Type_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.InNamespaceOf(typeof(MyClass));
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task InNamespaceOf_Generic_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.InNamespaceOf<MyClass>();
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task InNamespaces_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.InNamespaces("MyNamespace", "YourNamespace");
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task NotInNamespaceOf_Type_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.NotInNamespaceOf(typeof(MyClass));
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task NotInNamespaceOf_Generic_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.NotInNamespaceOf<MyClass>();
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task NotInNamespaces_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.NotInNamespaces("MyNamespace", "YourNamespace");
        var types = new[] { typeof(MyClass), typeof(YourClass), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task WithAttribute_Type_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.WithAttribute(typeof(EditorBrowsableAttribute));
        var types = new[] { typeof(MyClassWithAttribute), typeof(YourClassWithAttribute), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task WithAttribute_Generic_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.WithAttribute<EditorBrowsableAttribute>();
        var types = new[] { typeof(MyClassWithAttribute), typeof(YourClassWithAttribute), typeof(OurClass), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task WithoutAttribute_Type_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.WithoutAttribute(typeof(EditorBrowsableAttribute));
        var types = new[] { typeof(MyClass), typeof(YourClassWithAttribute), typeof(OurClassWithoutAttribute), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    [Fact]
    public async Task WithoutAttribute_Generic_FiltersCorrectly()
    {
        var typeFilter = new TypeFilter();
        typeFilter.WithoutAttribute<EditorBrowsableAttribute>();
        var types = new[] { typeof(MyClass), typeof(YourClassWithAttribute), typeof(OurClassWithoutAttribute), };
        var filteredTypes = types.Where(t => typeFilter.Filters.All(f => f(t))).ToArray();
        await Verify(filteredTypes);
    }

    private class MyClass { }

    private class YourClass { }

    private class OurClass { }

    private class ClassMy { }

    private class ClassYour { }

    private class InterfaceOur { }

    private class OurInterface { }

    private class MyInterface { }

    private class ClassInterface { }

    [EditorBrowsable]
    private class MyClassWithAttribute { }

    [EditorBrowsable]
    private class YourClassWithAttribute { }

    private class OurClassWithoutAttribute { }
}
