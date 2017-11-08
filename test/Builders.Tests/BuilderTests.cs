using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.Builders.Tests
{
    public class BuilderTests
    {
        class TestGenericValueContainer : Builder
        {
        }


        [Fact]
        public void ReturnsNullOfNoValue()
        {
            var container = new TestGenericValueContainer();

            container[typeof(string)].Should().BeNull();
        }


        [Fact]
        public void SetAValue()
        {
            var container = new TestGenericValueContainer();

            container[typeof(string)] = "abc";

            container[typeof(string)].Should().Be("abc");
        }
        [Fact]
        public void GetAStronglyTypedValue()
        {
            var container = new TestGenericValueContainer();
            container[typeof(string)] = "abc";
            container.Get<string>().Should().Be("abc");
        }

        [Fact]
        public void SetAStronglyTypedValue()
        {
            var container = new TestGenericValueContainer();
            container.Set("abc");
            container.Get<string>().Should().Be("abc");
        }

        [Fact]
        public void SetShouldChain()
        {
            var container = new TestGenericValueContainer();
            container.Set("abc").Should().Be(container);
        }

        class ChildBuilder : Builder<TestGenericValueContainer>
        {
            public ChildBuilder(TestGenericValueContainer parent) : base(parent)
            {
            }
        }

        [Fact]
        public void ShouldHaveAParent()
        {
            var parent = new TestGenericValueContainer();
            var builder = new ChildBuilder(parent);
            builder.Parent.Should().BeSameAs(parent);
        }

        [Fact]
        public void ShouldExitProperly()
        {
            var parent = new TestGenericValueContainer();
            var builder = new ChildBuilder(parent);
            builder.Exit().Should().BeSameAs(parent);
        }
    }
}
