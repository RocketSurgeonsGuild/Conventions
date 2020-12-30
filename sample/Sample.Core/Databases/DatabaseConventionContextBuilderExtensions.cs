using Rocket.Surgery.Conventions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sample.Core.Databases
{
    #region codeblock
    public static class DatabaseConventionContextBuilderExtensions
    {
        public static ConventionContextBuilder ConfigureDatabase([NotNull] this ConventionContextBuilder container, DatabaseConvention @delegate)
        {
            container.AppendDelegate(@delegate);
            return container;
        }

        public static ConventionContextBuilder ConfigureDatabase([NotNull] this ConventionContextBuilder container, Action<IDatabaseConfigurator> @delegate)
        {
            container.AppendDelegate(new DatabaseConvention((context, configurator) => @delegate(configurator)));
            return container;
        }
    }
    #endregion
}