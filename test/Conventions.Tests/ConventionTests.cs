using System.Linq;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionTests
    {

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
            A.CallTo(() => provider.Get<IServiceConvention, ServiceConventionDelegate>())
                .Returns(new[]
                {
                    new DelegateOrConvention<IServiceConvention, ServiceConventionDelegate>(contrib),
                    new DelegateOrConvention<IServiceConvention, ServiceConventionDelegate>(contrib2),
                    new DelegateOrConvention<IServiceConvention, ServiceConventionDelegate>(dele),
                    new DelegateOrConvention<IServiceConvention, ServiceConventionDelegate>(dele2),
                }.AsEnumerable());
            var composer = new ServiceConventionComposer(scanner, A.Fake<ILogger>());

            composer.Register(new ServiceConventionContext());
            composer.Register(new ServiceConventionContext());
            A.CallTo(() => dele.Invoke(A<ServiceConventionContext>._)).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => dele2.Invoke(A<ServiceConventionContext>._)).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => contrib.Register(A<ServiceConventionContext>._)).MustHaveHappened(Repeated.Exactly.Twice);
            A.CallTo(() => contrib2.Register(A<ServiceConventionContext>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }
    }
}
