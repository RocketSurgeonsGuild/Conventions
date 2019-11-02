using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using IMsftConfigurationBuilder = Microsoft.Extensions.Configuration.IConfigurationBuilder;

namespace Rocket.Surgery.Extensions.Configuration
{
    class ProxyConfigurationBuilder : IMsftConfigurationBuilder
    {
        private readonly IMsftConfigurationBuilder _builder;
        private readonly IList<IConfigurationSource> _sources = new List<IConfigurationSource>();

        public ProxyConfigurationBuilder(IMsftConfigurationBuilder builder)
        {
            this._builder = builder;
        }

        public IDictionary<string, object> Properties => _builder.Properties;

        public IList<IConfigurationSource> Sources => _builder.Sources;

        public IMsftConfigurationBuilder Add(IConfigurationSource source)
        {
            _sources.Add(source);
            return this;
        }

        public IEnumerable<IConfigurationSource> GetAdditionalSources()
        {
            return _sources;
        }

        public IConfigurationRoot Build()
        {
            return _builder.Build();
        }
    }
}
