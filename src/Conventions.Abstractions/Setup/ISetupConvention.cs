﻿namespace Rocket.Surgery.Conventions.Setup;

/// <summary>
///     IInitConvention
/// </summary>
[PublicAPI]
public interface ISetupConvention : IConvention
{
    /// <summary>
    ///     Initialize or configure a convention before any other convention has run against the context.
    /// </summary>
    /// <param name="context"></param>
    void Register(IConventionContext context);
}