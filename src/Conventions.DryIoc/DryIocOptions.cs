namespace Rocket.Surgery.Conventions.DryIoc;

/// <summary>
///     Options for building the DryIoc Container
/// </summary>
[PublicAPI]
public class DryIocOptions
{
    /// <summary>
    ///     Prevents additional container registrations (defaults to true)
    /// </summary>
    public bool NoMoreRegistrationAllowed { get; set; } = true;
}
