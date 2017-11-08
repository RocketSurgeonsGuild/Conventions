using FluentAssertions;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionContextTests
    {
        class TestGenericValueContainer : ConventionContext
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
    }
}
