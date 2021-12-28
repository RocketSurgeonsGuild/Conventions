using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Conventions;

/// <summary>
///     Created on demand before each generation pass
/// </summary>
internal class ConventionAttributeSyntaxReceiver : ISyntaxReceiver
{
    public List<AttributeListSyntax> ExportCandidates { get; } = new List<AttributeListSyntax>();
    public List<TypeDeclarationSyntax> ExportedConventions { get; } = new List<TypeDeclarationSyntax>();
    public List<SyntaxNode> ImportCandidates { get; } = new List<SyntaxNode>();

    /// <summary>
    ///     Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        {
            // any field with at least one attribute is a candidate for property generation
            if (
                syntaxNode is AttributeListSyntax attributeListSyntax
             && attributeListSyntax.Target is { Identifier: { RawKind: (int)SyntaxKind.AssemblyKeyword } }
             && attributeListSyntax.Attributes.Any(z => z.Name.ToFullString().TrimEnd().EndsWith("Convention", StringComparison.Ordinal))
            )
            {
                ExportCandidates.Add(attributeListSyntax);
            }
        }

        {
            if (syntaxNode is TypeDeclarationSyntax baseType && baseType is ClassDeclarationSyntax or RecordDeclarationSyntax
                                                             && baseType.AttributeLists.Any(
                                                                    z => z.Target is null or { Identifier: { RawKind: (int)SyntaxKind.ClassKeyword } }
                                                                      && z.Attributes.Any(
                                                                             c => c.Name.ToFullString().TrimEnd().EndsWith(
                                                                                 "ExportConvention", StringComparison.Ordinal
                                                                             ) || c.Name.ToFullString().TrimEnd().EndsWith(
                                                                                 "ExportConventionAttribute", StringComparison.Ordinal
                                                                             )
                                                                         )
                                                                )
               )
            {
                ExportedConventions.Add(baseType);
            }
        }

        {
            // any field with at least one attribute is a candidate for property generation
            if (syntaxNode is AttributeListSyntax attributeListSyntax
             && attributeListSyntax.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true
             && attributeListSyntax.Attributes.Any(
                    z => z.Name.ToFullString().TrimEnd().EndsWith("ImportConventions", StringComparison.OrdinalIgnoreCase)
                      || z.Name.ToFullString().TrimEnd().EndsWith("ImportConventionsAttribute", StringComparison.OrdinalIgnoreCase)
                ))
            {
                ImportCandidates.Add(attributeListSyntax);
            }

            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
             && classDeclarationSyntax.AttributeLists.SelectMany(z => z.Attributes)
                                      .Any(
                                           z => z.Name.ToFullString().TrimEnd().EndsWith("ImportConventions", StringComparison.OrdinalIgnoreCase)
                                             || z.Name.ToFullString().TrimEnd().EndsWith("ImportConventionsAttribute", StringComparison.OrdinalIgnoreCase)
                                       ))
            {
                ImportCandidates.Add(classDeclarationSyntax);
            }
        }
    }
}
