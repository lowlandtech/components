namespace LowlandTech.Components;

/// <summary>
/// Reducer for DataFormState.
/// </summary>
public static class DataFormReducer
{
    public static DataFormState Reduce(DataFormState state, object action)
    {
        return action switch
        {
            DataFormActions.InitForm init => ReduceInitForm(state, init),
            DataFormActions.SetFieldValue set => ReduceSetFieldValue(state, set),
            DataFormActions.FieldChanging changing => ReduceFieldChanging(state, changing),
            DataFormActions.FieldChanged changed => ReduceFieldChanged(state, changed),
            DataFormActions.FieldBlurred blurred => ReduceFieldBlurred(state, blurred),
            DataFormActions.FieldFocused focused => ReduceFieldFocused(state, focused),
            DataFormActions.ValidateField validate => ReduceValidateField(state, validate),
            DataFormActions.SubmitForm => state with { IsSubmitting = true },
            DataFormActions.SubmitSuccess => state with { IsSubmitting = false, IsValid = true, Errors = new Dictionary<string, IReadOnlyList<string>>() },
            DataFormActions.SubmitFailure failure => state with { IsSubmitting = false, IsValid = false, Errors = failure.Errors },
            DataFormActions.ResetForm => new DataFormState { Model = state.Model },
            _ => state
        };
    }

    private static DataFormState ReduceInitForm(DataFormState state, DataFormActions.InitForm action)
    {
        var values = new Dictionary<string, object?>();
        foreach (var field in action.Fields)
        {
            values[field.PropertyName] = null;
        }
        return state with
        {
            Fields = action.Fields,
            Values = values,
            Errors = new Dictionary<string, IReadOnlyList<string>>(),
            Touched = new HashSet<string>(),
            IsValid = true,
            IsSubmitting = false,
            IsDirty = false,
            Model = action.Model
        };
    }

    private static DataFormState ReduceSetFieldValue(DataFormState state, DataFormActions.SetFieldValue action)
    {
        var values = new Dictionary<string, object?>(state.Values)
        {
            [action.FieldName] = action.Value
        };
        return state with
        {
            Values = values,
            IsDirty = true
        };
    }

    private static DataFormState ReduceFieldChanging(DataFormState state, DataFormActions.FieldChanging action)
    {
        var values = new Dictionary<string, object?>(state.Values)
        {
            [action.FieldName] = action.Value
        };
        return state with
        {
            Values = values,
            IsDirty = true
        };
    }

    private static DataFormState ReduceFieldChanged(DataFormState state, DataFormActions.FieldChanged action)
    {
        var touched = new HashSet<string>(state.Touched)
        {
            action.FieldName
        };
        return state with
        {
            Touched = touched
        };
    }

    private static DataFormState ReduceFieldBlurred(DataFormState state, DataFormActions.FieldBlurred action)
    {
        var touched = new HashSet<string>(state.Touched)
        {
            action.FieldName
        };
        return state with { Touched = touched };
    }

    private static DataFormState ReduceFieldFocused(DataFormState state, DataFormActions.FieldFocused action)
    {
        return state;
    }

    private static DataFormState ReduceValidateField(DataFormState state, DataFormActions.ValidateField action)
    {
        var errors = new Dictionary<string, IReadOnlyList<string>>(state.Errors);
        if (action.Errors.Count > 0)
        {
            errors[action.FieldName] = action.Errors;
        }
        else
        {
            errors.Remove(action.FieldName);
        }
        var isValid = errors.Count == 0;
        return state with
        {
            Errors = errors,
            IsValid = isValid
        };
    }
}
