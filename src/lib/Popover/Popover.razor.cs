using Microsoft.AspNetCore.Components;

namespace LowlandTech.Components.Popover;

public partial class Popover : LtComponentBase, IDisposable
{
    private Guid _id = Guid.NewGuid();
    private bool _registered;

    [Parameter] public RenderFragment? PopoverContent { get; set; }
    [Parameter] public bool Open { get; set; }
    [Parameter] public string? PopoverClass { get; set; }
    [Parameter] public PopoverPlacement Placement { get; set; } = PopoverPlacement.BottomStart;

    [Inject] private IPopoverService PopoverService { get; set; } = default!;

    protected override void OnInitialized()
    {
        PopoverService.Register(_id);
        _registered = true;
    }

    protected override void OnParametersSet()
    {
        if (_registered)
        {
            PopoverService.Update(_id, new PopoverState
            {
                Open = Open,
                Fragment = PopoverContent,
                CssClass = PopoverClass,
                Placement = Placement,
            });
        }
    }

    public void Dispose()
    {
        PopoverService.Unregister(_id);
    }
}
