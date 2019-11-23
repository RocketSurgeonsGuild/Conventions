using FakeItEasy;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionComposerTests
    {
        [Fact]
        public void RegisterShouldCallConvention()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention = A.Fake<IConvention>(c => c.Implements<ITestConvention>().Implements<IServiceConvention>());
            var context = A.Fake<ITestConventionContext>(c => c.Implements<IServiceConventionContext>());

            A.CallTo(() => scanner.BuildProvider())
               .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(new[] { new DelegateOrConvention(convention) });

            Composer.Register(scanner, context, typeof(ITestConvention), typeof(TestConventionDelegate));

            A.CallTo(() => ( (ITestConvention)convention ).Register(A<ITestConventionContext>._))
               .MustHaveHappenedOnceExactly();
            A.CallTo(() => ( (IServiceConvention)convention ).Register(A<IServiceConventionContext>._))
               .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RegisterShouldCallBothConvention()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention = A.Fake<IConvention>(c => c.Implements<ITestConvention>().Implements<IServiceConvention>());
            var convention2 = A.Fake<IServiceConvention>();
            var context = A.Fake<IConventionContext>(
                c => c.Implements<ITestConventionContext>().Implements<IServiceConventionContext>()
            );

            A.CallTo(() => scanner.BuildProvider())
               .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(new[] { new DelegateOrConvention(convention), new DelegateOrConvention(convention2) });

            Composer.Register(
                scanner,
                context,
                typeof(IServiceConvention),
                typeof(ServiceConventionDelegate),
                typeof(ITestConvention),
                typeof(TestConventionDelegate)
            );

            A.CallTo(() => ( (ITestConvention)convention ).Register(A<ITestConventionContext>._))
               .MustHaveHappenedOnceExactly();
            A.CallTo(() => ( (IServiceConvention)convention ).Register(A<IServiceConventionContext>._))
               .MustHaveHappenedOnceExactly();
            A.CallTo(() => convention2.Register(A<IServiceConventionContext>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RegisterShouldCallBothDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var delegate2 = A.Fake<ServiceConventionDelegate>();
            var context = A.Fake<IConventionContext>(
                c => c.Implements<ITestConventionContext>().Implements<IServiceConventionContext>()
            );

            A.CallTo(() => scanner.BuildProvider())
               .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(new[] { new DelegateOrConvention(@delegate), new DelegateOrConvention(delegate2) });

            Composer.Register(
                scanner,
                context,
                typeof(IServiceConvention),
                typeof(ServiceConventionDelegate),
                typeof(ITestConvention),
                typeof(TestConventionDelegate)
            );

            A.CallTo(() => @delegate(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => delegate2(A<IServiceConventionContext>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RegisterShouldCallAllTheThings()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention = A.Fake<ITestConvention>();
            var convention2 = A.Fake<IServiceConvention>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var delegate2 = A.Fake<ServiceConventionDelegate>();
            var context = A.Fake<IConventionContext>(
                c => c.Implements<ITestConventionContext>().Implements<IServiceConventionContext>()
            );

            A.CallTo(() => scanner.BuildProvider())
               .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(
                    new[]
                    {
                        new DelegateOrConvention(convention),
                        new DelegateOrConvention(convention2),
                        new DelegateOrConvention(@delegate),
                        new DelegateOrConvention(delegate2)
                    }
                );

            Composer.Register(
                scanner,
                context,
                typeof(IServiceConvention),
                typeof(ServiceConventionDelegate),
                typeof(ITestConvention),
                typeof(TestConventionDelegate)
            );

            A.CallTo(() => convention.Register(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => convention2.Register(A<IServiceConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => @delegate(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => delegate2(A<IServiceConventionContext>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RegisterShouldCallConventions()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention1 = A.Fake<ITestConvention>();
            var convention2 = A.Fake<ITestConvention>();
            var convention3 = A.Fake<ITestConvention>();
            var context = A.Fake<ITestConventionContext>();

            A.CallTo(() => scanner.BuildProvider())
               .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(
                    new[]
                    {
                        new DelegateOrConvention(convention1),
                        new DelegateOrConvention(convention2),
                        new DelegateOrConvention(convention3)
                    }
                );

            Composer.Register(scanner, context, typeof(ITestConvention), typeof(TestConventionDelegate));

            A.CallTo(() => convention1.Register(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => convention2.Register(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => convention3.Register(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RegisterShouldCallDelegate()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(new DelegateOrConvention[] { @delegate });

            Composer.Register(scanner, context, typeof(ITestConvention), typeof(TestConventionDelegate));

            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RegisterShouldCallDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var delegate1 = A.Fake<TestConventionDelegate>();
            var delegate2 = A.Fake<TestConventionDelegate>();
            var delegate3 = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(
                    new[]
                    {
                        new DelegateOrConvention(delegate2),
                        new DelegateOrConvention(delegate1),
                        new DelegateOrConvention(delegate3)
                    }
                );

            Composer.Register(scanner, context, typeof(ITestConvention), typeof(TestConventionDelegate));

            A.CallTo(() => delegate1.Invoke(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => delegate2.Invoke(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => delegate3.Invoke(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RegisterShouldCallConventionsAndDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention = A.Fake<ITestConvention>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.GetAll(HostType.Undefined))
               .Returns(
                    new[]
                    {
                        new DelegateOrConvention(@delegate),
                        new DelegateOrConvention(convention)
                    }
                );

            Composer.Register(scanner, context, typeof(ITestConvention), typeof(TestConventionDelegate));

            A.CallTo(() => convention.Register(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappenedOnceExactly();
        }
    }
}