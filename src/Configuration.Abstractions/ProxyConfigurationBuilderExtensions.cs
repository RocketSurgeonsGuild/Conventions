using System;
using System.Collections.Generic;
using IMsftConfigurationBuilder = Microsoft.Extensions.Configuration.IConfigurationBuilder;

namespace Rocket.Surgery.Extensions.Configuration
{
    internal static class ProxyConfigurationBuilderExtensions
    {
        public static T Apply<T>(
            this T builder,
            IEnumerable<Func<IMsftConfigurationBuilder, IMsftConfigurationBuilder>> builders
        )
            where T : IMsftConfigurationBuilder
        {
            foreach (var b in builders)
            {
                b(builder);
            }

            return builder;
        }

        public static T Apply<T>(
            this T builder,
            IEnumerable<Func<IMsftConfigurationBuilder, string, IMsftConfigurationBuilder>> builders,
            string environmentName
        )
            where T : IMsftConfigurationBuilder
        {
            foreach (var b in builders)
            {
                b(builder, environmentName);
            }

            return builder;
        }
    }
}