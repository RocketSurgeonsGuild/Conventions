using System;
using Rocket.Surgery.Conventions;
using Sample.DependencyTwo;

[assembly: Convention(typeof(Class2))]

namespace Sample.DependencyTwo
{
    public class Class2 : IConvention
    {
    }
}
