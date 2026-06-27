namespace Sample.Core.Databases;

internal sealed class DatabaseConfigurator : IDatabaseConfigurator
{
    public ICollection<string> Tables { get; } = [];
    public ICollection<string> Views { get; } = [];

    public void AddTable(string name) => Tables.Add(name);

    public void AddView(string name) => Views.Add(name);
}
