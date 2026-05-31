namespace LowlandTech.Components;

/// <summary>
/// Reflection-based <see cref="IComponentDescriptorRegistry"/>. Sweeps
/// every loaded assembly for <see cref="TinyComponentBase"/> subclasses
/// decorated with <see cref="PropertiesAttribute"/> and builds an
/// in-memory cache of their designable properties.
///
/// Discovery is lazy on first read — assemblies that load late (plugin
/// hot-reload, lazy-loaded WASM modules) are still picked up because we
/// re-scan when the cached domain assembly count drifts.
/// </summary>
public sealed class ComponentDescriptorRegistry : IComponentDescriptorRegistry
{
    private readonly object _gate = new();
    private List<ComponentDescriptor>? _cache;
    private int _cachedAssemblyCount;

    public IReadOnlyList<ComponentDescriptor> All => Cached();

    public ComponentDescriptor? GetByElement(string? element)
    {
        if (string.IsNullOrWhiteSpace(element)) return null;
        return Cached()
            .FirstOrDefault(d => string.Equals(d.Element, element, StringComparison.OrdinalIgnoreCase));
    }

    public ComponentDescriptor? GetByType(Type componentType)
        => Cached().FirstOrDefault(d => d.ComponentType == componentType);

    public IReadOnlyList<ComponentDescriptor> GetByGroup(string group)
        => Cached()
            .Where(d => string.Equals(d.Group, group, StringComparison.OrdinalIgnoreCase))
            .ToList();

    private List<ComponentDescriptor> Cached()
    {
        // Re-discover when the loaded-assembly count changes. Cheap check
        // on every read; full scan only when something actually loaded.
        var current = AppDomain.CurrentDomain.GetAssemblies().Length;
        if (_cache is not null && _cachedAssemblyCount == current)
            return _cache;

        lock (_gate)
        {
            current = AppDomain.CurrentDomain.GetAssemblies().Length;
            if (_cache is not null && _cachedAssemblyCount == current)
                return _cache;

            _cache = Discover();
            _cachedAssemblyCount = current;
            return _cache;
        }
    }

    private static List<ComponentDescriptor> Discover()
    {
        var descriptors = new List<ComponentDescriptor>();
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = asm.GetTypes(); }
            catch (ReflectionTypeLoadException ex)
            {
                // A late-loaded plugin can have a missing optional
                // reference; skip the broken types and continue with
                // whatever loaded successfully.
                types = ex.Types.Where(t => t is not null).Cast<Type>().ToArray();
            }
            catch
            {
                continue;
            }

            foreach (var type in types)
            {
                if (type is null || type.IsAbstract) continue;
                if (!typeof(TinyComponentBase).IsAssignableFrom(type)) continue;
                var classAttr = type.GetCustomAttribute<PropertiesAttribute>(inherit: false);
                if (classAttr is null) continue;
                descriptors.Add(BuildDescriptor(type, classAttr));
            }
        }

        return descriptors
            .OrderBy(d => d.Group, StringComparer.OrdinalIgnoreCase)
            .ThenBy(d => d.Element, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static ComponentDescriptor BuildDescriptor(Type type, PropertiesAttribute classAttr)
    {
        var props = new List<PropertyDescriptor>();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var instance = TryCreateInstance(type);

        for (var i = 0; i < properties.Length; i++)
        {
            var p = properties[i];
            // Must be a Blazor [Parameter] AND carry [PropertyGrid] —
            // the gate prevents non-designable parameters (CascadingValue
            // backing fields, etc.) from leaking into the panel.
            if (p.GetCustomAttribute<ParameterAttribute>(inherit: false) is null) continue;
            var pg = p.GetCustomAttribute<PropertyGridAttribute>(inherit: false);
            if (pg is null) continue;

            var editor = pg.Editor == PropertyEditor.Inferred ? InferEditor(p) : pg.Editor;
            var options = ResolveOptions(p, pg, editor);
            var defaultValue = instance is not null ? p.GetValue(instance) : null;

            props.Add(new PropertyDescriptor(
                PropertyInfo: p,
                Name: p.Name,
                Label: pg.Label ?? Humanise(p.Name),
                Group: pg.Group,
                Editor: editor,
                ValueType: Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType,
                DefaultValue: defaultValue,
                Options: options,
                Min: double.IsNaN(pg.Min) ? null : pg.Min,
                Max: double.IsNaN(pg.Max) ? null : pg.Max,
                Step: double.IsNaN(pg.Step) ? null : pg.Step,
                Placeholder: pg.Placeholder,
                Description: pg.Description,
                Order: pg.Order,
                DeclarationIndex: i));
        }

        return new ComponentDescriptor(
            ComponentType: type,
            Element: classAttr.Element ?? type.Name,
            Group: classAttr.Group,
            Icon: classAttr.Icon,
            Description: classAttr.Description,
            Properties: props);
    }

    private static PropertyEditor InferEditor(PropertyInfo p)
    {
        var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
        if (t == typeof(bool)) return PropertyEditor.Switch;
        if (t == typeof(int) || t == typeof(long) ||
            t == typeof(double) || t == typeof(float) || t == typeof(decimal))
            return PropertyEditor.Number;
        if (t.IsEnum) return PropertyEditor.Select;
        return PropertyEditor.Text;
    }

    private static IReadOnlyList<string> ResolveOptions(PropertyInfo p, PropertyGridAttribute pg, PropertyEditor editor)
    {
        if (pg.Options is { Length: > 0 }) return IncludeEmptyOption(pg.Options, pg.EmptyOption);
        if (pg.OptionsProvider is not null) return IncludeEmptyOption(ResolveProviderOptions(pg.OptionsProvider), pg.EmptyOption);
        if (editor != PropertyEditor.Select) return [];
        var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
        if (t.IsEnum) return IncludeEmptyOption(Enum.GetNames(t), pg.EmptyOption);
        return [];
    }

    private static IReadOnlyList<string> ResolveProviderOptions(Type providerType)
    {
        var all = providerType.GetProperty("All", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) ??
            providerType.GetField("All", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);

        if (all is not IEnumerable entries)
            return [];

        var options = new List<string>();
        foreach (var entry in entries)
        {
            var name = entry?.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance)?.GetValue(entry)?.ToString();
            if (!string.IsNullOrWhiteSpace(name))
                options.Add(name);
        }

        return options;
    }

    private static IReadOnlyList<string> IncludeEmptyOption(IReadOnlyList<string> options, string? emptyOption)
    {
        if (string.IsNullOrWhiteSpace(emptyOption))
            return options;

        if (options.Any(option => string.Equals(option, emptyOption, StringComparison.OrdinalIgnoreCase)))
            return options;

        return [emptyOption, .. options];
    }

    private static object? TryCreateInstance(Type type)
    {
        // Components with a parameterless constructor seed nicely. Some
        // components have only a primary constructor with required
        // services — we silently skip default-value capture for those
        // (the Properties panel will render empty slots until the user
        // commits a value, no crash).
        try { return Activator.CreateInstance(type); }
        catch { return null; }
    }

    /// <summary>
    /// "FontSize" → "Font size", "Disabled" → "Disabled",
    /// "MaxLength" → "Max length", "IsGrid" → "Is grid".
    /// </summary>
    private static string Humanise(string pascalName)
    {
        if (string.IsNullOrEmpty(pascalName)) return pascalName;
        var sb = new System.Text.StringBuilder(pascalName.Length + 8);
        for (var i = 0; i < pascalName.Length; i++)
        {
            var c = pascalName[i];
            if (i > 0 && char.IsUpper(c) && (!char.IsUpper(pascalName[i - 1]) ||
                (i + 1 < pascalName.Length && !char.IsUpper(pascalName[i + 1]))))
            {
                sb.Append(' ');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(i == 0 ? c : char.ToLowerInvariant(c));
            }
        }
        return sb.ToString();
    }
}
