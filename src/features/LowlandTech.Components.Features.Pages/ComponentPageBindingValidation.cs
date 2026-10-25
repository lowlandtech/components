namespace LowlandTech.Components.Features.Pages;

/// <summary>
/// The outcome of validating a page candidate.
/// </summary>
/// <param name="Binding">The candidate the outcome belongs to.</param>
/// <param name="IsValid">
/// True when the candidate may be published. An invalid candidate never
/// changes the published projection.
/// </param>
/// <param name="Errors">
/// Every refusal the candidate produced. A candidate can be refused for more
/// than one reason, so a host reports all of them.
/// </param>
public sealed record ComponentPageBindingValidation(
    ComponentPageBinding Binding,
    bool IsValid,
    IReadOnlyList<ComponentPageBindingError> Errors);
