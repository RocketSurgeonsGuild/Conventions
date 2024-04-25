using Rocket.Surgery.Conventions;

namespace Sample.Core.Databases;

#region codeblock

public static class DatabaseConfiguratorExtensions
{
    public static IDatabaseConfigurator ApplyConventions(this IDatabaseConfigurator configurator, IConventionContext conventionContext)
    {
        foreach (var item in conventionContext.Conventions.Get<IDatabaseConvention, DatabaseConvention>())
        {
            if (item is IDatabaseConvention convention)
                convention.Register(conventionContext, configurator);
            else if (item is DatabaseConvention @delegate) @delegate(conventionContext, configurator);
        }

        return configurator;
    }
}

#endregion