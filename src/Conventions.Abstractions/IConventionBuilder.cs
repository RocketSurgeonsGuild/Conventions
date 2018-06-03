using System;
using Rocket.Surgery.Builders;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    public interface IConventionBuilder<out TBuilder, in TConvention, in TDelegate> : IBuilder, IConventionContainer<TBuilder, TConvention, TDelegate>
        where TBuilder : IBuilder
        where TConvention : IConvention
        where TDelegate : Delegate
    {
        IAssemblyProvider AssemblyProvider { get; }
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
    }
}
