namespace LowlandTech.Components;

/// <summary>
/// Effects for DataFormState that dispatch intents via FieldEventDiscoveryService.
/// </summary>
public class DataFormEffects
{
    private readonly FieldEventDiscoveryService _discovery;

    public DataFormEffects(FieldEventDiscoveryService discovery)
    {
        _discovery = discovery;
    }

    public async Task HandleFieldChanging(LowlandTech.Components.Core.Store<DataFormState> store, object action)
    {
        if (action is not DataFormActions.FieldChanging changing) return;

        var intent = _discovery.BuildIntent(changing.FieldName, "Changing", changing.Value);
        store.Dispatch(new DataFormActions.DispatchIntent(intent));
    }

    public async Task HandleFieldChanged(LowlandTech.Components.Core.Store<DataFormState> store, object action)
    {
        if (action is not DataFormActions.FieldChanged changed) return;

        var intent = _discovery.BuildIntent(changed.FieldName, "Changed", changed.Value);
        store.Dispatch(new DataFormActions.DispatchIntent(intent));
    }

    public async Task HandleFieldBlurred(LowlandTech.Components.Core.Store<DataFormState> store, object action)
    {
        if (action is not DataFormActions.FieldBlurred blurred) return;

        var intent = _discovery.BuildIntent(blurred.FieldName, "Blur");
        store.Dispatch(new DataFormActions.DispatchIntent(intent));
    }

    public async Task HandleFieldFocused(LowlandTech.Components.Core.Store<DataFormState> store, object action)
    {
        if (action is not DataFormActions.FieldFocused focused) return;

        var intent = _discovery.BuildIntent(focused.FieldName, "Focus");
        store.Dispatch(new DataFormActions.DispatchIntent(intent));
    }

    public async Task HandleSubmitForm(LowlandTech.Components.Core.Store<DataFormState> store, object action)
    {
        if (action is not DataFormActions.SubmitForm) return;

        // Submit intent
        var intent = _discovery.BuildIntent("form", "Submit", store.State.Values);
        store.Dispatch(new DataFormActions.DispatchIntent(intent));
    }
}
