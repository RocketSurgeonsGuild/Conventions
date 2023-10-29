using System.Globalization;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

internal class YamlConfigurationFileParser
{
    private readonly IDictionary<string, string?> _data = new SortedDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _context = new Stack<string>();
    // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
    private string _currentPath = null!;

    public IDictionary<string, string?> Parse(Stream input)
    {
        _data.Clear();
        _context.Clear();

        // https://dotnetfiddle.net/rrR2Bb
        var yaml = new YamlStream();
        using var reader = new StreamReader(input, true);
        yaml.Load(reader);

        if (yaml.Documents.Any())
        {
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            // The document node is a mapping node
            VisitYamlMappingNode(mapping);
        }

        return _data;
    }

    private void VisitYamlNodePair(KeyValuePair<YamlNode, YamlNode> yamlNodePair)
    {
        var context = ( (YamlScalarNode)yamlNodePair.Key ).Value;
        // ReSharper disable once NullableWarningSuppressionIsUsed RedundantSuppressNullableWarningExpression
        VisitYamlNode(context!, yamlNodePair.Value);
    }

    private void VisitYamlNode(string context, YamlNode node)
    {
        if (node is YamlScalarNode scalarNode)
        {
            VisitYamlScalarNode(context, scalarNode);
        }

        if (node is YamlMappingNode mappingNode)
        {
            VisitYamlMappingNode(context, mappingNode);
        }

        if (node is YamlSequenceNode sequenceNode)
        {
            VisitYamlSequenceNode(context, sequenceNode);
        }
    }

    private void VisitYamlScalarNode(string context, YamlScalarNode yamlValue)
    {
        //a node with a single 1-1 mapping
        EnterContext(context);
        var currentKey = _currentPath;

        if (_data.ContainsKey(currentKey))
        {
            throw new FormatException("FormatError_KeyIsDuplicated - " + currentKey);
        }

        _data[currentKey] = YamlConfigurationFileParser.IsNullValue(yamlValue) ? null : yamlValue.Value;
        ExitContext();
    }

    private void VisitYamlMappingNode(YamlMappingNode node)
    {
        foreach (var yamlNodePair in node.Children)
        {
            VisitYamlNodePair(yamlNodePair);
        }
    }

    private void VisitYamlMappingNode(string context, YamlMappingNode yamlValue)
    {
        //a node with an associated sub-document
        EnterContext(context);

        VisitYamlMappingNode(yamlValue);

        ExitContext();
    }

    private void VisitYamlSequenceNode(string context, YamlSequenceNode yamlValue)
    {
        //a node with an associated list
        EnterContext(context);

        VisitYamlSequenceNode(yamlValue);

        ExitContext();
    }

    private void VisitYamlSequenceNode(YamlSequenceNode node)
    {
        for (var i = 0; i < node.Children.Count; i++)
        {
            VisitYamlNode(i.ToString(NumberFormatInfo.InvariantInfo), node.Children[i]);
        }
    }

    private void EnterContext(string context)
    {
        _context.Push(context);
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private void ExitContext()
    {
        _context.Pop();
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private static bool IsNullValue(YamlScalarNode yamlValue)
    {
        return yamlValue.Style == ScalarStyle.Plain
            && (
                   yamlValue.Value == "~"
                || yamlValue.Value == "null"
                || yamlValue.Value == "Null"
                || yamlValue.Value == "NULL"
               );
    }
}
