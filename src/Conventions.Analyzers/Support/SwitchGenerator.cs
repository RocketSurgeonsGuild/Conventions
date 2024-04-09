using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Rocket.Surgery.Conventions.Helpers;

namespace Rocket.Surgery.Conventions.Support;

internal static class SwitchGenerator
{
    public static SwitchStatementSyntax GenerateSwitchStatement(List<(SourceLocation location, BlockSyntax block)> items)
    {
        var lineNumberIdentifier = IdentifierName("lineNumber");
        var switchStatement = SwitchStatement(lineNumberIdentifier);
        foreach (var lineGrouping in items.GroupBy(x => x.location.LineNumber))
        {
            var lineSwitchSection = createNestedSwitchSections(
                    lineGrouping.ToArray(),
                    IdentifierName("filePath"),
                    x => x.location.FilePath,
                    generateFilePathSwitchStatement,
                    value => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(value))
                )
               .AddLabels(CaseSwitchLabel(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(lineGrouping.Key))));

            switchStatement = switchStatement.AddSections(lineSwitchSection);
        }

        return switchStatement;
    }

    static SwitchSectionSyntax createNestedSwitchSections<T>(
        IReadOnlyList<(SourceLocation location, BlockSyntax block)> blocks,
        NameSyntax identifier,
        Func<(SourceLocation location, BlockSyntax block), T> regroup,
        Func<IGrouping<T, (SourceLocation location, BlockSyntax block)>, SwitchSectionSyntax> next,
        Func<T, LiteralExpressionSyntax> literalFactory
    )
    {
        if (blocks is [var localBlock])
        {
            return SwitchSection()
                                .AddStatements(localBlock.block.Statements.ToArray())
                                .AddStatements(BreakStatement());
        }

        var section = SwitchStatement(identifier);
        foreach (var item in blocks.GroupBy(regroup))
        {
            section = section.AddSections(next(item).AddLabels(CaseSwitchLabel(literalFactory(item.Key))));
        }

        return SwitchSection().AddStatements(section, BreakStatement());
    }

    static SwitchSectionSyntax generateFilePathSwitchStatement(IGrouping<string, (SourceLocation location, BlockSyntax block)> innerGroup)
    {
        return createNestedSwitchSections(
            innerGroup.ToArray(),
            IdentifierName("memberName"),
            x => x.location.MemberName,
            generateMemberNameSwitchStatement,
            value =>
                LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    Literal(value)
                )
        );
    }

    static SwitchSectionSyntax generateMemberNameSwitchStatement(IGrouping<string, (SourceLocation location, BlockSyntax block)> innerGroup)
    {
        return SwitchSection()
                            .AddLabels(
                                 CaseSwitchLabel(
                                     LiteralExpression(
                                         SyntaxKind.StringLiteralExpression,
                                         Literal(innerGroup.Key)
                                     )
                                 )
                             )
                            .AddStatements(innerGroup.FirstOrDefault().block?.Statements.ToArray() ?? Array.Empty<StatementSyntax>())
                            .AddStatements(BreakStatement());
    }
}
