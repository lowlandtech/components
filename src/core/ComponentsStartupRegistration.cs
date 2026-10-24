namespace LowlandTech.Components.Core;

/// <summary>
/// The startup registration a package contributes to a host: the logical key
/// the package owns and the symbolic identities it binds.
///
/// <para>
/// A base library registers its logical key so that an opt-in feature can
/// declare a logical dependency on it. A feature package registers its own
/// logical key together with the components it contributes.
/// </para>
/// </summary>
/// <param name="LogicalKey">The logical key the package owns.</param>
/// <param name="Bindings">The symbolic identities the package binds.</param>
public sealed record ComponentsStartupRegistration(
    string LogicalKey,
    IReadOnlyList<ComponentsStartupBinding> Bindings);
