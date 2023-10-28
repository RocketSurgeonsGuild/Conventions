namespace Sample.Core.Databases;

internal sealed class DatabaseConfigurator : IDatabaseConfigurator
{
    public ICollection<string> Tables { get; } = new List<string>();
    public ICollection<string> Views { get; } = new List<string>();

    public void AddTable(string name)
    {
        Tables.Add(name);
    }

    public void AddView(string name)
    {
        Views.Add(name);
    }
}
