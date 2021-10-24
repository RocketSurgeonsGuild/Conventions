using System.Text;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.Conventions;

internal static class Helpers
{
    public static string GetFullMetadataName(ISymbol? s)
    {
        if (s == null || IsRootNamespace(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!IsRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
            {
                sb.Insert(0, '+');
            }
            else
            {
                sb.Insert(0, '.');
            }

            sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            //sb.Insert(0, s.MetadataName);
            s = s.ContainingSymbol;
        }

        return sb.ToString();

        static bool IsRootNamespace(ISymbol symbol)
        {
            INamespaceSymbol? s;
            return ( s = symbol as INamespaceSymbol ) != null && s.IsGlobalNamespace;
        }
    }
}
