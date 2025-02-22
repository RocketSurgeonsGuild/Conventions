using FakeItEasy;
using Xunit;

#pragma warning disable CA1034
#pragma warning disable CA1040

namespace Rocket.Surgery.Conventions.Tests;

public class ServiceProviderDictionaryExtensionsTests
{
    [Fact]
    public void Should_Get_Item_By_Type()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType = A.Fake<IMyType>();
        context[typeof(IMyType)] = myType;

        context.Get<IMyType>().ShouldBeSameAs(myType);
    }

    [Fact]
    public void Should_Get_Item_By_Name()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType = A.Fake<IMyType>();
        context["value"] = myType;
        context.Get<IMyType>("value").ShouldBeSameAs(myType);
    }

    [Fact]
    public void Should_Require_Item_By_Type()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType = A.Fake<IMyType>();
        context[typeof(IMyType)] = myType;

        context.Require<IMyType>().ShouldBeSameAs(myType);
    }

    [Fact]
    public void Should_Require_Item_By_Name()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType = A.Fake<IMyType>();
        context["value"] = myType;

        context.Require<IMyType>("value").ShouldBeSameAs(myType);
    }

    [Fact]
    public void Should_Fail_To_Require_Item_By_Type()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        Action a = () => context.Require<IMyType>();
        a.ShouldThrow<KeyNotFoundException>();
    }

    [Fact]
    public void Should_Fail_To_Require_Item_By_Name()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        Action a = () => context.Require<IMyType>("value");
        a.ShouldThrow<KeyNotFoundException>();
    }

    [Fact]
    public void Should_Set_Item_By_Type()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType = A.Fake<IMyType>();

        context.Set(myType);
    }

    [Fact]
    public void Should_Set_Item_By_Name()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType = A.Fake<IMyType>();

        context.Set("value", myType);
    }

    [Fact]
    public void Should_GetOrAdd_Item_By_Type_Get()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType1 = A.Fake<IMyType>();
        var myType2 = A.Fake<IMyType>();
        context.GetOrAdd(() => myType2).ShouldBeSameAs(myType2);
    }

    [Fact]
    public void Should_GetOrAdd_Item_By_Name_Get()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType1 = A.Fake<IMyType>();
        var myType2 = A.Fake<IMyType>();
        context["value"] = myType1;
        context.GetOrAdd("value", () => myType2).ShouldNotBeSameAs(myType2);
    }

    [Fact]
    public void Should_GetOrAdd_Item_By_Type_Add()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType2 = A.Fake<IMyType>();
        context.GetOrAdd(() => myType2).ShouldBeSameAs(myType2);
    }

    [Fact]
    public void Should_GetOrAdd_Item_By_Name_Add()
    {
        IServiceProviderDictionary context = new ServiceProviderDictionary();
        var myType2 = A.Fake<IMyType>();
        context.GetOrAdd("value", () => myType2).ShouldBeSameAs(myType2);
    }

    public interface IMyType;
}
