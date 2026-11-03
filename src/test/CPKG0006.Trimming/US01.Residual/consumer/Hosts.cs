using LowlandTech.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Cpkg0006.Consumer;

/// <summary>
/// Renders a <see cref="DataGrid{TItem}"/> whose only column reads a nested property of
/// the row model. The column resolves that path by reflection, which is one of the
/// suppressed paths this probe has to see work after trimming.
/// </summary>
public sealed class GridHost : ComponentBase
{
    private static readonly ProbeModel[] Rows =
    [
        new(new ProbeCustomer("Ada")),
        new(new ProbeCustomer("Grace")),
    ];

    /// <summary>The rendered markup, for the probe to inspect.</summary>
    public static string LastMarkup { get; private set; } = string.Empty;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<DataGrid<ProbeModel>>(0);
        builder.AddAttribute(1, "Items", Rows);
        builder.AddAttribute(2, "Columns", (RenderFragment)(columns =>
        {
            columns.OpenComponent<DataGridColumn<ProbeModel>>(3);
            columns.AddAttribute(4, "Property", "Customer.Name");
            columns.AddAttribute(5, "Title", "Customer");
            columns.CloseComponent();
        }));
        builder.CloseComponent();
    }
}

/// <summary>
/// A block page with one attributed lifecycle hook. The base class discovers the hook by
/// scanning the methods of the component type, which is the second suppressed path.
/// </summary>
public sealed class HookHost : BlockPageBase
{
    /// <summary>How often the attributed hook ran.</summary>
    public static int HookRuns { get; private set; }

    [OnInitialized]
    private void ProbeHook() => HookRuns++;

    protected override void BuildRenderTree(RenderTreeBuilder builder) =>
        builder.AddContent(0, "hook-host");
}

/// <summary>
/// Renders a session outbox that carries a payload of the consumer's own type. The list
/// serializes that payload to show it, which is the third suppressed path.
/// </summary>
public sealed class OutboxHost : ComponentBase
{
    private readonly ProbeOutbox _outbox = new();

    protected override void OnInitialized() =>
        _outbox.Append(new SessionOutboxEntry("probe", new ProbePayload { Value = 7 }, DateTimeOffset.UtcNow));

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<SessionOutboxList>(0);
        builder.AddAttribute(1, "Outbox", _outbox);
        builder.CloseComponent();
    }
}

/// <summary>The row model of the grid.</summary>
public sealed class ProbeModel(ProbeCustomer customer)
{
    public ProbeCustomer Customer { get; } = customer;
}

/// <summary>The nested value the column reads.</summary>
public sealed class ProbeCustomer(string name)
{
    public string Name { get; } = name;
}

/// <summary>The payload the outbox carries.</summary>
public sealed class ProbePayload
{
    public int Value { get; set; }
}

/// <summary>The smallest outbox the list needs.</summary>
public sealed class ProbeOutbox : ISessionOutbox
{
    private readonly List<SessionOutboxEntry> _entries = [];

    public IReadOnlyList<SessionOutboxEntry> Entries => _entries;

    public event Action? Changed;

    public void Append(SessionOutboxEntry entry)
    {
        _entries.Add(entry);
        Changed?.Invoke();
    }

    public void Clear()
    {
        _entries.Clear();
        Changed?.Invoke();
    }
}
