namespace Clavus;

/// <summary>
///     Assembly-level marker recording that this assembly declares a managed <c>ClavusConfiguration</c> file
///     (e.g. an <c>appsettings.json</c>/<c>.yaml</c>/<c>.toml</c> family file) that has been packaged for
///     distribution and/or discovered by <c>Clavus.Analyzers</c>.
/// </summary>
/// <remarks>
///     Emitted by <c>Clavus.Analyzers</c> for every project with discovered <c>ClavusConfiguration</c> items
///     (see <c>Clavus.ConfigurationAssembly.Emit</c> in the generator). A host-side generator pass reads this
///     attribute off referenced assemblies via compile-time reflection (<c>Compilation.ReferencedAssemblyNames</c>
///     / <c>IAssemblySymbol.GetAttributes()</c>) - the same mechanism as runtime
///     <see cref="System.Reflection.Assembly" />.<c>GetCustomAttributes()</c>, but at compile time - to build a
///     <c>ClavusConfigurationManifest</c> without any runtime file probing. Multiple instances are allowed on the
///     same assembly - one per contributed configuration file/base-name group.
/// </remarks>
/// <param name="name">The logical base name of the configuration group (e.g. <c>appsettings</c>).</param>
/// <param name="relativePath">
///     The path of the packaged/copied configuration file relative to the consuming application's output directory,
///     mirroring the package's <c>contentFiles</c> layout.
/// </param>
[PublicAPI]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ConfigurationAssemblyAttribute(string name, string relativePath) : Attribute
{
    /// <summary>
    ///     The logical base name of the configuration group (e.g. <c>appsettings</c>).
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    ///     The path of the packaged/copied configuration file relative to the consuming application's output
    ///     directory.
    /// </summary>
    public string RelativePath { get; } = relativePath;
}
