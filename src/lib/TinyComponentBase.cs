namespace LowlandTech.Components;

public abstract class TinyComponentBase : ComponentBase
{
    private static readonly ConcurrentDictionary<Type, bool> DesignableParameterCache = new();
    private static readonly ConcurrentDictionary<string, (WeakReference<TinyComponentBase> Ref, string SurfaceId, long Order)> DesignableInstances = new();
    private static long _globalRegistrationOrder;
    private readonly string _designInstanceId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
    private bool _registeredAsDesignable;

    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool IsDesignMode { get; set; }
    [Parameter] public bool IsDesignSelected { get; set; }
    [Parameter] public EventCallback<bool> IsDesignSelectedChanged { get; set; }
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [CascadingParameter(Name = "TinyDesignMode")]
    private bool CascadedDesignMode { get; set; }

    [CascadingParameter(Name = "TinyDesignPropertyValues")]
    private IReadOnlyDictionary<string, string>? DesignPropertyValues { get; set; }

    [CascadingParameter(Name = "TinySelectedDesignInstanceId")]
    private string? CascadedSelectedDesignInstanceId { get; set; }

    [CascadingParameter(Name = "TinyDesignSelected")]
    private EventCallback<TinyDesignSelection> DesignSelected { get; set; }

    /// <summary>
    /// The component type currently selected in the design surface, cascaded from
    /// <c>DesignSurface</c>. Used to reapply persisted property values after a
    /// reload when the original instance ID is no longer valid.
    /// </summary>
    [CascadingParameter(Name = "TinyDesignComponentType")]
    private Type? CascadedDesignComponentType { get; set; }

    [CascadingParameter(Name = "TinyDesignSurfaceId")]
    private string? CascadedDesignSurfaceId { get; set; }

    /// <summary>
    /// The enclosing <c>ThemeProvider</c> context, or <c>null</c> when no
    /// provider wraps this component.  Used by <see cref="ThemeClass"/> to
    /// resolve active theme class slot overrides.
    /// </summary>
    [CascadingParameter]
    protected ThemeProviderContext? ThemeContext { get; set; }

    /// <summary>
    /// The component type the descriptor registry should use when looking up
    /// or reporting this component's design-time metadata.  Defaults to
    /// <see cref="object.GetType"/> — override in generic subclasses that
    /// register a concrete non-generic proxy (e.g. <c>TreeViewDesigner</c>).
    /// </summary>
    protected virtual Type DesignType => GetType();

    protected bool HasPropertyGridParameters =>
        DesignableParameterCache.GetOrAdd(GetType(), HasDesignableParameters);

    protected bool EffectiveDesignMode => IsDesignMode || CascadedDesignMode;

    protected bool IsDesignSelectable => EffectiveDesignMode && HasPropertyGridParameters;

    protected bool EffectiveDesignSelected =>
        IsDesignSelected ||
        string.Equals(CascadedSelectedDesignInstanceId, _designInstanceId, StringComparison.Ordinal);

    protected string DesignModeClass => IsDesignSelectable
        ? EffectiveDesignSelected
            ? "lt-design-selectable lt-design-selected"
            : "lt-design-selectable"
        : string.Empty;

    protected string DesignDisabledClass => IsDisabledInDesignMode ? "lt-design-disabled" : string.Empty;

    /// <summary>
    /// Resolves the effective CSS class string for a named component slot,
    /// optionally applying the active theme's class override.
    /// </summary>
    /// <param name="component">Component name, e.g. <c>"Button"</c>.</param>
    /// <param name="slot">Slot name, e.g. <c>"Root"</c>.</param>
    /// <param name="defaultClass">The component's built-in class string.</param>
    /// <param name="variant">Active variant, or <c>null</c>.</param>
    /// <param name="size">Active size, or <c>null</c>.</param>
    /// <param name="state">Active state string, or <c>null</c>.</param>
    /// <param name="consumerClass">
    /// The value of the component's <c>Class</c> parameter.  Always placed
    /// after the resolved class, regardless of merge mode.
    /// </param>
    /// <param name="designClass">
    /// Design-mode classes from <see cref="DesignModeClass"/> /
    /// <see cref="DesignDisabledClass"/>.  Always placed last.
    /// </param>
    protected string ThemeClass(
        string component,
        string slot,
        string defaultClass,
        string? variant = null,
        string? size = null,
        string? state = null,
        string? consumerClass = null,
        string? designClass = null)
    {
        if (ThemeContext is null)
        {
            // No provider — combine defaults directly.
            return Combine(defaultClass, consumerClass, designClass);
        }

        var key = new ComponentSlotKey(component, slot, variant, size, state);
        return ThemeContext.Resolver.Class(
            ThemeContext.ProviderId,
            key,
            defaultClass,
            consumerClass,
            designClass);
    }

    /// <summary>
    /// Returns a theme-supplied fragment for the given slot, or <c>null</c>
    /// when the active theme does not override that slot.
    /// </summary>
    protected RenderFragment<ThemeFragmentContext>? ThemeFragment(
        string component,
        string slot,
        string? variant = null,
        string? size = null,
        string? state = null)
    {
        if (ThemeContext is null) return null;
        var key = new ComponentSlotKey(component, slot, variant, size, state);
        return ThemeContext.Resolver.Fragment(ThemeContext.ProviderId, key);
    }

    private static string Combine(string main, string? extra1, string? extra2)
    {
        if (string.IsNullOrEmpty(extra1) && string.IsNullOrEmpty(extra2))
            return main;
        var parts = new[] { main, extra1 ?? string.Empty, extra2 ?? string.Empty };
        return string.Join(" ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }

    protected string? DesignModeAttribute => EffectiveDesignMode ? "true" : null;

    protected string? DesignSelectableAttribute => IsDesignSelectable ? "true" : null;

    protected string? DesignSelectedAttribute => IsDesignSelectable && EffectiveDesignSelected ? "true" : null;

    protected string? DesignInstanceIdAttribute => IsDesignSelectable ? _designInstanceId : null;

    protected bool IsDisabledInDesignMode => EffectiveDesignMode && IsComponentDisabled;

    protected virtual bool IsComponentDisabled => HasDisabledAttribute;

    protected IReadOnlyDictionary<string, object>? DesignModeAdditionalAttributes
    {
        get
        {
            if (!IsDisabledInDesignMode || AdditionalAttributes is null)
                return AdditionalAttributes;

            var attributes = new Dictionary<string, object>(AdditionalAttributes, StringComparer.OrdinalIgnoreCase);
            attributes.Remove("disabled");
            attributes["aria-disabled"] = "true";
            attributes["data-disabled"] = "true";
            return attributes;
        }
    }

    protected async Task SelectInDesignModeAsync()
    {
        if (!IsDesignSelectable)
            return;

        if (!DesignSelected.HasDelegate && !IsDesignSelected)
        {
            IsDesignSelected = true;
            if (IsDesignSelectedChanged.HasDelegate)
                await IsDesignSelectedChanged.InvokeAsync(true);
        }

        if (DesignSelected.HasDelegate)
            await DesignSelected.InvokeAsync(new TinyDesignSelection(
                _designInstanceId,
                DesignType,
                CaptureDesignValues()));
    }

    private bool HasDisabledAttribute
    {
        get
        {
            if (AdditionalAttributes is null || !AdditionalAttributes.TryGetValue("disabled", out var value))
                return false;

            return value switch
            {
                null => true,
                bool b => b,
                string s => !string.Equals(s, "false", StringComparison.OrdinalIgnoreCase),
                _ => true
            };
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        TryRegister();
    }

    private void TryRegister()
    {
        if (!IsDesignSelectable || _registeredAsDesignable)
            return;
        var surfaceId = CascadedDesignSurfaceId ?? string.Empty;
        var order = Interlocked.Increment(ref _globalRegistrationOrder);
        DesignableInstances[_designInstanceId] = (new WeakReference<TinyComponentBase>(this), surfaceId, order);
        _registeredAsDesignable = true;
    }

    protected override void OnParametersSet()
    {
        // Lazy registration: if design mode just turned on (e.g. after session restore)
        // and this component hasn't registered yet, do it now.
        TryRegister();

        if (!EffectiveDesignMode ||
            !EffectiveDesignSelected ||
            DesignPropertyValues is null ||
            DesignPropertyValues.Count == 0)
        {
            return;
        }

        ApplyDesignPropertyValues();
    }

    /// <summary>
    /// Returns the instance ID of the first live registered component (in DOM/render order)
    /// whose <see cref="DesignType"/> matches <paramref name="designType"/> and that
    /// belongs to the given <paramref name="surfaceId"/>.
    /// Scoping by surface prevents instances from other story cards on the same page
    /// from being picked up. Ordering by registration sequence gives the first
    /// component in document order rather than a random hash-bucket element.
    /// </summary>
    public static string? FindFirstInstanceIdOfType(Type designType, string surfaceId)
    {
        string? bestKey = null;
        long bestOrder = long.MaxValue;

        foreach (var kvp in DesignableInstances)
        {
            var (weakRef, sid, order) = kvp.Value;
            if (sid != surfaceId) continue;
            if (order >= bestOrder) continue;
            if (!weakRef.TryGetTarget(out var instance)) continue;
            if (instance.DesignType != designType) continue;

            bestKey = kvp.Key;
            bestOrder = order;
        }

        return bestKey;
    }

    /// <summary>
    /// Returns <c>true</c> if a live component with the given <paramref name="instanceId"/>
    /// is currently registered in <see cref="DesignableInstances"/>.
    /// </summary>
    public static bool HasLiveInstance(string instanceId)
    {
        return DesignableInstances.TryGetValue(instanceId, out var entry)
            && entry.Ref.TryGetTarget(out _);
    }

    [JSInvokable]
    public static Task OnDesignElementClicked(string instanceId)
    {
        if (DesignableInstances.TryGetValue(instanceId, out var entry))
        {
            if (entry.Ref.TryGetTarget(out var instance))
            {
                return instance.SelectInDesignModeAsync();
            }
            DesignableInstances.TryRemove(instanceId, out _);
        }
        return Task.CompletedTask;
    }

    private static bool HasDesignableParameters(Type componentType) =>
        componentType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(property =>
                property.GetCustomAttribute<ParameterAttribute>(inherit: false) is not null &&
                property.GetCustomAttribute<PropertyGridAttribute>(inherit: false) is not null);

    private IReadOnlyDictionary<string, string> CaptureDesignValues()
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<ParameterAttribute>(inherit: false) is null ||
                property.GetCustomAttribute<PropertyGridAttribute>(inherit: false) is null)
            {
                continue;
            }

            values[property.Name] = property.GetValue(this)?.ToString() ?? string.Empty;
        }

        return values;
    }

    private void ApplyDesignPropertyValues()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<ParameterAttribute>(inherit: false) is null ||
                property.GetCustomAttribute<PropertyGridAttribute>(inherit: false) is null ||
                !property.CanWrite ||
                !DesignPropertyValues!.TryGetValue(property.Name, out var value))
            {
                continue;
            }

            var convertedValue = ConvertDesignValue(value, property.PropertyType);
            property.SetValue(this, convertedValue);
        }
    }

    private static object? ConvertDesignValue(string value, Type targetType)
    {
        var type = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (type == typeof(string)) return value;
        if (type == typeof(bool)) return bool.TryParse(value, out var b) && b;
        if (type.IsEnum) return Enum.Parse(type, value, ignoreCase: true);
        if (type == typeof(int)) return int.TryParse(value, CultureInfo.InvariantCulture, out var i) ? i : default(int);
        if (type == typeof(long)) return long.TryParse(value, CultureInfo.InvariantCulture, out var l) ? l : default(long);
        if (type == typeof(double)) return double.TryParse(value, CultureInfo.InvariantCulture, out var d) ? d : default(double);
        if (type == typeof(float)) return float.TryParse(value, CultureInfo.InvariantCulture, out var f) ? f : default(float);
        if (type == typeof(decimal)) return decimal.TryParse(value, CultureInfo.InvariantCulture, out var m) ? m : default(decimal);
        return value;
    }
}

public sealed record TinyDesignSelection(
    string InstanceId,
    Type ComponentType,
    IReadOnlyDictionary<string, string> Values);
