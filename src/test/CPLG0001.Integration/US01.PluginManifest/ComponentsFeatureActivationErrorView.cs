namespace LowlandTech.Components.Tests.CPLG0001.Integration.US01.PluginManifest;

/// <summary>
/// One failure the Components feature activation reported. The subject is the
/// name the failure is reported by: the feature key, the missing logical
/// dependency key or the identity that did not resolve.
/// </summary>
/// <param name="Code">The production error code.</param>
/// <param name="Subject">The name the failure names.</param>
/// <param name="Message">The human readable failure.</param>
public sealed record ComponentsFeatureActivationErrorView(
    string Code,
    string Subject,
    string Message);
