using DryIoc;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;

namespace Clavus.DryIoc.Tests;

public static class DryIocFixtures
{
    public interface IAbc;

    public interface IAbc2;

    public interface IAbc3;

    public interface IAbc4;

    public interface IOtherAbc3;

    public interface IOtherAbc4;

    [ExportClavusPart]
    public class AbcConvention : IDryIocPart, IServicePart
    {
        public void Register(IClavusContext context, IContainer container) => container.RegisterInstance(A.Fake<IAbc>());

        public void Register(IClavusContext context, IServiceCollection services) => services.AddSingleton(A.Fake<IAbc2>());
    }

    [ExportClavusPart]
    public class OtherConvention : IServicePart
    {
        public void Register(IClavusContext context, IServiceCollection services) => services.AddSingleton(A.Fake<IOtherAbc3>());
    }
}
