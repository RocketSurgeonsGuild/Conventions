using Rocket.Surgery.Conventions;

namespace Sample.Core.Databases;

#region codeblock

public delegate void DatabaseConvention(IConventionContext context, IDatabaseConfigurator configurator);

public delegate ValueTask DatabaseAsyncConvention(IConventionContext context, IDatabaseConfigurator configurator, CancellationToken cancellationToken);

#endregion
