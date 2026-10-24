namespace LowlandTech.Components.Core;

/// <summary>
/// One trusted startup binding: the logical key that owns a symbolic identity
/// and the CLR type that identity resolves to.
///
/// <para>
/// Bindings are startup metadata. They are captured when the host starts and
/// never loaded, inspected or resolved at runtime, so a symbolic identity only
/// ever becomes a type through a binding the host itself registered.
/// </para>
/// </summary>
/// <param name="LogicalKey">The logical key that owns the identity.</param>
/// <param name="SymbolicIdentity">The symbolic identity of the component.</param>
/// <param name="ComponentType">The type the identity resolves to.</param>
public sealed record ComponentsStartupBinding(
    string LogicalKey,
    string SymbolicIdentity,
    Type ComponentType);
