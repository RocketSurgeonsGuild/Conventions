using System.Globalization;
using System.Text.RegularExpressions;

namespace Clavus.Support.Configuration;

/// <summary>
///     The inferred shape of a leaf configuration value, in BCL terms. The emitter maps these to NodaTime
///     equivalents when NodaTime type-mode is active (see <see cref="ConfigurationValueTypeInference.ToClrTypeName" />).
/// </summary>
internal enum InferredValueKind
{
    TimeSpan,
    DateOnly,
    TimeOnly,
    DateTimeOffset,
    Bool,
    Int,
    Long,
    Double,
    String,
}

/// <summary>
///     Implements the type-inference precedence chain from design.md Decision 3: given a raw config value's
///     string representation, the first strict, anchored shape match wins, in the order
///     <c>TimeSpan -&gt; DateOnly -&gt; TimeOnly -&gt; DateTimeOffset -&gt; primitive fallback</c>.
/// </summary>
/// <remarks>
///     Validation deliberately avoids the BCL <c>DateOnly</c>/<c>TimeOnly</c> types themselves: this generator
///     targets <c>netstandard2.0</c> (the Roslyn analyzer host), where those types don't exist. Shape validity is
///     instead checked with <see cref="DateTime" />/<see cref="TimeSpan" />/<see cref="DateTimeOffset" />, which is
///     semantically equivalent for the purpose of rejecting out-of-range components (e.g. an hour of 24). The
///     generated *code* still emits the real <c>System.DateOnly</c>/<c>System.TimeOnly</c> type names as text - it
///     is the consuming project (not the generator) that needs those types available at its own target framework.
/// </remarks>
internal static class ConfigurationValueTypeInference
{
    // 'd.hh:mm:ss[.fffffff]' or 'hh:mm:ss[.fffffff]' - duration shape: colon-delimited with a seconds component and
    // no 'T' date/time separator and no timezone offset. Anchored so "24:00" (2 segments) never matches here.
    private static readonly Regex _timeSpanPattern = new(
        @"^(?<days>\d{1,8}\.)?\d{1,2}:\d{2}:\d{2}(\.\d{1,7})?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    // ISO 8601 date-only: yyyy-MM-dd.
    private static readonly Regex _dateOnlyPattern = new(
        @"^\d{4}-\d{2}-\d{2}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    // HH:mm[:ss[.fff]] with no date component - anchored so it never overlaps the TimeSpan/DateTimeOffset shapes.
    private static readonly Regex _timeOnlyPattern = new(
        @"^\d{2}:\d{2}(:\d{2}(\.\d{1,7})?)?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    // ISO 8601 date + time, optional fractional seconds, optional 'Z' or +/-HH:mm offset.
    private static readonly Regex _dateTimeOffsetPattern = new(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(:\d{2}(\.\d{1,7})?)?(Z|[+-]\d{2}:\d{2})?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    /// <summary>
    ///     Infers the shape of <paramref name="rawValue" /> using the strict, anchored precedence chain.
    ///     A <see langword="null" /> value (JSON <c>null</c>) is treated as <see cref="InferredValueKind.String" />
    ///     - there is no shape to infer from an absent value.
    /// </summary>
    public static InferredValueKind Infer(string? rawValue)
    {
        if (string.IsNullOrEmpty(rawValue)) return InferredValueKind.String;

        if (_timeSpanPattern.IsMatch(rawValue) && TimeSpan.TryParse(rawValue, CultureInfo.InvariantCulture, out _))
            return InferredValueKind.TimeSpan;

        if (_dateOnlyPattern.IsMatch(rawValue)
         && DateTime.TryParseExact(rawValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            return InferredValueKind.DateOnly;
        }

        if (_timeOnlyPattern.IsMatch(rawValue)
         && DateTime.TryParseExact(
                rawValue,
                ["HH:mm", "HH:mm:ss", "HH:mm:ss.f", "HH:mm:ss.ff", "HH:mm:ss.fff", "HH:mm:ss.ffff", "HH:mm:ss.fffff", "HH:mm:ss.ffffff", "HH:mm:ss.fffffff",],
                CultureInfo.InvariantCulture,
                DateTimeStyles.NoCurrentDateDefault,
                out _
            ))
        {
            return InferredValueKind.TimeOnly;
        }

        if (_dateTimeOffsetPattern.IsMatch(rawValue)
         && DateTimeOffset.TryParse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            return InferredValueKind.DateTimeOffset;
        }

        if (bool.TryParse(rawValue, out _)) return InferredValueKind.Bool;
        return  int.TryParse(rawValue, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _) 
            ?  InferredValueKind.Int 
            :  long.TryParse(rawValue, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _)
            ? InferredValueKind.Long
            : double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
            ? InferredValueKind.Double
            : InferredValueKind.String;
    }

    /// <summary>
    ///     Maps an inferred kind to the fully-qualified CLR type name to emit, substituting the NodaTime
    ///     equivalents for the four date/time/duration kinds when <paramref name="useNodaTime" /> is
    ///     <see langword="true" /> (design.md Decision 3).
    /// </summary>
    public static string ToClrTypeName(InferredValueKind kind, bool useNodaTime) =>
        kind switch
        {
            InferredValueKind.TimeSpan => useNodaTime ? "global::NodaTime.Duration" : "global::System.TimeSpan",
            InferredValueKind.DateOnly => useNodaTime ? "global::NodaTime.LocalDate" : "global::System.DateOnly",
            InferredValueKind.TimeOnly => useNodaTime ? "global::NodaTime.LocalTime" : "global::System.TimeOnly",
            InferredValueKind.DateTimeOffset => useNodaTime ? "global::NodaTime.OffsetDateTime" : "global::System.DateTimeOffset",
            InferredValueKind.Bool => "bool",
            InferredValueKind.Int => "int",
            InferredValueKind.Long => "long",
            InferredValueKind.Double => "double",
            InferredValueKind.String => "string",
            _ => "string",
        };
}
