namespace LowlandTech.Components;

public abstract class TinyComponentBase : ComponentBase
{
    private static readonly ConcurrentDictionary<Type, bool> DesignableParameterCache = new();
    private static readonly ConcurrentDictionary<string, WeakReference<TinyComponentBase>> DesignableInstances = new();
    private readonly string _designInstanceId = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

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

    protected bool HasPropertyGridParameters =>
        DesignableParameterCache.GetOrAdd(GetType(), HasDesignableParameters);

    protected bool EffectiveDesignMode => IsDesignMode || CascadedDesignMode;

    protected bool IsDesignSelectable => EffectiveDesignMode && HasPropertyGridParameters;

    protected bool EffectiveDesignSelected =>
        IsDesignSelected || string.Equals(CascadedSelectedDesignInstanceId, _designInstanceId, StringComparison.Ordinal);

    protected string DesignModeClass => IsDesignSelectable
        ? EffectiveDesignSelected
            ? "lt-design-selectable lt-design-selected"
            : "lt-design-selectable"
        : string.Empty;

    protected string DesignDisabledClass => IsDisabledInDesignMode ? "lt-design-disabled" : string.Empty;

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
                GetType(),
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
        if (IsDesignSelectable)
        {
            DesignableInstances[_designInstanceId] = new WeakReference<TinyComponentBase>(this);
        }
    }

    protected override void OnParametersSet()
    {
        if (!EffectiveDesignMode ||
            !EffectiveDesignSelected ||
            DesignPropertyValues is null ||
            DesignPropertyValues.Count == 0)
        {
            return;
        }

        ApplyDesignPropertyValues();
    }

    [JSInvokable]
    public static Task OnDesignElementClicked(string instanceId)
    {
        if (DesignableInstances.TryGetValue(instanceId, out var weakRef))
        {
            if (weakRef.TryGetTarget(out var instance))
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
