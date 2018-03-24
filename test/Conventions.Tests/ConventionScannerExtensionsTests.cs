using System;
using System.Linq;
using FakeItEasy;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionScannerExtensionsTests
    {
        [Fact]
        public void ShouldAddConvention_WithParams()
        {
            var scanner = A.Fake<IConventionScanner>();
            var convention1 = A.Fake<IConvention>();
            var convention2 = A.Fake<IConvention>();
            var convention3 = A.Fake<IConvention>();

            scanner.PrependConvention(convention1, convention2, convention3);

            A.CallTo(() => scanner.PrependConvention(A<IConvention>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void ShouldAddConvention_WithEnumerable()
        {
            var scanner = A.Fake<IConventionScanner>();
            var convention1 = A.Fake<IConvention>();
            var convention2 = A.Fake<IConvention>();
            var convention3 = A.Fake<IConvention>();

            scanner.PrependConvention(new[] { convention1, convention2, convention3 }.AsEnumerable());

            A.CallTo(() => scanner.PrependConvention(A<IConvention>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void ShouldAddConventionDelegate_WithParams()
        {
            var scanner = A.Fake<IConventionScanner>();
            var d1 = new ServiceConventionDelegate(context => { });
            var d2 = new ServiceConventionDelegate(context => { });
            var d3 = new ServiceConventionDelegate(context => { });

            scanner.PrependDelegate(d1, d2, d3);

            A.CallTo(() => scanner.PrependDelegate(A<Delegate>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void ShouldAddConventionDelegate_WithEnumerable()
        {
            var scanner = A.Fake<IConventionScanner>();
            var d1 = new ServiceConventionDelegate(context => { });
            var d2 = new ServiceConventionDelegate(context => { });
            var d3 = new ServiceConventionDelegate(context => { });

            scanner.PrependDelegate(new[] { d1, d2, d3 }.AsEnumerable());

            A.CallTo(() => scanner.PrependDelegate(A<Delegate>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void ShouldExcludeConvention_WithParams()
        {
            var scanner = A.Fake<IConventionScanner>();
            var d1 = A.Fake<IConvention>().GetType();
            var d2 = A.Fake<IConvention>().GetType();
            var d3 = A.Fake<IConvention>().GetType();

            scanner.ExceptConvention(d1, d2, d3);

            A.CallTo(() => scanner.ExceptConvention(A<Type>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }

        [Fact]
        public void ShouldExcludeConvention_WithEnumerable()
        {
            var scanner = A.Fake<IConventionScanner>();
            var d1 = A.Fake<IConvention>().GetType();
            var d2 = A.Fake<IConvention>().GetType();
            var d3 = A.Fake<IConvention>().GetType();

            scanner.ExceptConvention(new[] { d1, d2, d3 }.AsEnumerable());

            A.CallTo(() => scanner.ExceptConvention(A<Type>._)).MustHaveHappened(Repeated.Exactly.Times(3));
        }
    }
}
