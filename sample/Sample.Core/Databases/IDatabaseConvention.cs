namespace Sample.Core.Databases;

#region codeblock

public interface IDatabaseConvention : IClavusPart
{
    void Register(IClavusContext context, IDatabaseConfigurator configurator);
}

public interface IDatabaseAsyncConvention : IClavusPart
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configurator"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IClavusContext context, IDatabaseConfigurator configurator, CancellationToken cancellationToken);
}

#endregion
