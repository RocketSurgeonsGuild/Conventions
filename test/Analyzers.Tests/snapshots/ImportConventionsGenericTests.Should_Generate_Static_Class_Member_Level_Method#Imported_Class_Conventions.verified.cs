﻿//HintName: Rocket.Surgery.Conventions.Analyzers/Rocket.Surgery.Conventions.ConventionAttributesGenerator/Imported_Class_Conventions.cs
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
        public static IEnumerable<IConventionWithDependencies> GetConventions(IServiceProviderDictionary serviceProvider)
        {
            foreach (var convention in Dep1.Dep1Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in Dep2Exports.GetConventions(serviceProvider))
                yield return convention;
            foreach (var convention in SampleDependencyThree.Conventions.Exports.GetConventions(serviceProvider))
                yield return convention;
        }
    }
}