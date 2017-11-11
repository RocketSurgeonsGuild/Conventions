using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionComposerTests
    {

        [Fact]
        public void ShouldConstructComposer()
        {
            var scanner = A.Fake<IConventionScanner>();
            var composer = new ConventionComposer(scanner, A.Fake<ILogger>());

            composer.Should().NotBeNull();
        }

        [Fact]
        public void RegisterShouldCallConvention()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention = A.Fake<ITestConvention>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new ConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll())
                .Returns(new[] { new DelegateOrConvention(convention) });

            composer.Register(context);

            A.CallTo(() => convention.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallBothConvention()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention = A.Fake<ITestConvention>();
            var convention2 = A.Fake<IServiceConvention>();
            var context = A.Fake<IConventionContext>(c => c.Implements<ITestConventionContext>().Implements<IServiceConventionContext>());
            var composer = new ConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll())
                .Returns(new[] { new DelegateOrConvention(convention), new DelegateOrConvention(convention2),  });

            composer.Register(context);

            A.CallTo(() => convention.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => convention2.Register(A<IServiceConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
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
            var composer = new ConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.GetAll())
                .Returns(new[]
                {
                    new DelegateOrConvention(convention1),
                    new DelegateOrConvention(convention2),
                    new DelegateOrConvention(convention3)
                });

            composer.Register(context);

            A.CallTo(() => convention1.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => convention2.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => convention3.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallDelegate()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new ConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.GetAll())
                .Returns(new DelegateOrConvention[] { @delegate });

            composer.Register(context);

            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
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
            var composer = new ConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.GetAll())
                .Returns(new[] {
                    new DelegateOrConvention(delegate2),
                    new DelegateOrConvention(delegate1),
                    new DelegateOrConvention(delegate3 )
                });

            composer.Register(context);

            A.CallTo(() => @delegate1.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate2.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate3.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallConventionsAndDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var convention = A.Fake<ITestConvention>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new ConventionComposer(scanner, A.Fake<ILogger>());

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.GetAll())
                .Returns(new[] {
                    new DelegateOrConvention(@delegate),
                    new DelegateOrConvention(convention )
                });

            composer.Register(context);

            A.CallTo(() => convention.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
