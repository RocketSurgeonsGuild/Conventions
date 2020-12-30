using Rocket.Surgery.Conventions;

namespace Sample.Core.Databases
{
    #region codeblock
    public delegate void DatabaseConvention(IConventionContext context, IDatabaseConfigurator configurator);
    #endregion
}