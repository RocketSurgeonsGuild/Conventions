using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionComposerTests
    {
        public interface ITestConventionContext : IConventionContext { }
        public interface ITestConvention : IConvention<ITestConventionContext> { }

        public delegate void TestContributionDelegate(ITestConventionContext context);

        public class TestConventionComposer : ConventionComposer<ITestConventionContext, ITestConvention, TestContributionDelegate> { public TestConventionComposer(IConventionScanner scanner, ILogger logger) : base(scanner, logger) { } }
        public class NotADelegateComposer : ConventionComposer<ITestConventionContext, ITestConvention, String> { public NotADelegateComposer(IConventionScanner scanner, ILogger logger) : base(scanner, logger) { } }

        [Fact]
        public void ShouldConstructComposer()
        {
            var scanner = A.Fake<IConventionScanner>();
            var composer = new TestConventionComposer(scanner, A.Fake<ILogger>());

            composer.Should().NotBeNull();
        }

        [Fact]
        public void ShouldThrowIfDelegateIsNotADelegate()
        {
            var scanner = A.Fake<IConventionScanner>();
            Action action = () => new NotADelegateComposer(scanner, A.Fake<ILogger>());
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RegisterShouldCallContribution()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var contribution = A.Fake<ITestConvention>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[] { new DelegateOrConvention<ITestConvention, TestContributionDelegate>(contribution) });

            composer.Register(context);

            A.CallTo(() => contribution.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallContributions()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var contribution1 = A.Fake<ITestConvention>();
            var contribution2 = A.Fake<ITestConvention>();
            var contribution3 = A.Fake<ITestConvention>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[]
                {
                    new DelegateOrConvention<ITestConvention, TestContributionDelegate>(contribution1),
                    new DelegateOrConvention<ITestConvention, TestContributionDelegate>(contribution2),
                    new DelegateOrConvention<ITestConvention, TestContributionDelegate>(contribution3)
                });

            composer.Register(context);

            A.CallTo(() => contribution1.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => contribution2.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => contribution3.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallDelegate()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var @delegate = A.Fake<TestContributionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new DelegateOrConvention<ITestConvention, TestContributionDelegate>[] { @delegate });

            composer.Register(context);

            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var delegate1 = A.Fake<TestContributionDelegate>();
            var delegate2 = A.Fake<TestContributionDelegate>();
            var delegate3 = A.Fake<TestContributionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[] { new DelegateOrConvention<ITestConvention, TestContributionDelegate>(delegate2),
                    new DelegateOrConvention<ITestConvention, TestContributionDelegate>(delegate1),
                    new DelegateOrConvention<ITestConvention, TestContributionDelegate>(delegate3 )
                });

            composer.Register(context);

            A.CallTo(() => @delegate1.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate2.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate3.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallContributionsAndDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var contribution = A.Fake<ITestConvention>();
            var @delegate = A.Fake<TestContributionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[] { new DelegateOrConvention<ITestConvention, TestContributionDelegate>(@delegate),
                    new DelegateOrConvention<ITestConvention, TestContributionDelegate>(contribution )
                });

            composer.Register(context);

            A.CallTo(() => contribution.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
