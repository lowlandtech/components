namespace LowlandTech.Components;

public partial class SessionAddToListDialog : TinyComponentBase
{
    [Inject] private IStringLocalizer<SessionBlockStrings>? Localizer { get; set; }

    private string L(string key, params object[] args) =>
        args.Length == 0
            ? Localizer?[key] ?? key
            : string.Format(Localizer?[key] ?? key, args);

    [Parameter, EditorRequired] public SessionNodeInput Node { get; set; } = new("context", "context", "Context");
    [Parameter] public IReadOnlyList<SessionNodeInput> Lists { get; set; } = [];
    [Parameter] public IReadOnlySet<string> MemberListIds { get; set; } = new HashSet<string>();
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public EventCallback<bool> IsOpenChanged { get; set; }
    [Parameter] public EventCallback<SessionAddToListRequest> OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private List<AddToListCandidate> Candidates { get; set; } = [];

    protected override void OnParametersSet()
    {
        Candidates = Lists
            .Select(list => new AddToListCandidate(list, MemberListIds.Contains(list.Id)))
            .ToList();
    }

    private async Task ConfirmAsync()
    {
        var toAdd = Candidates
            .Where(c => c.IsSelected && !MemberListIds.Contains(c.List.Id))
            .Select(c => c.List.Id)
            .ToHashSet();

        var toRemove = Candidates
            .Where(c => !c.IsSelected && MemberListIds.Contains(c.List.Id))
            .Select(c => c.List.Id)
            .ToHashSet();

        if (toAdd.Count > 0 || toRemove.Count > 0)
        {
            await OnConfirm.InvokeAsync(new SessionAddToListRequest(Node.Id, Node.NodeTypeKey, toAdd, toRemove));
        }

        await CloseAsync();
    }

    private async Task CancelAsync()
    {
        await OnCancel.InvokeAsync();
        await CloseAsync();
    }

    private async Task CloseAsync()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(false);
    }

    public sealed class AddToListCandidate(SessionNodeInput list, bool isSelected)
    {
        public SessionNodeInput List { get; } = list;
        public bool IsSelected { get; set; } = isSelected;
    }
}
