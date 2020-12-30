using System.Collections.Generic;

namespace Sample.Core.Databases
{
    class DatabaseConfigurator : IDatabaseConfigurator
    {
        public DatabaseConfigurator()
        {
            Tables = new List<string>();
            Views = new List<string>();
        }

        public ICollection<string> Tables { get; }
        public ICollection<string> Views { get; }

        public void AddTable(string name) => Tables.Add(name);

        public void AddView(string name) => Views.Add(name);
    }
}