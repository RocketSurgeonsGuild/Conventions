using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable ObjectCreationAsStatement
#pragma warning disable CA1806

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionTests : AutoFakeTest
    {
        [Fact]
        public void ConventionAttributeThrowsIfNonConventionGiven()
        {
            Action a = () => new ConventionAttribute(typeof(object));
            a.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ComposerCallsValuesAsExpected()
        {
            var scanner = A.Fake<IConventionScanner>();
            var provider = A.Fake<IConventionProvider>();

            var contrib = A.Fake<IServiceConvention>();
            var contrib2 = A.Fake<IServiceConvention>();
            var dele = A.Fake<ServiceConventionDelegate>();
            var dele2 = A.Fake<ServiceConventionDelegate>();

            A.CallTo(() => scanner.BuildProvider()).Returns(provider);
            A.CallTo(() => provider.Get<IServiceConvention, ServiceConventionDelegate>(HostType.Undefined))
               .Returns(
                    new[]
                    {
                        new DelegateOrConvention(contrib),
                        new DelegateOrConvention(contrib2),
                        new DelegateOrConvention(dele),
                        new DelegateOrConvention(dele2)
                    }.AsEnumerable()
                );

            Composer.Register<ServiceConventionContext, IServiceConvention, ServiceConventionDelegate>(
                provider,
                new ServiceConventionContext(Logger)
            );
            Composer.Register<ServiceConventionContext, IServiceConvention, ServiceConventionDelegate>(
                provider,
                new ServiceConventionContext(Logger)
            );
            A.CallTo(() => dele.Invoke(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => dele2.Invoke(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => contrib.Register(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => contrib2.Register(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
        }

        public ConventionTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}