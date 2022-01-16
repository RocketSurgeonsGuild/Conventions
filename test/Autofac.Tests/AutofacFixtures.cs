using Autofac;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Autofac;
using Rocket.Surgery.Conventions.DependencyInjection;

namespace Rocket.Surgery.Extensions.Autofac.Tests;

public class AutofacFixtures
{
    public interface IAbc
    {
    }

    public interface IAbc2
    {
    }

    public interface IAbc3
    {
    }

    public interface IAbc4
    {
    }

    public interface IOtherAbc3
    {
    }

    public interface IOtherAbc4
    {
    }

    [ExportConvention]
    public class AbcConvention : IAutofacConvention
    {
        public void Register(IConventionContext conventionContext, IConfiguration configuration, IServiceCollection services, ContainerBuilder container)
        {
            container.RegisterInstance(A.Fake<IAbc>());
            services.AddSingleton(A.Fake<IAbc2>());
        }
    }

    [ExportConvention]
    public class OtherConvention : IServiceConvention
    {
        public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
        {
            services.AddSingleton(A.Fake<IOtherAbc3>());
        }
    }
}
