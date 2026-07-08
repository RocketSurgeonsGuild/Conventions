using System.Globalization;

using Microsoft.Extensions.Configuration;
using Tomlyn.Model;

namespace Clavus.Configuration.Toml;

internal sealed class TomlConfigurationStreamParser
{
    private readonly IDictionary<string, string?> _data = new SortedDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _context = new();
    private string _currentPath = "";

    public IDictionary<string, string?> Parse(Stream input)
    {
        _data.Clear();
        _context.Clear();

        using var reader = new StreamReader(input, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        var text = reader.ReadToEnd();

        var table = Tomlyn.Toml.ToModel<TomlTable>(text);
        if (table is not null) VisitTomlTable(table);

        return _data;
    }

    private void VisitTomlTable(TomlTable table)
    {
        foreach (var pair in table)
        {
            VisitValue(pair.Key, pair.Value);
        }
    }

    private void VisitValue(string context, object? value)
    {
        switch (value)
        {
            case TomlTable table:
                EnterContext(context);
                VisitTomlTable(table);
                ExitContext();
                break;
            case TomlTableArray tableArray:
                EnterContext(context);
                for (var i = 0; i < tableArray.Count; i++)
                {
                    VisitValue(i.ToString(CultureInfo.InvariantCulture), tableArray[i]);
                }
                ExitContext();
                break;
            case TomlArray array:
                EnterContext(context);
                for (var i = 0; i < array.Count; i++)
                {
                    VisitValue(i.ToString(CultureInfo.InvariantCulture), array[i]);
                }
                ExitContext();
                break;
            default:
                VisitScalar(context, value);
                break;
        }
    }

    private void VisitScalar(string context, object? value)
    {
        EnterContext(context);
        var currentKey = _currentPath;

        if (_data.ContainsKey(currentKey)) throw new FormatException($"A duplicate key '{currentKey}' was found.");

        _data[currentKey] = ConvertScalar(value);
        ExitContext();
    }

    private static string? ConvertScalar(object? value) => value switch
    {
        null => null,
        string s => s,
        bool b => b ? "true" : "false",
        long l => l.ToString(CultureInfo.InvariantCulture),
        double d => d.ToString("R", CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString(),
    };

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
}
