using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using IMsftConfigurationBuilder = Microsoft.Extensions.Configuration.IConfigurationBuilder;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// Options for configuring a hosting environment
    /// </summary>
    public class ConfigurationOptions
    {
        /// <summary>
        /// Additional settings providers to be inserted after the default application settings file (typically appsettings.json)
        /// </summary>
        public List<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>> ApplicationConfiguration { get; } = new List<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>>();

        /// <summary>
        /// Additional settings providers to be inserted after the default environment application settings file (typically appsettings.{env}.json)
        /// </summary>
        public List<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>> EnvironmentConfiguration { get; } = new List<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>>();
    }

    static class ProxyConfigurationBuilderExtensions
    {
        public static T Apply<T>(this T builder, IEnumerable<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>> builders) where T : IMsftConfigurationBuilder
        {
            foreach (var b in builders)
            {
                b(builder);
            }
            return builder;
        }

        public static T Apply<T>(this T builder, IEnumerable<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>> builders, string environmentName) where T : IMsftConfigurationBuilder
        {
            foreach (var b in builders)
            {
                b(builder, environmentName);
            }
            return builder;
        }
    }

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
