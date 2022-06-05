//HintName: Imported_Class_Conventions.cs
using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions;

namespace TestProject
{
    public partial class Program
    {
        /// <summary>
        /// The conventions imported into this assembly
        /// </summary>
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProvider serviceProvider)
        {
            foreach (var convention in TestProject.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}