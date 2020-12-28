# Creating a custom convention
The essence of a convention is a method, with any number of parameters, that can be implemented to do some work.

Lets say you want to create a convention for handling database configuration.

Goal: Distribute database configuration across assemblies

Given this fake interface for configuring the database.
```c#
public interface IDatabaseConfigurator
{
    void AddTable(string name);
    void AddView(string name);
}
```

## Define your interface and optional delegate
Now we will define our interface and delegate.  The delegate and extenison methods are optional but makes it easier for consumers to create adhoc conventions.


```c#
public interface IDatabaseConvention : IConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IDatabaseConfigurator configurator);
}

public void DatabaseConvention(IConventionContext context, IConfiguration configuration, IDatabaseConfigurator configurator);

public static class DatabaseConventionContextBuilderExtensions
{
    public static ConventionContextBuilder ConfigureDatabase([NotNull] this ConventionContextBuilder container, DatabaseConvention @delegate)
    {
        container.AppendDelegate(@delegate);
        return container;
    }

    public static ConventionContextBuilder ConfigureDatabase([NotNull] this ConventionContextBuilder container, Action<IConfiguration, IDatabaseConfigurator> @delegate)
    {
        container.AppendDelegate(new DatabaseConvention((context, configuration, configurator) => @delegate(configuration, configurator)));
        return container;
    }

    public static ConventionContextBuilder ConfigureDatabase([NotNull] this ConventionContextBuilder container, Action<IDatabaseConfigurator> @delegate)
    {
        container.AppendDelegate(new DatabaseConvention((context, configuration, configurator) => @delegate(configurator)));
        return container;
    }
}
```

## Create your application method
We have defined our conventions now we need to be able to apply our conventions.

There are two ways to do this, if for example your database configuration happens during service registration (perhaps via `IOptions`) then you can implement this
a convention that will run during the services convention.  The other way additional way is to create an extension method that takes takes the convention and applies it.

In this example we'll support both.  The manual way using an extension method and a convention that will do it "automagically".

```c#
public static class RocketSurgeryServiceCollectionExtensions
{
    public static IDatabaseConfigurator ApplyConventions(this IDatabaseConfigurator configurator, IConventionContext conventionContext)
    {
        var configuration = conventionContext.Get<IConfiguration>() ?? throw new ArgumentException("Configuration was not found in context", nameof(conventionContext));
        foreach (var item in conventionContext.Conventions.Get<IDatabaseConvention, DatabaseConvention>())
        {
            if (item is IDatabaseConvention convention)
            {
                convention.Register(conventionContext, configuration, configurator);
            }
            else if (item is DatabaseConvention @delegate)
            {
                @delegate(conventionContext, configuration, configurator);
            }
        }

        return services;
    }
}

[assembly: Convention(typeof(DatabaseConvention))]
public class DatabaseConvention : IServiceConvention
{
    public void Register(IConventionContext context, IConfiguration configuration, IServiceCollection services)
    {
        var configurator = new DatabseConfigurator(); // or whatever is required
        configurator.ApplyConventions(context, configuration, configurator);
    }
}
```

