namespace LowlandTech.Components;

public static class SessionGraphSchemaEffects
{
    public static Task LoadPagesAsync(
        ISessionGraphSchemaQueryService schema,
        SessionStateService stateService,
        object action,
        CancellationToken ct = default) =>
        action switch
        {
            LoadSessionNodeTypesPageRequested requested => LoadNodeTypesAsync(schema, stateService, requested, ct),
            LoadSessionEdgeTypesPageRequested requested => LoadEdgeTypesAsync(schema, stateService, requested, ct),
            _ => Task.CompletedTask,
        };

    private static async Task LoadNodeTypesAsync(
        ISessionGraphSchemaQueryService schema,
        SessionStateService stateService,
        LoadSessionNodeTypesPageRequested action,
        CancellationToken ct)
    {
        try
        {
            var page = await schema.QueryNodeTypesAsync(
                new SessionGraphSchemaPageRequest(action.PageIndex, action.PageSize, action.Search),
                ct).ConfigureAwait(false);
            stateService.Dispatch(new LoadSessionNodeTypesPageSucceeded(
                page.Items,
                page.TotalCount,
                page.PageIndex,
                page.PageSize,
                page.Search));
        }
        catch (Exception ex)
        {
            stateService.Dispatch(new LoadSessionNodeTypesPageFailed(ex.Message));
        }
    }

    private static async Task LoadEdgeTypesAsync(
        ISessionGraphSchemaQueryService schema,
        SessionStateService stateService,
        LoadSessionEdgeTypesPageRequested action,
        CancellationToken ct)
    {
        try
        {
            var page = await schema.QueryEdgeTypesAsync(
                new SessionGraphSchemaPageRequest(action.PageIndex, action.PageSize, action.Search),
                ct).ConfigureAwait(false);
            stateService.Dispatch(new LoadSessionEdgeTypesPageSucceeded(
                page.Items,
                page.TotalCount,
                page.PageIndex,
                page.PageSize,
                page.Search));
        }
        catch (Exception ex)
        {
            stateService.Dispatch(new LoadSessionEdgeTypesPageFailed(ex.Message));
        }
    }
}
