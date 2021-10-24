namespace Sample.Core.Databases;

#region codeblock

public interface IDatabaseConfigurator
{
    void AddTable(string name);
    void AddView(string name);
}

#endregion
