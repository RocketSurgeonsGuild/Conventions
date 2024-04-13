namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
public class AppDomainConventionFactory(AppDomain appDomain) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override IAssemblyProvider CreateAssemblyProvider(ConventionContextBuilder builder) => new AppDomainAssemblyProvider(appDomain);
}
