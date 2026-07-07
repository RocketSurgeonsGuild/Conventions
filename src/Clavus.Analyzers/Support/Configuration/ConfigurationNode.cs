using System.Globalization;

namespace Clavus.Support.Configuration;

/// <summary>
///     A node in the shape tree reconstructed from a configuration file's flattened key/value pairs. Object
///     property names become <see cref="Children" /> keyed by their original (non-PascalCase) segment name;
///     a node whose children are all numeric-index segments (<c>"0"</c>, <c>"1"</c>, ...) is treated as an array.
/// </summary>
internal sealed class ConfigurationNode
{
    private readonly Dictionary<string, ConfigurationNode> _children = [with(StringComparer.OrdinalIgnoreCase)];

    /// <summary>The raw value for a leaf node (no children). <see langword="null" /> for object/array nodes.</summary>
    public string? LeafValue { get; set; }

    /// <summary>Whether this node has any children (i.e. is an object or array, not a scalar leaf).</summary>
    public bool IsLeaf => _children.Count == 0;

    /// <summary>Whether every child segment parses as a non-negative integer index, i.e. this node is an array.</summary>
    public bool IsArray => _children.Count > 0 && _children.Keys.All(k => int.TryParse(k, NumberStyles.None, CultureInfo.InvariantCulture, out _));

    /// <summary>Child nodes keyed by their original path segment, in first-seen order.</summary>
    public IReadOnlyDictionary<string, ConfigurationNode> Children => _children;

    public ConfigurationNode GetOrAddChild(string segment)
    {
        if (_children.TryGetValue(segment, out var existing)) return existing;
        var child = new ConfigurationNode();
        _children[segment] = child;
        return child;
    }

    /// <summary>
    ///     Builds a shape tree from a configuration file's flattened key/value pairs (see
    ///     <see cref="JsonFlatConfigurationReader" />). Colon (<c>:</c>) is the path separator, matching
    ///     <see cref="Microsoft.Extensions.Configuration.IConfiguration" /> key conventions.
    /// </summary>
    public static ConfigurationNode Build(IEnumerable<KeyValuePair<string, string?>> flatValues)
    {
        var root = new ConfigurationNode();
        foreach (var (key, value) in flatValues)
        {
            if (string.IsNullOrEmpty(key)) continue;

            var segments = key.Split(':');
            var node = root;
            foreach (var segment in segments)
            {
                node = node.GetOrAddChild(segment);
            }

            node.LeafValue = value;
        }

        return root;
    }
}
