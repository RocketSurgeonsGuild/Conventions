// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

using Tomlyn;

namespace Clavus.Configuration.Toml;

/// <summary>
///     Extension methods for adding <see cref="TomlConfigurationExtensions" />.
/// </summary>
public static class TomlConfigurationExtensions
{
    /// <summary>
    ///     Adds the TOML configuration provider at <paramref name="path" /> to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="path">
    ///     Path relative to the base path stored in
    ///     <see cref="IConfigurationBuilder.Properties" /> of <paramref name="builder" />.
    /// </param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddTomlFile(this IConfigurationBuilder builder, string path) => AddTomlFile(builder, null, path, false, false);

    /// <summary>
    ///     Adds the TOML configuration provider at <paramref name="path" /> to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="path">
    ///     Path relative to the base path stored in
    ///     <see cref="IConfigurationBuilder.Properties" /> of <paramref name="builder" />.
    /// </param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddTomlFile(this IConfigurationBuilder builder, string path, bool optional) => AddTomlFile(builder, null, path, optional, false);

    /// <summary>
    ///     Adds the TOML configuration provider at <paramref name="path" /> to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="path">
    ///     Path relative to the base path stored in
    ///     <see cref="IConfigurationBuilder.Properties" /> of <paramref name="builder" />.
    /// </param>
    /// <param name="optional">Whether the file is optional.</param>
    /// <param name="reloadOnChange">Whether the configuration should be reloaded if the file changes.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddTomlFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange) => AddTomlFile(builder, null, path, optional, reloadOnChange);

    /// <summary>
    ///     Adds a TOML configuration source to <paramref name="builder" />.
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
    public static IConfigurationBuilder AddTomlFile(
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
            : builder.AddTomlFile(
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
    ///     Adds a TOML configuration source to <paramref name="builder" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="configureSource">Configures the source.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddTomlFile(this IConfigurationBuilder builder, Action<TomlConfigurationSource> configureSource) => builder.Add(configureSource);

    /// <summary>
    ///     Adds a TOML configuration source to <paramref name="builder" /> that reads from a <see cref="Stream" />.
    /// </summary>
    /// <param name="builder">The <see cref="IConfigurationBuilder" /> to add to.</param>
    /// <param name="stream">The <see cref="Stream" /> to read the toml configuration data from.</param>
    /// <returns>The <see cref="IConfigurationBuilder" />.</returns>
    public static IConfigurationBuilder AddTomlStream(this IConfigurationBuilder builder, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Add(CreateTomlConfigurationSource(stream));
    }

    internal static IConfigurationSource CreateTomlConfigurationSource(Stream stream)
    {
        return new StaticConfigurationSource
        {
            Data = readStream(stream),
        };

        static IDictionary<string, string?> readStream(Stream s)
        {
            try
            {
                return new TomlConfigurationStreamParser().Parse(s);
            }
            catch (TomlException e)
            {
                throw new FormatException($"Could not parse the TOML file: {e.Message}.", e);
            }
        }
    }
}
