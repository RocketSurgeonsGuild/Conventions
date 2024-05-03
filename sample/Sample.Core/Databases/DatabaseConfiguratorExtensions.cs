using Rocket.Surgery.Conventions;

namespace Sample.Core.Databases;

#region codeblock

public static class DatabaseConfiguratorExtensions
{
    public static IDatabaseConfigurator ApplyConventions(this IDatabaseConfigurator configurator, IConventionContext context)
    {
        foreach (var item in context.Conventions.Get<IDatabaseConvention, DatabaseConvention>())
        {
            if (item is IDatabaseConvention convention)
                convention.Register(context, configurator);
            else if (item is DatabaseConvention @delegate) @delegate(context, configurator);
        }

        return configurator;
    }
}

#endregion
