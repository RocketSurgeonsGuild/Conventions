using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Clavus.Configuration;

/// <summary>
///     Registration hooks used by generated managed-configuration wiring (see the
///     <c>clavus-managed-configuration</c> feature's <c>IConfigurationPart</c> emission) to bind a
///     strongly-typed generated configuration class to the standard
///     <see cref="Microsoft.Extensions.Options" /> pipeline.
/// </summary>
/// <remarks>
///     Generated per-config-file registration code should call
///     <see cref="AddClavusConfigurationOptions{TOptions}" /> rather than hand-rolling
///     <c>AddOptions{TOptions}()</c> binding, so reload wiring stays centralized and consistent
///     across every generated part, and so this is the single integration surface the generator
///     needs to target once the codegen (tasks 3-4 of the feature) lands.
/// </remarks>
[PublicAPI]
public static class ClavusConfigurationOptionsExtensions
{
    /// <summary>
    ///     Binds <typeparamref name="TOptions" /> to the configuration section at
    ///     <paramref name="sectionKey" /> via the standard <see cref="IOptions{TOptions}" /> pipeline.
    /// </summary>
    /// <remarks>
    ///     <see cref="OptionsBuilder{TOptions}" />.Bind(<see cref="IConfiguration" />) — from
    ///     <c>Microsoft.Extensions.Options.ConfigurationExtensions</c> — registers both the
    ///     <see cref="IConfigureOptions{TOptions}" /> binder <b>and</b> the
    ///     <see cref="IOptionsChangeTokenSource{TOptions}" /> that watches
    ///     <see cref="IConfiguration.GetReloadToken" />. That single call is what makes
    ///     <see cref="IOptionsMonitor{TOptions}" />/<see cref="IOptionsSnapshot{TOptions}" /> observe
    ///     changes (e.g. a file-system change on a source registered with <c>reloadOnChange: true</c>)
    ///     without any additional wiring — there is intentionally no separate, hand-rolled
    ///     <see cref="IOptionsChangeTokenSource{TOptions}" /> registration here.
    /// </remarks>
    /// <typeparam name="TOptions">The generated configuration class type.</typeparam>
    /// <param name="services">The service collection to register against.</param>
    /// <param name="configuration">The root configuration to bind from.</param>
    /// <param name="sectionKey">The configuration section key that maps to <typeparamref name="TOptions" />.</param>
    /// <returns>The <see cref="OptionsBuilder{TOptions}" /> for further configuration.</returns>
    /// <remarks>
    ///     <para>
    ///         This calls into <c>Microsoft.Extensions.Options.ConfigurationExtensions</c>'
    ///         reflection-based <c>ConfigurationBinder</c>, so it carries the same trimming/AOT
    ///         caveats as any other <c>IConfiguration</c> binding call — the attributes below make
    ///         that explicit rather than silently swallowing the warning. Fully trim-safe binding
    ///         (e.g. via <c>Microsoft.Extensions.Configuration.Binder.SourceGeneration</c> emitted
    ///         directly by the generator) is tracked as a follow-up once the codegen stage lands;
    ///         it does not change this method's public signature.
    ///     </para>
    /// </remarks>
    [RequiresUnreferencedCode("Binds TOptions via reflection-based ConfigurationBinder; TOptions's members must be preserved.")]
    [RequiresDynamicCode("Binds TOptions via reflection-based ConfigurationBinder, which may require runtime code generation.")]
    public static OptionsBuilder<TOptions> AddClavusConfigurationOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionKey
    )
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(sectionKey);

        return services
              .AddOptions<TOptions>()
              .Bind(configuration.GetSection(sectionKey));
    }
}
