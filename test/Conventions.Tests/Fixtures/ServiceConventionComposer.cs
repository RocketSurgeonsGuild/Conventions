using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions.Tests.Fixtures
{
    public class ServiceConventionComposer : ConventionComposer<ServiceConventionContext, IServiceConvention, ServiceConventionDelegate>
    {
        public ServiceConventionComposer(IConventionScanner scanner) : base(scanner)
        {
        }
    }
}
