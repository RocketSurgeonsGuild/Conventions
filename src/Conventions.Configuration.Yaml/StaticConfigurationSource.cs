using Microsoft.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

internal class StaticConfigurationSource: IConfigurationSource
{
    public IDictionary<string, string?> Data { get; set; } = new Dictionary<string, string?>();

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new StaticConfigurationProvider(Data);
}
