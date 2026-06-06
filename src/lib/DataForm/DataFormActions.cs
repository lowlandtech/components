namespace LowlandTech.Components;

/// <summary>
/// Actions for DataFormState reducer.
/// </summary>
public static class DataFormActions
{
    public sealed record InitForm(IReadOnlyList<DataFormField> Fields, object? Model = null);
    public sealed record SetFieldValue(string FieldName, object? Value);
    public sealed record FieldChanging(string FieldName, object? Value);
    public sealed record FieldChanged(string FieldName, object? Value);
    public sealed record FieldBlurred(string FieldName);
    public sealed record FieldFocused(string FieldName);
    public sealed record ValidateField(string FieldName, IReadOnlyList<string> Errors);
    public sealed record SubmitForm;
    public sealed record SubmitSuccess;
    public sealed record SubmitFailure(IReadOnlyDictionary<string, IReadOnlyList<string>> Errors);
    public sealed record ResetForm;
    public sealed record DispatchIntent(FormFieldIntent Intent);
}
