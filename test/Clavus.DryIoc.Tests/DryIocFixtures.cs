using DryIoc;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Surgery.Clavus.DependencyInjection;
using Rocket.Surgery.Clavus.DryIoc;

namespace Rocket.Surgery.Extensions.DryIoc.Tests;

public static class DryIocFixtures
{
    public interface IAbc;

    public interface IAbc2;

    public interface IAbc3;

    public interface IAbc4;

    public interface IOtherAbc3;

    public interface IOtherAbc4;

    [ExportClavusPart]
    public class AbcConvention : IDryIocConvention
    {
        public IContainer Register(IClavusContext context, IConfiguration configuration, IServiceCollection services, IContainer container)
        {
            container.RegisterInstance(A.Fake<IAbc>());
            services.AddSingleton(A.Fake<IAbc2>());
            return container;
        }
    }

    [ExportClavusPart]
    public class OtherConvention : IServicePart
    {
        public void Register(IClavusContext context, IConfiguration configuration, IServiceCollection services) => services.AddSingleton(A.Fake<IOtherAbc3>());
    }
}
