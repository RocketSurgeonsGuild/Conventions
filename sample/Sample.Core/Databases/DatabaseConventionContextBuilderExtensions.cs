using Rocket.Surgery.Conventions;

// ReSharper disable UnusedParameter.Local
namespace Sample.Core.Databases;

#region codeblock

public static class DatabaseConventionContextBuilderExtensions
{
    public static ConventionContextBuilder ConfigureDatabase(this ConventionContextBuilder container, DatabaseConvention @delegate)
    {
        container.AppendDelegate(@delegate);
        return container;
    }

    public static ConventionContextBuilder ConfigureDatabase(this ConventionContextBuilder container, Action<IDatabaseConfigurator> @delegate)
    {
        container.AppendDelegate(new DatabaseConvention((context, configurator) => @delegate(configurator)));
        return container;
    }
}

#endregion
