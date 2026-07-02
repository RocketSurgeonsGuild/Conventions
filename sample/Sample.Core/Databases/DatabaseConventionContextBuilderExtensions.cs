// ReSharper disable UnusedParameter.Local
namespace Sample.Core.Databases;

#region codeblock

public static class DatabaseClavusContextBuilderExtensions
{
    public static ClavusContextBuilder ConfigureDatabase(this ClavusContextBuilder container, DatabaseConvention @delegate)
    {
        container.AppendDelegate(@delegate, 0, ClavusCategory.Application);
        return container;
    }

    public static ClavusContextBuilder ConfigureDatabase(this ClavusContextBuilder container, Action<IDatabaseConfigurator> @delegate)
    {
        container.AppendDelegate(new DatabaseConvention((context, configurator) => @delegate(configurator)), 0, ClavusCategory.Application);
        return container;
    }
}

#endregion
