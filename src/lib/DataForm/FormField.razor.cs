namespace LowlandTech.Components;

public partial class FormField : TinyComponentBase
{
    [Parameter, EditorRequired] public string FieldName { get; set; } = string.Empty;
    [Parameter] public FormFieldType FieldType { get; set; } = FormFieldType.Text;
    [Parameter] public string? Label { get; set; }
    [Parameter] public object? Value { get; set; }
    [Parameter] public EventCallback<object?> ValueChanged { get; set; }
    [Parameter] public Type? EnumType { get; set; }
    [Parameter] public IValidatorDescription? Validator { get; set; }
    [Parameter] public string? Mask { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public EventCallback<FormFieldChangeArgs> OnChanging { get; set; }
    [Parameter] public EventCallback<FormFieldChangeArgs> OnChanged { get; set; }
    [Parameter] public EventCallback<FormFieldChangeArgs> OnChange { get; set; }
    [Parameter] public EventCallback<FocusEventArgs> OnBlur { get; set; }
    [Parameter] public EventCallback<FocusEventArgs> OnFocus { get; set; }
    [Parameter] public RenderFragment<FormField>? LabelTemplate { get; set; }
    [Parameter] public RenderFragment<FormField>? ValidationTemplate { get; set; }

    private string _valueString = string.Empty;
    private readonly List<string> _errors = new();
    private object? _lastValue;

    private string FieldId => $"form-field-{FieldName}";
    private string ValidationId => $"{FieldId}-validation";
    private bool HasErrors => _errors.Count > 0;

    protected override void OnParametersSet()
    {
        _valueString = FormatValue(Value);
        _lastValue = Value;
    }

    private string FormatValue(object? value)
    {
        if (value == null) return string.Empty;
        if (value is DateTime dt) return dt.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        if (value is DateOnly d) return d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return value.ToString() ?? string.Empty;
    }

    private string GetWrapperClass()
    {
        var baseClass = "flex flex-col gap-2";
        return string.IsNullOrEmpty(Class) ? baseClass : $"{baseClass} {Class}";
    }

    private string GetInputClass()
    {
        var baseClass = "flex h-9 w-full rounded-md border bg-transparent px-3 py-1 text-base shadow-sm transition-colors file:inline-flex file:h-7 file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 md:text-sm";
        var borderClass = HasErrors
            ? " border-destructive focus-visible:ring-destructive"
            : " border-input focus-visible:ring-ring";
        return $"{baseClass}{borderClass}";
    }

    private async Task OnBoolChanged(bool value)
    {
        var oldValue = Value;
        Value = value;
        _valueString = value.ToString().ToLowerInvariant();
        await EmitChangingAsync(oldValue, value);
        Validate();
        await EmitChangedAsync(oldValue, value);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task OnSelectChanged(string? value)
    {
        var oldValue = Value;
        object? newValue = null;
        if (EnumType != null && !string.IsNullOrEmpty(value))
        {
            if (Enum.TryParse(EnumType, value, out var parsed))
                newValue = parsed;
        }
        Value = newValue;
        _valueString = value ?? string.Empty;
        await EmitChangingAsync(oldValue, newValue);
        Validate();
        await EmitChangedAsync(oldValue, newValue);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task OnInputChanging(ChangeEventArgs e)
    {
        var rawValue = e.Value?.ToString() ?? string.Empty;
        var maskedValue = FormFieldMask.Apply(rawValue, Mask);
        _valueString = maskedValue ?? rawValue;
        var oldValue = Value;
        var newValue = ConvertInputValue(_valueString);
        await EmitChangingAsync(oldValue, newValue);
    }

    private async Task OnInputChanged(ChangeEventArgs e)
    {
        var rawValue = e.Value?.ToString() ?? string.Empty;
        var maskedValue = FormFieldMask.Apply(rawValue, Mask);
        _valueString = maskedValue ?? rawValue;
        var oldValue = Value;
        Value = ConvertInputValue(_valueString);
        Validate();
        await EmitChangedAsync(oldValue, Value);
        await EmitChangeAsync(oldValue, Value);
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task OnInputBlur(FocusEventArgs e)
    {
        Value = ConvertInputValue(_valueString);
        Validate();
        if (OnBlur.HasDelegate)
            await OnBlur.InvokeAsync(e);
    }

    private async Task OnInputFocus(FocusEventArgs e)
    {
        if (OnFocus.HasDelegate)
            await OnFocus.InvokeAsync(e);
    }

    private object? ConvertInputValue(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;

        return FieldType switch
        {
            FormFieldType.Number => int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : null,
            FormFieldType.Decimal => decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null,
            FormFieldType.Date => DateOnly.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null,
            FormFieldType.DateTime => DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : null,
            FormFieldType.Guid => Guid.TryParse(input, out var g) ? g : null,
            FormFieldType.Bool => bool.TryParse(input, out var b) && b,
            FormFieldType.Enum => EnumType != null && Enum.TryParse(EnumType, input, out var e) ? e : null,
            _ => input
        };
    }

    private void Validate()
    {
        _errors.Clear();

        if (Required && (Value == null || (Value is string s && string.IsNullOrWhiteSpace(s))))
        {
            _errors.Add($"{Label ?? FieldName} is required.");
        }

        if (Validator?.Rules != null)
        {
            foreach (var rule in Validator.Rules)
            {
                if (!EvaluateRule(rule))
                {
                    _errors.Add(rule.Message);
                }
            }
        }

        if (!string.IsNullOrEmpty(Mask) && Value is string str && !FormFieldMask.IsComplete(str, Mask))
        {
            _errors.Add($"{Label ?? FieldName} does not match the required format.");
        }

        if (!string.IsNullOrEmpty(Validator?.Pattern) && Value is string text && !Regex.IsMatch(text, Validator.Pattern))
        {
            _errors.Add(Validator?.ErrorMessage ?? $"{Label ?? FieldName} is invalid.");
        }

        if (Validator?.MaxLength != null && Value is string txt && txt.Length > Validator.MaxLength)
        {
            _errors.Add($"{Label ?? FieldName} must not exceed {Validator.MaxLength} characters.");
        }
    }

    private bool EvaluateRule(ValidationRule rule)
    {
        try
        {
            var expr = new NCalc.Expression(rule.Condition);
            expr.Parameters["value"] = Value;
            var result = expr.Evaluate();
            return result is bool b && b;
        }
        catch
        {
            return false;
        }
    }

    private async Task EmitChangingAsync(object? oldValue, object? newValue)
    {
        if (!OnChanging.HasDelegate) return;
        await OnChanging.InvokeAsync(new FormFieldChangeArgs
        {
            FieldName = FieldName,
            OldValue = oldValue,
            NewValue = newValue,
            IsValid = !HasErrors,
            Errors = _errors.ToList()
        });
    }

    private async Task EmitChangedAsync(object? oldValue, object? newValue)
    {
        if (!OnChanged.HasDelegate) return;
        await OnChanged.InvokeAsync(new FormFieldChangeArgs
        {
            FieldName = FieldName,
            OldValue = oldValue,
            NewValue = newValue,
            IsValid = !HasErrors,
            Errors = _errors.ToList()
        });
    }

    private async Task EmitChangeAsync(object? oldValue, object? newValue)
    {
        if (!OnChange.HasDelegate) return;
        await OnChange.InvokeAsync(new FormFieldChangeArgs
        {
            FieldName = FieldName,
            OldValue = oldValue,
            NewValue = newValue,
            IsValid = !HasErrors,
            Errors = _errors.ToList()
        });
    }
}
