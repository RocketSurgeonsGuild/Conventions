using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;

namespace Clavus.Support.Configuration;

/// <summary>
///     Reads a JSON configuration file into a flat, ordered map of colon-delimited keys to their raw
///     string representation - the same shape <see cref="Microsoft.Extensions.Configuration.IConfiguration" />
///     ultimately exposes values as at runtime. Object property names become path segments; array elements use
///     their zero-based index as a path segment (mirroring
///     <c>Microsoft.Extensions.Configuration.Json</c>'s own flattening behavior).
/// </summary>
internal static class JsonFlatConfigurationReader
{
    /// <summary>
    ///     Parses <paramref name="json" /> and returns its flattened key/value pairs in document order.
    /// </summary>
    /// <exception cref="JsonException">The document is not well-formed JSON.</exception>
    public static ImmutableArray<KeyValuePair<string, string?>> Read(string json)
    {
        var results = ImmutableArray.CreateBuilder<KeyValuePair<string, string?>>();
        using var document = JsonDocument.Parse(
            json,
            new() { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true, }
        );
        Walk(document.RootElement, null, results);
        return results.ToImmutable();
    }

    /// <summary>
    ///     Attempts to parse <paramref name="json" />, returning <see langword="false" /> (and an empty result)
    ///     if the document is not well-formed.
    /// </summary>
    public static bool TryRead(string json, out ImmutableArray<KeyValuePair<string, string?>> flatValues)
    {
        try
        {
            flatValues = Read(json);
            return true;
        }
        catch (JsonException)
        {
            flatValues = [];
            return false;
        }
    }

    private static void Walk(JsonElement element, string? path, ImmutableArray<KeyValuePair<string, string?>>.Builder results)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    Walk(property.Value, Combine(path, property.Name), results);
                }

                break;
            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    Walk(item, Combine(path, index.ToString(CultureInfo.InvariantCulture)), results);
                    index++;
                }

                break;
            case JsonValueKind.String:
                results.Add(new(path ?? "", element.GetString()));
                break;
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
                results.Add(new(path ?? "", element.GetRawText()));
                break;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            default:
                results.Add(new(path ?? "", null));
                break;
        }
    }

    private static string Combine(string? path, string segment) => path is { Length: > 0, } ? $"{path}:{segment}" : segment;
}
