using FakeItEasy;
using FluentAssertions;
using Xunit;

#pragma warning disable CA1040
#pragma warning disable CA1034

namespace Rocket.Surgery.Conventions.Tests;

public class ConventionContextExtensionsTests
{
    [Fact]
    public async Task Should_Get_Item_By_Type()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType = A.Fake<IMyType>();
        context.Set(myType);

        context.Get<IMyType>().Should().BeSameAs(myType);
    }

    [Fact]
    public async Task Should_Get_Item_By_Name()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType = A.Fake<IMyType>();
        context.Set("value", myType);
        context.Get<IMyType>("value").Should().BeSameAs(myType);
    }

    [Fact]
    public async Task Should_Require_Item_By_Type()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType = A.Fake<IMyType>();
        context.Set(myType);

        context.Require<IMyType>().Should().BeSameAs(myType);
    }

    [Fact]
    public async Task Should_Require_Item_By_Name()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType = A.Fake<IMyType>();
        context.Set("value", myType);
        context.Require<IMyType>("value").Should().BeSameAs(myType);
    }

    [Fact]
    public async Task Should_Fail_To_Require_Item_By_Type()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType = A.Fake<IMyType>();
        Action a = () => context.Require<IMyType>();
        a.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public async Task Should_Fail_To_Require_Item_By_Name()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType = A.Fake<IMyType>();
        Action a = () => context.Require<IMyType>("value");
        a.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public async Task Should_Get_IsUnitTestHost()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        context.Set(HostType.UnitTest);

        context.IsUnitTestHost().Should().BeTrue();
    }

    [Fact]
    public async Task Should_Not_IsUnitTestHost()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        context.Set(HostType.Live);

        context.IsUnitTestHost().Should().BeFalse();
    }

    [Fact]
    public async Task Should_GetOrAdd_Item_By_Type_Get()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType1 = A.Fake<IMyType>();
        var myType2 = A.Fake<IMyType>();
        context.Set(myType1);
        context.GetOrAdd(() => myType2).Should().NotBeSameAs(myType2);
    }

    [Fact]
    public async Task Should_GetOrAdd_Item_By_Name_Get()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType1 = A.Fake<IMyType>();
        var myType2 = A.Fake<IMyType>();
        context.Set("value", myType1);
        context.GetOrAdd("value", () => myType2).Should().NotBeSameAs(myType2);
    }

    [Fact]
    public async Task Should_GetOrAdd_Item_By_Type_Add()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType2 = A.Fake<IMyType>();
        context.GetOrAdd(() => myType2).Should().BeSameAs(myType2);
    }

    [Fact]
    public async Task Should_GetOrAdd_Item_By_Name_Add()
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        var myType2 = A.Fake<IMyType>();
        context.GetOrAdd("value", () => myType2).Should().BeSameAs(myType2);
    }

    public interface IMyType;

    [Theory]
    [InlineData(HostType.Undefined)]
    [InlineData(HostType.Live)]
    [InlineData(HostType.UnitTest)]
    public async Task Should_Get_HostType(HostType hostType)
    {
        var context = await ConventionContext.FromAsync(ConventionContextBuilder.Create(_ => []));
        context.Set(hostType);
        context.GetHostType().Should().Be(hostType);
    }
}
