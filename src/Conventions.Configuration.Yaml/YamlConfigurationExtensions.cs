// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

using YamlDotNet.Core;

namespace Rocket.Surgery.Conventions.Configuration.Yaml;

/// <summary>
///     Extension methods for adding <see cref="YamlConfigurationExtensions" />.
/// </summary>
public static class YamlConfigurationExtensions
{
    /// <summary>
    ///     Adds the YAML configuration provider at <paramref name="path" /> to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="path">
    ///     Path relative to the base path stored in
    ///     <see cref="IConfigurationBuilder.Properties" /> of <paramref name="builder" />.
    /// </param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path) => AddYamlFile(builder, null, path, false, false);

    /// <summary>
    ///     Adds the YAML configuration provider at <paramref name="path" /> to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="path">
    ///     Path relative to the base path stored in
    ///     <see cref="IConfigurationBuilder.Properties" /> of <paramref name="builder" />.
    /// </param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional) => AddYamlFile(builder, null, path, optional, false);

    /// <summary>
    ///     Adds the YAML configuration provider at <paramref name="path" /> to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="path">
    ///     Path relative to the base path stored in
    ///     <see cref="IConfigurationBuilder.Properties" /> of <paramref name="builder" />.
    /// </param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange) => AddYamlFile(builder, null, path, optional, reloadOnChange);

    /// <summary>
    ///     Adds a YAML configuration source to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="provider">The <see cref="IFileProvider" /> to use to access the file.</param>
    /// <param name="path">
    ///     Path relative to the base path stored in
    ///     <see cref="IConfigurationBuilder.Properties" /> of <paramref name="builder" />.
    /// </param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddYamlFile(
        this IConfigurationBuilder builder,
        IFileProvider? provider,
        string path,
        bool optional,
        bool reloadOnChange
    )
    {
        ArgumentNullException.ThrowIfNull(builder);

        return string.IsNullOrEmpty(path)
            ? throw new ArgumentException("File path must be a non-empty string.", nameof(path))
            : builder.AddYamlFile(
                s =>
                {
                    s.FileProvider = provider;
                    s.Path = path;
                    s.Optional = optional;
                    s.ReloadOnChange = reloadOnChange;
                    s.ResolveFileProvider();
                }
            );
    }

    /// <summary>
    ///     Adds a YAML configuration source to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="configureSource">Configures the source.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, Action<YamlConfigurationSource> configureSource) => builder.Add(configureSource);

    /// <summary>
    ///     Adds a YAML configuration source to <paramref name="builder" /> that reads from a <see cref="Stream" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="stream">The <see cref="Stream" /> to read the yaml configuration data from.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddYamlStream(this IConfigurationBuilder builder, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Add(CreateYamlConfigurationSource(stream));
    }

    internal static IConfigurationSource CreateYamlConfigurationSource(Stream stream)
    {
        return new StaticConfigurationSource
        {
            Data = readStream(stream),
        };

        static IDictionary<string, string?> readStream(Stream s)
        {
            try
            {
                return new YamlConfigurationStreamParser().Parse(s);
            }
            catch (YamlException e)
            {
                throw new FormatException(string.Format("Could not parse the YAML file: {{0}}.", e.Message), e);
            }
        }
    }
}
