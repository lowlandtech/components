namespace LowlandTech.Components.Features.Pages;

/// <summary>
/// The owners a host currently holds active, and the revision that state belongs to.
/// </summary>
/// <param name="Revision">
/// The revision of the authority the host delivered. The bridge publishes the
/// pages of exactly this revision and nothing older or newer.
/// </param>
/// <param name="ActiveOwnerKeys">
/// The logical keys of the owners that are active in this revision. An owner
/// that is absent is withdrawn: its pages stay registered but leave every
/// published projection.
/// </param>
public sealed record ComponentPageOwnerSnapshot(
    long Revision,
    IReadOnlyCollection<string> ActiveOwnerKeys);
