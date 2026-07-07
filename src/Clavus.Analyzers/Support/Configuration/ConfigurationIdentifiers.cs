using System.Text;

namespace Clavus.Support.Configuration;

/// <summary>
///     Helpers for turning configuration keys/file names into valid, conventional C# identifiers.
/// </summary>
internal static class ConfigurationIdentifiers
{
    /// <summary>
    ///     Converts a configuration key segment (e.g. <c>connection-string</c>, <c>max_retries</c>, <c>enabled</c>)
    ///     into a PascalCase C# identifier segment (e.g. <c>ConnectionString</c>, <c>MaxRetries</c>, <c>Enabled</c>).
    /// </summary>
    public static string ToPascalCase(string segment)
    {
        if (string.IsNullOrEmpty(segment)) return "_";

        var builder = new StringBuilder(segment.Length);
        var upperNext = true;
        foreach (var ch in segment)
        {
            if (ch is '-' or '_' or ' ' or '.')
            {
                upperNext = true;
                continue;
            }

            builder.Append(upperNext ? char.ToUpperInvariant(ch) : ch);
            upperNext = false;
        }

        if (builder.Length == 0) return "_";
        if (char.IsDigit(builder[0])) builder.Insert(0, '_');

        return EscapeIfKeyword(builder.ToString());
    }

    /// <summary>
    ///     Derives the root generated class name from a configuration base name (e.g. <c>appsettings</c> -&gt;
    ///     <c>AppSettingsConfiguration</c>), per design.md Decision 4.
    /// </summary>
    public static string ToRootClassName(string baseName) => $"{ToPascalCase(baseName)}Configuration";

    private static readonly HashSet<string> _keywords =
    [
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern",
        "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
        "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
        "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof",
        "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
    ];

    private static string EscapeIfKeyword(string identifier) => _keywords.Contains(identifier) ? $"@{identifier}" : identifier;
}
