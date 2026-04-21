namespace LowlandTech.Components.Feed;

/// <summary>
/// Marks a parameterless method to run during Blazor's <c>OnInitialized</c>
/// lifecycle event on a <see cref="BlockPageBase"/>-derived component.
/// Reflection discovers every method carrying this attribute (across all
/// partial classes — generated + <c>.Custom.cs</c>) and invokes them in
/// declaration order after <c>base.OnInitialized()</c>.
/// </summary>
/// <remarks>
/// Declare methods as <c>private void</c> with no parameters. Exceptions
/// propagate — the host's error boundary handles them.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OnInitializedAttribute : Attribute { }

/// <summary>Marks a parameterless method to run during Blazor's
/// <c>OnParametersSet</c> lifecycle.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OnParametersSetAttribute : Attribute { }

/// <summary>Marks a parameterless method to run when the page's backing
/// <c>Store&lt;T&gt;</c> raises its <c>StateChanged</c> event. The base's
/// <see cref="BlockPageBase.RaiseStateChanged"/> calls every hooked method
/// THEN schedules <c>StateHasChanged()</c>.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OnStateChangedAttribute : Attribute { }

/// <summary>Marks a parameterless method to run during <c>OnAfterRender</c>.
/// Hooks are invoked AFTER base lifecycle work and only on the main-thread
/// render callback — they don't receive <c>firstRender</c>; filter via
/// component state if needed.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OnAfterRenderAttribute : Attribute { }

/// <summary>Marks a parameterless method to run during <see cref="IDisposable.Dispose"/>.
/// Use for store unsubscriptions, JS interop teardown, timer cancellation —
/// anything that pairs with an <see cref="OnInitializedAttribute"/> setup.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OnDisposeAttribute : Attribute { }
