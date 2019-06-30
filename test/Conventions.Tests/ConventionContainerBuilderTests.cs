using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionContainerBuilderTests : AutoTestBase
    {
        public ConventionContainerBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        private class CCBuilder : ConventionContainerBuilder<CCBuilder, IServiceConvention, ServiceConventionDelegate>
        {
            public CCBuilder(IConventionScanner scanner, IDictionary<object, object> properties) : base(scanner, properties)
            {
            }
        }

        class C : IServiceConvention
        {
            public void Register(IServiceConventionContext context)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void ShouldConstruct()
        {
            var properties = new Dictionary<object, object>();
            AutoFake.Provide<IDictionary<object, object>>(properties);
            var builder = AutoFake.Resolve<CCBuilder>();

            builder.Should().NotBeNull();
            builder.Properties.Should().NotBeNull();
            builder.Scanner.Should().NotBeNull();

            builder["a"] = "b";

            builder["a"].Should().Be("b");
        }

        [Fact]
        public void ShouldAppendConventions()
        {
            var properties = new Dictionary<object, object>();
            AutoFake.Provide<IDictionary<object, object>>(properties);
            var builder = AutoFake.Resolve<CCBuilder>();

            var conventionsArray = Enumerable.Range(0, 10).Select(x => A.Fake<IServiceConvention>()).ToArray();
            var conventionsEnumerable = Enumerable.Range(0, 10).Select(x => A.Fake<IServiceConvention>());
            builder.AppendConvention(conventionsArray)
                .AppendConvention(conventionsEnumerable)
                .AppendConvention<C>();

            A.CallTo(() => builder.Scanner.AppendConvention(A<IEnumerable<IServiceConvention>>.Ignored))
                .MustHaveHappened(2, Times.Exactly);

            A.CallTo(() => builder.Scanner.AppendConvention<C>())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldAppendDelegates()
        {
            var properties = new Dictionary<object, object>();
            AutoFake.Provide<IDictionary<object, object>>(properties);
            var builder = AutoFake.Resolve<CCBuilder>();

            var conventionsArray = Enumerable.Range(0, 10).Select(x => A.Fake<ServiceConventionDelegate>()).ToArray();
            var conventionsEnumerable = Enumerable.Range(0, 10).Select(x => A.Fake<ServiceConventionDelegate>());
            builder.AppendDelegate(conventionsArray)
                .AppendDelegate(conventionsEnumerable);

            A.CallTo(() => builder.Scanner.AppendDelegate(A<Delegate[]>.Ignored))
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => builder.Scanner.AppendDelegate(A<IEnumerable<Delegate>>.Ignored))
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependConventions()
        {
            var properties = new Dictionary<object, object>();
            AutoFake.Provide<IDictionary<object, object>>(properties);
            var builder = AutoFake.Resolve<CCBuilder>();

            var conventionsArray = Enumerable.Range(0, 10).Select(x => A.Fake<IServiceConvention>()).ToArray();
            var conventionsEnumerable = Enumerable.Range(0, 10).Select(x => A.Fake<IServiceConvention>());
            builder.PrependConvention(conventionsArray)
                .PrependConvention(conventionsEnumerable)
                .PrependConvention<C>();

            A.CallTo(() => builder.Scanner.PrependConvention(A<IEnumerable<IServiceConvention>>.Ignored))
                .MustHaveHappened(2, Times.Exactly);

            A.CallTo(() => builder.Scanner.PrependConvention<C>())
                .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void ShouldPrependDelegates()
        {
            var properties = new Dictionary<object, object>();
            AutoFake.Provide<IDictionary<object, object>>(properties);
            var builder = AutoFake.Resolve<CCBuilder>();

            var conventionsArray = Enumerable.Range(0, 10).Select(x => A.Fake<ServiceConventionDelegate>()).ToArray();
            var conventionsEnumerable = Enumerable.Range(0, 10).Select(x => A.Fake<ServiceConventionDelegate>());
            builder.PrependDelegate(conventionsArray)
                .PrependDelegate(conventionsEnumerable);

            A.CallTo(() => builder.Scanner.PrependDelegate(A<Delegate[]>.Ignored))
                .MustHaveHappened(1, Times.Exactly);

            A.CallTo(() => builder.Scanner.PrependDelegate(A<IEnumerable<Delegate>>.Ignored))
                .MustHaveHappened(1, Times.Exactly);
        }
    }
}
