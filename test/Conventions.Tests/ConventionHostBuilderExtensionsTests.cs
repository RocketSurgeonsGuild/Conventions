using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionHostBuilderExtensionsTests
    {
        public interface IMyType { }

        [Fact]
        public void Should_Get_Item_By_Type()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType = A.Fake<IMyType>();
            A.CallTo(() => context.ServiceProperties[typeof(IMyType)]).Returns(myType);

            context.Get<IMyType>().Should().BeSameAs(myType);
        }

        [Fact]
        public void Should_Get_Item_By_Name()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType = A.Fake<IMyType>();
            A.CallTo(() => context.ServiceProperties["value"]).Returns(myType);

            context.Get<IMyType>("value").Should().BeSameAs(myType);
        }

        [Fact]
        public void Should_Set_Item_By_Type()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType = A.Fake<IMyType>();

            context.Set(myType);

            A.CallToSet(() => context.ServiceProperties[typeof(IMyType)]).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Set_Item_By_Name()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType = A.Fake<IMyType>();

            context.Set("value", myType);

            A.CallToSet(() => context.ServiceProperties["value"]).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_Get_TestHost()
        {
            var context = A.Fake<IConventionHostBuilder>();
            A.CallTo(() => context.ServiceProperties[typeof(HostType)]).Returns(HostType.UnitTestHost);

            context.IsUnitTestHost().Should().BeTrue();
        }

        [Fact]
        public void Should_Not_TestHost()
        {
            var context = A.Fake<IConventionHostBuilder>();
            A.CallTo(() => context.ServiceProperties[typeof(HostType)]).Returns(HostType.Live);

            context.IsUnitTestHost().Should().BeFalse();
        }

        [Theory]
        [InlineData(HostType.UnitTestHost)]
        [InlineData(HostType.Live)]
        public void Should_Get_HostType(HostType hostType)
        {
            var context = A.Fake<IConventionHostBuilder>();
            A.CallTo(() => context.ServiceProperties[typeof(HostType)]).Returns(hostType);

            context.GetHostType().Should().Be(hostType);
        }

        [Fact]
        public void Should_Not_GetHostType()
        {
            var context = A.Fake<IConventionHostBuilder>();
            A.CallTo(() => context.ServiceProperties[typeof(HostType)]).Returns(null);

            context.GetHostType().Should().BeNull();
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Type_Get()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType1 = A.Fake<IMyType>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context.ServiceProperties[typeof(IMyType)]).Returns(myType1);

            context.GetOrAdd(() => myType2).Should().NotBeSameAs(myType2);
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Name_Get()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType1 = A.Fake<IMyType>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context.ServiceProperties["value"]).Returns(myType1);

            context.GetOrAdd("value", () => myType2).Should().NotBeSameAs(myType2);
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Type_Add()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType1 = A.Fake<IMyType>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context.ServiceProperties[typeof(IMyType)]).Returns(null);

            context.GetOrAdd(() => myType2).Should().BeSameAs(myType2);
            A.CallToSet(() => context.ServiceProperties[typeof(IMyType)]).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Should_GetOrAdd_Item_By_Name_Add()
        {
            var context = A.Fake<IConventionHostBuilder>();
            var myType1 = A.Fake<IMyType>();
            var myType2 = A.Fake<IMyType>();
            A.CallTo(() => context.ServiceProperties["value"]).Returns(null);

            context.GetOrAdd("value", () => myType2).Should().BeSameAs(myType2);
            A.CallToSet(() => context.ServiceProperties[typeof(IMyType)]).MustHaveHappenedOnceExactly();
        }
    }
}
