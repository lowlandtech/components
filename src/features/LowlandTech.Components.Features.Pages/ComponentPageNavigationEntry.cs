namespace LowlandTech.Components.Features.Pages;

/// <summary>
/// A published page entry for the shell navigation.
/// </summary>
/// <param name="RoutePath">The route the entry navigates to.</param>
/// <param name="Label">The text the navigation shows.</param>
/// <param name="Icon">The logical icon name of the entry.</param>
/// <param name="Order">The sort order of the entry; lower values come first.</param>
/// <param name="Group">The optional group heading above the entry.</param>
/// <param name="SubGroup">The optional sub-group the entry is nested under.</param>
public sealed record ComponentPageNavigationEntry(
    string RoutePath,
    string Label,
    string Icon,
    int Order,
    string? Group,
    string? SubGroup);
