namespace LowlandTech.Components.DataForm;

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

    private EditContext _editContext = null!;
    private object _currentModel = null!;
    private IReadOnlyList<DataFormField> _fields = [];
    private List<string> _validationSummary = [];
    private readonly Dictionary<string, List<string>> _fieldMessages = new();

    protected override void OnParametersSet()
    {
        if (_currentModel != Model)
        {
            _currentModel = Model;
            _editContext = EditContext ?? new EditContext(Model);
            _fieldMessages.Clear();
            _validationSummary.Clear();
            DiscoverFields();
        }
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

    private RenderFragment RenderInput(DataFormField field, string fieldId, bool hasErrors) => builder =>
    {
        var inputKind = field.InputKind;
        var prop = Model.GetType().GetProperty(field.PropertyName);
        if (prop == null) return;

        var value = prop.GetValue(Model);
        var ariaInvalid = hasErrors ? "true" : null;
        var describedBy = hasErrors ? $"{fieldId}-validation" : null;
        var cssClass = hasErrors
            ? "flex h-9 w-full rounded-md border border-destructive bg-transparent px-3 py-1 text-base shadow-sm transition-colors file:inline-flex file:h-7 file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-destructive disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
            : "flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-base shadow-sm transition-colors file:inline-flex file:h-7 file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm";

        if (inputKind == "checkbox")
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "type", "checkbox");
            builder.AddAttribute(2, "id", fieldId);
            builder.AddAttribute(3, "data-field-input", field.PropertyName);
            builder.AddAttribute(4, "checked", value is true);
            builder.AddAttribute(5, "class", "h-4 w-4 rounded border-input text-primary focus:ring-ring");
            builder.AddAttribute(6, "aria-invalid", ariaInvalid);
            if (describedBy != null) builder.AddAttribute(7, "aria-describedby", describedBy);
            builder.AddAttribute(8, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
            {
                var boolValue = e.Value is true || e.Value?.ToString() == "on";
                SetValue(field.PropertyName, boolValue);
            }));
            builder.CloseElement();
        }
        else if (inputKind == "select" && prop.PropertyType.IsEnum)
        {
            builder.OpenElement(0, "select");
            builder.AddAttribute(1, "id", fieldId);
            builder.AddAttribute(2, "data-field-input", field.PropertyName);
            builder.AddAttribute(3, "class", cssClass);
            builder.AddAttribute(4, "aria-invalid", ariaInvalid);
            if (describedBy != null) builder.AddAttribute(5, "aria-describedby", describedBy);
            builder.AddAttribute(6, "value", value?.ToString() ?? "");
            builder.AddAttribute(7, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
            {
                if (Enum.TryParse(prop.PropertyType, e.Value?.ToString(), out var enumValue))
                {
                    SetValue(field.PropertyName, enumValue);
                }
            }));

            foreach (var name in Enum.GetNames(prop.PropertyType))
            {
                builder.OpenElement(8, "option");
                builder.AddAttribute(9, "value", name);
                builder.AddContent(10, name);
                builder.CloseElement();
            }

            builder.CloseElement();
        }
        else
        {
            builder.OpenElement(0, "input");
            builder.AddAttribute(1, "type", inputKind);
            builder.AddAttribute(2, "id", fieldId);
            builder.AddAttribute(3, "data-field-input", field.PropertyName);
            builder.AddAttribute(4, "value", value?.ToString() ?? "");
            builder.AddAttribute(5, "class", cssClass);
            builder.AddAttribute(6, "aria-invalid", ariaInvalid);
            if (describedBy != null) builder.AddAttribute(7, "aria-describedby", describedBy);
            builder.AddAttribute(8, "oninput", EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
            {
                var newValue = ConvertValue(e.Value, prop.PropertyType);
                SetValue(field.PropertyName, newValue);
            }));
            builder.CloseElement();
        }
    };

    private void SetValue(string propertyName, object? value)
    {
        var prop = Model.GetType().GetProperty(propertyName);
        if (prop == null || !prop.CanWrite) return;

        prop.SetValue(Model, value);
        _editContext?.NotifyFieldChanged(new FieldIdentifier(Model, propertyName));
        OnModelChanged.InvokeAsync();
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null) return null;

        var underlying = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlying == typeof(int) && int.TryParse(value.ToString(), out var intVal))
            return intVal;
        if (underlying == typeof(long) && long.TryParse(value.ToString(), out var longVal))
            return longVal;
        if (underlying == typeof(double) && double.TryParse(value.ToString(), out var doubleVal))
            return doubleVal;
        if (underlying == typeof(decimal) && decimal.TryParse(value.ToString(), out var decimalVal))
            return decimalVal;
        if (underlying == typeof(float) && float.TryParse(value.ToString(), out var floatVal))
            return floatVal;
        if (underlying == typeof(DateTime) && DateTime.TryParse(value.ToString(), out var dtVal))
            return dtVal;
        if (underlying == typeof(DateOnly) && DateOnly.TryParse(value.ToString(), out var doVal))
            return doVal;
        if (underlying == typeof(bool))
            return value is true || value.ToString() == "on";

        return value;
    }

    private object? GetDisplayValue(string propertyName)
    {
        var prop = Model.GetType().GetProperty(propertyName);
        return prop?.GetValue(Model)?.ToString() ?? "";
    }

    private IReadOnlyList<string> GetFieldMessages(string propertyName)
    {
        if (_editContext == null) return [];
        return _editContext.GetValidationMessages(new FieldIdentifier(Model, propertyName)).ToList();
    }

    private void ResetModel()
    {
        _fieldMessages.Clear();
        _validationSummary.Clear();
        _editContext = new EditContext(Model);
        StateHasChanged();
    }

    private async Task HandleSubmit()
    {
        if (ValidateOnSubmit)
        {
            var isValid = _editContext.Validate();
            _validationSummary = _editContext.GetValidationMessages().ToList();
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
