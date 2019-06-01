using System;
using System.Collections.Generic;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public interface IConventionBuilder<out TBuilder, in TConvention, in TDelegate> : IConventionContainer<TBuilder, TConvention, TDelegate>
        where TBuilder : IConventionBuilder<TBuilder, TConvention, TDelegate>
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
    }
}
