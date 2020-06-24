using System.Reflection;

namespace Rocket.Surgery.Conventions.CommandLine
{
    /// <summary>
    /// VersionConvention.
    /// Implements the <see cref="McMaster.Extensions.CommandLineUtils.Conventions.IConvention" />
    /// </summary>
    /// <seealso cref="McMaster.Extensions.CommandLineUtils.Conventions.IConvention" />
    internal class VersionConvention : McMaster.Extensions.CommandLineUtils.Conventions.IConvention
    {
        private readonly Assembly _entryAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionConvention" /> class.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        public VersionConvention(Assembly entryAssembly) => _entryAssembly = entryAssembly;

        /// <summary>
        /// Applies the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Apply(McMaster.Extensions.CommandLineUtils.Conventions.ConventionContext context)
        {
            if (context.ModelType == null)
            {
                return;
            }

            // TODO: All tagged assembly versions
            context.Application.VersionOption(
                "--version",
                () => _entryAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version,
                () => _entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                  ?.InformationalVersion
            );
        }
    }
}