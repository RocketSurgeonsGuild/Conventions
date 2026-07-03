using Autofac;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;

namespace Clavus.Autofac.Tests;

public static class AutofacFixtures
{
    public interface IAbc;

    public interface IAbc2;

    public interface IAbc3;

    public interface IAbc4;

    public interface IOtherAbc3;

    public interface IOtherAbc4;

    [ExportClavusPart]
    public class AbcConvention : IAutofacPart
    {
        public void Register(IClavusContext context, IServiceCollection services, ContainerBuilder container)
        {
            container.RegisterInstance(A.Fake<IAbc>());
            services.AddSingleton(A.Fake<IAbc2>());
        }
    }

    [ExportClavusPart]
    public class OtherConvention : IServicePart
    {
        public void Register(IClavusContext context, IServiceCollection services) => services.AddSingleton(A.Fake<IOtherAbc3>());
    }
}
