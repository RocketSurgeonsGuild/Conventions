// ReSharper disable once CheckNamespace

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.CommandLine;
using Rocket.Surgery.Conventions.Configuration;

// ReSharper disable once CheckNamespace
namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary>
    /// Extension method to apply service conventions
    /// </summary>
    public static class RocketSurgeryCommandLineExtensions
    {
        /// <summary>
        /// Apply configuration conventions
        /// </summary>
        /// <param name="conventionContext"></param>
        /// <param name="entryAssembly"></param>
        /// <returns></returns>
        public static ICommandLine CreateCommandLine(this IConventionContext conventionContext, Assembly? entryAssembly = null)
        {
            entryAssembly ??= Assembly.GetEntryAssembly()!;
            var commandLineApplication = new CommandLineApplication<ApplicationState>
            {
                UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
            };

            var context = new CommandLineContext(commandLineApplication);
            foreach (var item in conventionContext.Conventions.Get<ICommandLineConvention, CommandLineConvention>())
            {
                if (item is ICommandLineConvention convention)
                {
                    convention.Register(conventionContext, context);
                }
                else if (item is CommandLineConvention @delegate)
                {
                    @delegate(conventionContext, context);
                }
            }

            commandLineApplication
               .Conventions
               .UseAttributes()
               .SetAppNameFromEntryAssembly()
               .SetRemainingArgsPropertyOnModel()
               .SetSubcommandPropertyOnModel()
               .SetParentPropertyOnModel()
                //.UseOnExecuteMethodFromModel()
               .UseOnValidateMethodFromModel()
               .UseOnValidationErrorMethodFromModel()
               .AddConvention(new DefaultHelpOptionConvention())
               .AddConvention(new VersionConvention(entryAssembly))
               .AddConvention(
                    new ActivatorUtilitiesConvention(
                        new CommandLineServiceProvider(commandLineApplication)
                    )
                );

            return new CommandLine(commandLineApplication, conventionContext.Logger);
        }
    }
}