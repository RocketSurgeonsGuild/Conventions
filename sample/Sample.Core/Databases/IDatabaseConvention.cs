using Rocket.Surgery.Conventions;

namespace Sample.Core.Databases;

#region codeblock

public interface IDatabaseConvention : IConvention
{
    public void Register(IConventionContext context, IDatabaseConfigurator configurator);
}

#endregion
