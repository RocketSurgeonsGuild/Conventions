namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
public class AppDomainConventionFactory(AppDomain appDomain) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AppDomainAssemblyProvider(appDomain);
}