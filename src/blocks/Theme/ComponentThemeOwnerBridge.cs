namespace LowlandTech.Components;

/// <summary>
/// The host authority over the plugin owners that contribute themes, at one
/// revision.
/// </summary>
/// <param name="Revision">The revision of the authority.</param>
/// <param name="ActiveOwnerKeys">The owners that are active in this revision.</param>
public sealed record ComponentThemeOwnerSnapshot(
    long Revision,
    IReadOnlyCollection<string> ActiveOwnerKeys);

/// <summary>
/// Projects the themes of the active plugin owners onto the provider scoped
/// resolver of the host.
/// </summary>
/// <remarks>
/// <para>
/// An installed theme is not proof of active ownership. The resolver of the
/// host finds a theme by its name, so the theme of an owner that withdrew stays
/// reachable as long as a provider selects it. This bridge closes that path: a
/// binding is validated before it is published, only the bindings of the
/// owners of the revision the bridge holds are published, and the withdrawal
/// of an owner restores the theme every provider had before it selected the
/// theme of that owner.
/// </para>
/// <para>
/// The bridge changes nothing of the resolver: the class of the theme replaces,
/// prepends or appends to the default class of the component, and the class of
/// the consumer and the class of the design mode stay last, in that order. The
/// base DI-only mode keeps registering the host themes directly and stays
/// unchanged.
/// </para>
/// </remarks>
public sealed class ComponentThemeOwnerBridge
{
    private readonly ComponentThemeService _service;
    private readonly Dictionary<string, ComponentThemeBinding> _bindings = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Selection> _selections = new(StringComparer.Ordinal);
    private ComponentThemeOwnerSnapshot _snapshot;

    /// <summary>
    /// Creates the bridge over the resolver of the host.
    /// </summary>
    /// <param name="service">The resolver of the host.</param>
    /// <param name="snapshot">The authority the bridge projects.</param>
    public ComponentThemeOwnerBridge(ComponentThemeService service, ComponentThemeOwnerSnapshot snapshot)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
    }

    /// <summary>The revision the bridge currently projects.</summary>
    public long Revision => _snapshot.Revision;

    /// <summary>The authority the bridge currently projects.</summary>
    public ComponentThemeOwnerSnapshot Snapshot => _snapshot;

    /// <summary>
    /// Whether the owner is active in the revision the bridge projects.
    /// </summary>
    public bool IsOwnerActive(string ownerKey) =>
        _snapshot.ActiveOwnerKeys.Contains(ownerKey, StringComparer.Ordinal);

    /// <summary>
    /// Publishes a binding after it has been validated. A refused binding
    /// changes nothing: the published bindings and the provider selections stay
    /// exactly as they were.
    /// </summary>
    /// <param name="binding">The binding the owner contributes.</param>
    /// <returns>The validation of the binding.</returns>
    public ComponentThemeBindingValidation Publish(ComponentThemeBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        var validation = Validate(binding);

        if (validation.IsValid)
        {
            _bindings[Key(binding.ProviderId, binding.ThemeKey)] = binding;
        }

        return validation;
    }

    /// <summary>
    /// Delivers the next revision of the host authority. A provider that had
    /// selected the theme of an owner that is no longer active falls back to
    /// the theme it had before, and loses the stylesheet selection of that
    /// owner.
    /// </summary>
    /// <param name="snapshot">The authority of the next revision.</param>
    public void Deliver(ComponentThemeOwnerSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        _snapshot = snapshot;

        foreach (var providerId in _selections.Keys.ToArray())
        {
            var selection = _selections[providerId];

            if (IsOwnerActive(selection.OwnerKey))
            {
                continue;
            }

            // The withdrawal restores the theme the provider had before it
            // selected the theme of the owner, or the host default when it had
            // none. The restoration goes through Set, so the provider boundary
            // is told about the change and stops rendering the withdrawn theme.
            var restored = selection.PreviousThemeName
                ?? _service.All.FirstOrDefault()?.Name;

            if (string.IsNullOrEmpty(restored))
            {
                _service.Unregister(providerId);
            }
            else
            {
                _service.Set(providerId, restored);
            }

            _selections.Remove(providerId);
        }
    }

    /// <summary>
    /// The bindings the bridge publishes for the owners of the revision it
    /// holds.
    /// </summary>
    public IReadOnlyList<ComponentThemeBinding> Enumerate() =>
        _bindings.Values
            .Where(binding => IsOwnerActive(binding.OwnerKey))
            .ToList();

    /// <summary>
    /// Selects the theme of a provider through the bridge, so the theme of an
    /// owner that is not active can never be selected.
    /// </summary>
    /// <param name="providerId">The identifier of the provider boundary.</param>
    /// <param name="themeKey">The key of the theme inside its owner.</param>
    /// <returns>Whether the theme was selected.</returns>
    public bool Select(string providerId, string themeKey)
    {
        if (!_bindings.TryGetValue(Key(providerId, themeKey), out var binding))
        {
            return false;
        }

        if (!IsOwnerActive(binding.OwnerKey))
        {
            return false;
        }

        if (!_selections.TryGetValue(providerId, out var selection))
        {
            selection = new Selection(
                binding.OwnerKey,
                _service.GetNameForProvider(providerId),
                binding.ThemeKey);

            _selections[providerId] = selection;
        }

        _service.Set(providerId, binding.Theme.Name);

        return true;
    }

    /// <summary>
    /// The stylesheet selection of a provider: the stylesheets of the theme the
    /// provider selected, and none when it selected no theme.
    /// </summary>
    /// <param name="providerId">The identifier of the provider boundary.</param>
    public IReadOnlyList<string> StylesheetsFor(string providerId) =>
        _selections.TryGetValue(providerId, out var selection)
        && _bindings.TryGetValue(Key(providerId, selection.ThemeKey), out var binding)
            ? binding.Stylesheets
            : [];

    private ComponentThemeBindingValidation Validate(ComponentThemeBinding binding)
    {
        var errors = new List<ComponentThemeBindingError>();

        if (!IsOwnerActive(binding.OwnerKey))
        {
            errors.Add(new ComponentThemeBindingError(
                ComponentThemeBindingErrorCode.InactiveOwner,
                binding.OwnerKey,
                $"The owner '{binding.OwnerKey}' is not active, so the binding is excluded before publication."));
        }

        if (string.IsNullOrWhiteSpace(binding.Theme?.Name))
        {
            errors.Add(new ComponentThemeBindingError(
                ComponentThemeBindingErrorCode.MissingThemeName,
                binding.ThemeKey,
                "The theme has no name, so a provider cannot select it."));
        }

        if (binding.Theme is IThemeStyleFamily family && string.IsNullOrWhiteSpace(family.StyleFamily))
        {
            errors.Add(new ComponentThemeBindingError(
                ComponentThemeBindingErrorCode.InvalidStyleFamily,
                binding.ThemeKey,
                "The style family of the theme is empty."));
        }

        if (binding.Theme is IComponentClassTheme classTheme)
        {
            foreach (var slot in classTheme.ClassSlots)
            {
                if (!Enum.IsDefined(slot.Value.MergeMode))
                {
                    errors.Add(new ComponentThemeBindingError(
                        ComponentThemeBindingErrorCode.UndefinedMergeMode,
                        $"{slot.Key.Component}.{slot.Key.Slot}",
                        $"The class slot '{slot.Key.Component}.{slot.Key.Slot}' uses a merge mode that does not exist."));
                }
            }
        }

        foreach (var stylesheet in binding.Stylesheets ?? [])
        {
            if (!IsLocalStylesheet(stylesheet))
            {
                errors.Add(new ComponentThemeBindingError(
                    ComponentThemeBindingErrorCode.InvalidStylesheetPath,
                    stylesheet,
                    $"The stylesheet '{stylesheet}' is not a local package CSS path."));
            }
        }

        if (_bindings.Values.Any(published =>
                string.Equals(published.ProviderId, binding.ProviderId, StringComparison.Ordinal)
                && string.Equals(published.ThemeKey, binding.ThemeKey, StringComparison.Ordinal)))
        {
            errors.Add(new ComponentThemeBindingError(
                ComponentThemeBindingErrorCode.DuplicateBinding,
                binding.ThemeKey,
                $"The provider '{binding.ProviderId}' already publishes the theme '{binding.ThemeKey}'."));
        }

        return new ComponentThemeBindingValidation(binding, errors.Count == 0, errors);
    }

    /// <summary>
    /// A stylesheet of a theme is a local package CSS path: relative, without
    /// traversal, under <c>_content</c>, and a CSS file.
    /// </summary>
    private static bool IsLocalStylesheet(string path) =>
        !string.IsNullOrEmpty(path)
        && path.Split('/').All(part => part.Length > 0 && part is not "." and not ".."
            && part.All(character => char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.'))
        && path.StartsWith("_content/", StringComparison.Ordinal)
        && path.Split('/').Length >= 3
        && path.EndsWith(".css", StringComparison.OrdinalIgnoreCase);

    private static string Key(string providerId, string themeKey) =>
        $"{providerId}{themeKey}";

    /// <summary>
    /// The selection of a provider: the owner that contributed the theme and
    /// the theme the provider had before.
    /// </summary>
    private sealed record Selection(string OwnerKey, string? PreviousThemeName, string ThemeKey);
}
