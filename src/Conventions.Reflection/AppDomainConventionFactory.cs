using Rocket.Surgery.DependencyInjection.Compiled;

namespace Rocket.Surgery.Conventions.Reflection;

/// <inheritdoc />
[RequiresUnreferencedCode("TypeSelector.GetTypesInternal may remove members at compile time")]
public class AppDomainConventionFactory(AppDomain appDomain) : ConventionFactoryBase
{
    /// <inheritdoc />
    public override ICompiledTypeProvider CreateTypeProvider(ConventionContextBuilder builder)
    {
        return new AppDomainAssemblyProvider(appDomain);
    }
}
