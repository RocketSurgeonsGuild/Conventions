using Rocket.Surgery.Conventions;

namespace Sample.Core.Databases;

#region codeblock

public static class DatabaseConfiguratorExtensions
{
    public static ValueTask ApplyConventions(this IDatabaseConfigurator configurator, IConventionContext context, CancellationToken cancellationToken = default) => context.RegisterConventions(z => z
                                                                                                                                                                         .AddHandler<IDatabaseConvention>(convention => convention.Register(context, configurator))
                                                                                                                                                                         .AddHandler<IDatabaseAsyncConvention>((convention) => convention.Register(context, configurator, cancellationToken))
                                                                                                                                                                         .AddHandler<DatabaseConvention>(convention => convention(context, configurator))
                                                                                                                                                                         .AddHandler<DatabaseAsyncConvention>((convention) => convention(context, configurator, cancellationToken))
        );
}

#endregion
