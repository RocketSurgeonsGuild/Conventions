namespace Sample.Core.Databases;

#region codeblock

public delegate void DatabaseConvention(IClavusContext context, IDatabaseConfigurator configurator);

public delegate ValueTask DatabaseAsyncConvention(IClavusContext context, IDatabaseConfigurator configurator, CancellationToken cancellationToken);

#endregion
