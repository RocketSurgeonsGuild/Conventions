using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Created on demand before each generation pass
    /// </summary>
    internal class ConventionAttributeSyntaxReceiver : ISyntaxReceiver
    {
        public List<AttributeListSyntax> ExportCandidates { get; } = new List<AttributeListSyntax>();
        public List<SyntaxNode> ImportCandidates { get; } = new List<SyntaxNode>();

        /// <summary>
        /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
        /// </summary>
        void ISyntaxReceiver.OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is AttributeListSyntax attributeListSyntax
                 && attributeListSyntax.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true
                 && attributeListSyntax.Attributes.Any(z => z.Name.ToFullString().EndsWith("Convention", StringComparison.OrdinalIgnoreCase)))
                {
                    ExportCandidates.Add(attributeListSyntax);
                }
            }

            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is AttributeListSyntax attributeListSyntax
                 && attributeListSyntax.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true
                 && attributeListSyntax.Attributes.Any(z => z.Name.ToFullString().EndsWith("ImportConventions", StringComparison.OrdinalIgnoreCase)))
                {
                    ImportCandidates.Add(attributeListSyntax);
                }

                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
                 && classDeclarationSyntax.AttributeLists.SelectMany(z => z.Attributes).Any(z => z.Name.ToFullString().EndsWith("ImportConventions", StringComparison.OrdinalIgnoreCase)))
                {
                    ImportCandidates.Add(classDeclarationSyntax);
                }
            }
        }
    }
}