using FakeItEasy;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class StaticConventionComposerTests
    {
        public interface ITestConventionContext : IConventionContext { }
        public interface ITestConvention : IConvention<ITestConventionContext> { }

        public delegate void TestContributionDelegate(ITestConventionContext context);

        [Fact]
        public void RegisterShouldCallContribution()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var contribution = A.Fake<ITestConvention>();
            var context = A.Fake<ITestConventionContext>();

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[] { new DelegateOrConvention(contribution) });

            Composer.Register<ITestConventionContext, ITestConvention, TestContributionDelegate>(scanner, context);

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

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[]
                {
                    new DelegateOrConvention(contribution1),
                    new DelegateOrConvention(contribution2),
                    new DelegateOrConvention(contribution3)
                });

            Composer.Register<ITestConventionContext, ITestConvention, TestContributionDelegate>(scanner, context);

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

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new DelegateOrConvention[] { @delegate });

            Composer.Register<ITestConventionContext, ITestConvention, TestContributionDelegate>(scanner, context);

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

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[] { new DelegateOrConvention(delegate2),
                    new DelegateOrConvention(delegate1),
                    new DelegateOrConvention(delegate3 )
                });

            Composer.Register<ITestConventionContext, ITestConvention, TestContributionDelegate>(scanner, context);

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

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestContributionDelegate>())
                .Returns(new[] { new DelegateOrConvention(@delegate),
                    new DelegateOrConvention(contribution )
                });

            Composer.Register<ITestConventionContext, ITestConvention, TestContributionDelegate>(scanner, context);

            A.CallTo(() => contribution.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
