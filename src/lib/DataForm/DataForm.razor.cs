namespace LowlandTech.Components;

public partial class DataForm : TinyComponentBase
{
    [Parameter, EditorRequired] public object Model { get; set; } = null!;
    [Parameter] public EditContext? EditContext { get; set; }
    [Parameter] public string Layout { get; set; } = "vertical";
    [Parameter] public IReadOnlyList<string>? IncludeProperties { get; set; }
    [Parameter] public IReadOnlyList<string>? ExcludeProperties { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool ShowValidationSummary { get; set; } = true;
    [Parameter] public bool ShowDefaultActions { get; set; } = true;
    [Parameter] public bool ValidateOnSubmit { get; set; } = true;
    [Parameter] public EventCallback<EditContext> OnValidSubmit { get; set; }
    [Parameter] public EventCallback<EditContext> OnInvalidSubmit { get; set; }
    [Parameter] public EventCallback OnModelChanged { get; set; }
    [Parameter] public RenderFragment<DataFormField>? FieldTemplate { get; set; }
    [Parameter] public RenderFragment<DataFormField>? LabelTemplate { get; set; }
    [Parameter] public RenderFragment<DataFormField>? ValidationTemplate { get; set; }
    [Parameter] public RenderFragment<DataForm>? ActionsTemplate { get; set; }
    [Parameter] public Store<DataFormState>? Store { get; set; }
    [Parameter] public IReadOnlyList<IValidatorDescription>? Validators { get; set; }
    [Parameter] public FieldEventDiscoveryService? DiscoveryService { get; set; }
    [Parameter] public bool UseStore { get; set; }
    [Parameter] public EventCallback<FormFieldIntent> OnIntentDispatched { get; set; }

    private EditContext _editContext = null!;
    private object _currentModel = null!;
    private EditContext? _currentEditContextParam;
    private ValidationMessageStore? _messageStore;
    private IReadOnlyList<DataFormField> _fields = [];
    private List<string> _validationSummary = [];
    private readonly Dictionary<string, List<string>> _fieldMessages = new();
    private readonly Dictionary<string, object?> _originalValues = new();
    private bool _storeSubscribed;

    protected override void OnParametersSet()
    {
        var newEditContext = EditContext ?? new EditContext(Model);
        if (_currentModel != Model || _currentEditContextParam != EditContext)
        {
            _currentModel = Model;
            _currentEditContextParam = EditContext;
            _editContext = newEditContext;
            _messageStore = new ValidationMessageStore(_editContext);
            _fieldMessages.Clear();
            _validationSummary.Clear();
            DiscoverFields();
            SnapshotValues();

            if (UseStore && Store != null && !_storeSubscribed)
            {
                _storeSubscribed = true;
                Store.StateChanged += OnStoreStateChanged;
                Store.Dispatch(new DataFormActions.InitForm(_fields.ToList(), Model));
            }
        }
    }

    private void OnStoreStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        if (_storeSubscribed && Store != null)
        {
            Store.StateChanged -= OnStoreStateChanged;
        }
    }

    private static FormFieldType MapToFormFieldType(DataFormField field)
    {
        return field.InputKind switch
        {
            "checkbox" => FormFieldType.Bool,
            "select" => FormFieldType.Enum,
            "number" => field.PropertyType == typeof(decimal) || field.PropertyType == typeof(decimal?)
                ? FormFieldType.Decimal
                : FormFieldType.Number,
            "date" => field.PropertyType == typeof(DateTime) || field.PropertyType == typeof(DateTime?)
                ? FormFieldType.DateTime
                : FormFieldType.Date,
            _ => FormFieldType.Text
        };
    }

    private IValidatorDescription? GetValidatorForField(string propertyName)
    {
        return Validators?.FirstOrDefault(v =>
            string.Equals(v.FieldName, propertyName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task OnFieldChanging(FormFieldChangeArgs args)
    {
        if (UseStore && Store != null)
        {
            Store.Dispatch(new DataFormActions.FieldChanging(args.FieldName, args.NewValue));
        }
    }

    private async Task OnFieldChanged(FormFieldChangeArgs args)
    {
        if (UseStore && Store != null)
        {
            Store.Dispatch(new DataFormActions.FieldChanged(args.FieldName, args.NewValue));
            Store.Dispatch(new DataFormActions.SetFieldValue(args.FieldName, args.NewValue));

            var validator = GetValidatorForField(args.FieldName);
            var errors = new List<string>();
            if (validator?.Rules != null)
            {
                foreach (var rule in validator.Rules)
                {
                    try
                    {
                        var expr = new NCalc.Expression(rule.Condition);
                        expr.Parameters["value"] = args.NewValue;
                        if (expr.Evaluate() is not bool b || !b)
                            errors.Add(rule.Message);
                    }
                    catch
                    {
                        errors.Add(rule.Message);
                    }
                }
            }
            Store.Dispatch(new DataFormActions.ValidateField(args.FieldName, errors));
        }
        else
        {
            SetValue(args.FieldName, args.NewValue);
        }
    }

    private async Task OnFieldChange(FormFieldChangeArgs args)
    {
        if (UseStore && Store != null)
        {
            // Additional change handling if needed
        }
    }

    private async Task OnFieldBlur(FocusEventArgs args)
    {
        // Blur handling is per-field in FormField component
    }

    private async Task OnFieldFocus(FocusEventArgs args)
    {
        // Focus handling is per-field in FormField component
    }

    private void DiscoverFields()
    {
        var modelType = Model.GetType();
        var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .ToList();

        if (IncludeProperties?.Count > 0)
        {
            properties = properties
                .Where(p => IncludeProperties.Contains(p.Name))
                .ToList();
        }

        if (ExcludeProperties?.Count > 0)
        {
            properties = properties
                .Where(p => !ExcludeProperties.Contains(p.Name))
                .ToList();
        }

        var fields = new List<DataFormField>();
        foreach (var prop in properties)
        {
            var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttr?.Name ?? prop.Name;
            var order = displayAttr?.Order ?? 0;
            var required = prop.GetCustomAttribute<RequiredAttribute>() != null;
            var inputKind = MapInputKind(prop.PropertyType);

            fields.Add(new DataFormField(
                prop.Name,
                displayName,
                prop.PropertyType,
                inputKind,
                required,
                ReadOnly,
                order));
        }

        _fields = fields.OrderBy(f => f.Order).ThenBy(f => f.PropertyName).ToList();
    }

    private static string MapInputKind(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        if (underlying == typeof(bool))
            return "checkbox";
        if (underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(double) || underlying == typeof(decimal) || underlying == typeof(float))
            return "number";
        if (underlying == typeof(DateTime) || underlying == typeof(DateOnly))
            return "date";
        if (underlying.IsEnum)
            return "select";

        return "text";
    }

    private string GetClass()
    {
        var baseClass = "space-y-4";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private string GetFieldContainerClass()
    {
        return Layout switch
        {
            "grid" => "grid gap-4 sm:grid-cols-2",
            "horizontal" => "space-y-4",
            _ => "space-y-4"
        };
    }

    private string GetFieldWrapperClass()
    {
        return Layout switch
        {
            "horizontal" => "flex flex-row items-start gap-4",
            _ => "flex flex-col gap-2"
        };
    }

    private string GetInputClass(bool hasErrors)
    {
        var baseClass = "flex h-9 w-full rounded-md border bg-transparent px-3 py-1 text-base shadow-sm transition-colors file:inline-flex file:h-7 file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm";
        var borderClass = hasErrors
            ? " border-destructive focus-visible:ring-destructive"
            : " border-input focus-visible:ring-ring";
        return $"{baseClass}{borderClass}";
    }

    internal void SetValue(string propertyName, object? value)
    {
        try
        {
            var prop = Model.GetType().GetProperty(propertyName);
            if (prop == null || !prop.CanWrite) return;

            var targetType = prop.PropertyType;
            var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Ensure type compatibility before setting
            if (value != null && !underlying.IsAssignableFrom(value.GetType()))
            {
                try
                {
                    value = Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return; // Conversion failed; skip update to avoid crash
                }
            }

            // Handle null for non-nullable value types only
            if (value == null && underlying.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
            {
                value = Activator.CreateInstance(underlying);
            }

            prop.SetValue(Model, value);
            _editContext?.NotifyFieldChanged(new FieldIdentifier(Model, propertyName));
            OnModelChanged.InvokeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DataForm.SetValue error: {ex.Message}");
        }
    }

    internal static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null) return null;

        var str = value.ToString();
        if (string.IsNullOrEmpty(str)) return null;

        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Already the correct type — return as-is
        if (underlying.IsAssignableFrom(value.GetType()))
            return value;

        // Parse with invariant culture so browsers' dot-separated decimals work everywhere
        if (underlying == typeof(int) && int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal))
            return intVal;
        if (underlying == typeof(long) && long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
            return longVal;
        if (underlying == typeof(double) && double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleVal))
            return doubleVal;
        if (underlying == typeof(decimal) && decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var decimalVal))
            return decimalVal;
        if (underlying == typeof(float) && float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatVal))
            return floatVal;
        if (underlying == typeof(DateTime) && DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtVal))
            return dtVal;
        if (underlying == typeof(DateOnly) && DateOnly.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var doVal))
            return doVal;
        if (underlying == typeof(bool))
            return value is true || str == "on" || (bool.TryParse(str, out var b) && b);
        if (underlying.IsEnum && Enum.TryParse(underlying, str, out var enumVal))
            return enumVal;

        // Last-resort fallback
        try
        {
            return Convert.ChangeType(str, underlying, CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    internal static object? ConvertEnumValue(string? value, Type targetType)
    {
        if (string.IsNullOrEmpty(value)) return null;
        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (underlying.IsEnum && Enum.TryParse(underlying, value, out var enumVal))
            return enumVal;
        return ConvertValue(value, targetType);
    }

    internal object? GetValue(string propertyName)
    {
        var prop = Model.GetType().GetProperty(propertyName);
        return prop?.GetValue(Model);
    }

    internal string GetDisplayValue(string propertyName)
    {
        var prop = Model.GetType().GetProperty(propertyName);
        if (prop == null) return "";
        var value = prop.GetValue(Model);
        if (value == null) return "";

        if (value is DateTime dt)
            return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (value is DateOnly d)
            return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        return value.ToString() ?? "";
    }

    private IReadOnlyList<string> GetFieldMessages(string propertyName)
    {
        if (_editContext == null) return [];
        return _editContext.GetValidationMessages(new FieldIdentifier(Model, propertyName)).ToList();
    }

    private void SnapshotValues()
    {
        _originalValues.Clear();
        foreach (var field in _fields)
        {
            var prop = Model.GetType().GetProperty(field.PropertyName);
            if (prop != null && prop.CanRead)
                _originalValues[field.PropertyName] = prop.GetValue(Model);
        }
    }

    private void ResetModel()
    {
        foreach (var field in _fields)
        {
            if (_originalValues.TryGetValue(field.PropertyName, out var originalValue))
            {
                var prop = Model.GetType().GetProperty(field.PropertyName);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(Model, originalValue);
            }
        }
        _fieldMessages.Clear();
        _validationSummary.Clear();
        _editContext = new EditContext(Model);
    }

    private async Task HandleSubmit()
    {
        if (UseStore && Store != null)
        {
            Store.Dispatch(new DataFormActions.SubmitForm());
            if (Store.State.IsValid)
            {
                await OnValidSubmit.InvokeAsync(_editContext);
            }
            else
            {
                await OnInvalidSubmit.InvokeAsync(_editContext);
            }
            return;
        }

        if (ValidateOnSubmit)
        {
            _messageStore?.Clear();
            var validationContext = new ValidationContext(Model);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(Model, validationContext, results, validateAllProperties: true);

            foreach (var result in results)
            {
                foreach (var member in result.MemberNames.DefaultIfEmpty(string.Empty))
                {
                    _messageStore?.Add(_editContext.Field(member), result.ErrorMessage ?? "Invalid");
                }
            }

            _validationSummary = results.Select(r => r.ErrorMessage ?? "Invalid").ToList();
            var isValid = results.Count == 0;
            StateHasChanged();

            if (isValid)
            {
                await OnValidSubmit.InvokeAsync(_editContext);
            }
            else
            {
                await OnInvalidSubmit.InvokeAsync(_editContext);
            }
        }
        else
        {
            await OnValidSubmit.InvokeAsync(_editContext);
        }
    }
}
