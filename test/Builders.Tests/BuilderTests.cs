using System.Collections.Generic;
using FluentAssertions;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Builders.Tests
{
    public class BuilderTests : AutoTestBase
    {
        class TestGenericValueContainer : Builder
        {
            public TestGenericValueContainer() : base(new Dictionary<object, object>()) {}
        }


        public BuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

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

        class ChildBuilder : Builder<TestGenericValueContainer>
        {
            public ChildBuilder(TestGenericValueContainer parent) : base(parent, new Dictionary<object, object>())
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
