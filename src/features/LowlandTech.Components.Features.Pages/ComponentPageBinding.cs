namespace LowlandTech.Components.Features.Pages;

/// <summary>
/// A page an owner contributes to the host.
/// </summary>
/// <param name="OwnerKey">
/// The logical key of the owner that contributed the page. The owner decides
/// whether the page is published; the bridge never decides on its own.
/// </param>
/// <param name="SymbolicIdentity">
/// The symbolic identity of the component in the host's trusted startup map.
/// The bridge never resolves a component by assembly scanning.
/// </param>
/// <param name="RoutePath">
/// The route the page is served under, for example <c>/plugin/reports</c>.
/// </param>
public sealed record ComponentPageBinding(
    string OwnerKey,
    string SymbolicIdentity,
    string RoutePath);
