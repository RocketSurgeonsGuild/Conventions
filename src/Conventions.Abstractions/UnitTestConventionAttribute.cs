using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Defines this convention as one that only runs during a unit test run
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class UnitTestConventionAttribute : Attribute, IHostBasedConvention
    {
        HostType IHostBasedConvention.HostType => HostType.UnitTestHost;
    }
}
