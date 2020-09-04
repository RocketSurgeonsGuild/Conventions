using FakeItEasy;
using FluentAssertions;
using Xunit;

#pragma warning disable CA1034
#pragma warning disable CA1040

namespace Rocket.Surgery.Conventions.Tests
{
    public class ServiceProviderDictionaryExtensionsTests
    {
        [Fact]
        public void Should_Get_Item_By_Type()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType = A.Fake<IMyType>();
            A.CallTo(() => context[typeof(IMyType)]).Returns(myType);

            context.Get<IMyType>().Should().BeSameAs(myType);
        }

        [Fact]
        public void Should_Get_Item_By_Name()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType = A.Fake<IMyType>();
            A.CallTo(() => context["value"]).Returns(myType);

            context.Get<IMyType>("value").Should().BeSameAs(myType);
        }

        [Fact]
        public void Should_Set_Item_By_Type()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType = A.Fake<IMyType>();

            context.Set(myType);

            A.CallToSet(() => context[typeof(IMyType)]).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Set_Item_By_Name()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType = A.Fake<IMyType>();

            context.Set("value", myType);

            A.CallToSet(() => context["value"]).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Type_Get()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType1 = A.Fake<IMyType>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context[typeof(IMyType)]).Returns(myType1);

            context.GetOrAdd(() => myType2).Should().NotBeSameAs(myType2);
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Name_Get()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType1 = A.Fake<IMyType>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context["value"]).Returns(myType1);

            context.GetOrAdd("value", () => myType2).Should().NotBeSameAs(myType2);
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Type_Add()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context[typeof(IMyType)]).Returns(null);

            context.GetOrAdd(() => myType2).Should().BeSameAs(myType2);
            A.CallToSet(() => context[typeof(IMyType)]).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Name_Add()
        {
            var context = A.Fake<IServiceProviderDictionary>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context["value"]).Returns(null);

            context.GetOrAdd("value", () => myType2).Should().BeSameAs(myType2);
            A.CallToSet(() => context[typeof(IMyType)]).MustHaveHappenedOnceExactly();
        }

        public interface IMyType { }
    }
}