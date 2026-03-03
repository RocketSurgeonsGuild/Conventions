using Rocket.Surgery.Conventions;

namespace Sample.Core.Databases;

#region codeblock

public interface IDatabaseConvention : IConvention
{
    void Register(IConventionContext context, IDatabaseConfigurator configurator);
}

public interface IDatabaseAsyncConvention : IConvention
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="configurator"></param>
    /// <param name="cancellationToken"></param>
    ValueTask Register(IConventionContext context, IDatabaseConfigurator configurator, CancellationToken cancellationToken);
}

#endregion
