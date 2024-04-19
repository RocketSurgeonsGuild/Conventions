using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions.Analyzers.Support.AssemblyProviders;

internal interface ICompiledTypeFilter<TSymbol>
{
    bool IsMatch(Compilation compilation, TSymbol targetType);
}