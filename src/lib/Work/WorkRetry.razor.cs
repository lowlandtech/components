namespace LowlandTech.Components.Work;

public partial class WorkRetry : TinyComponentBase
{
    [Parameter] public IReadOnlyList<WorkItem> Jobs { get; set; } = [];
    [Parameter] public bool IsReadOnly { get; set; }
    [Parameter] public EventCallback<ToggleWorkSelection> OnToggleSelection { get; set; }
    [Parameter] public EventCallback<WorkItemSelected> OnSelected { get; set; }

    private async Task ToggleAsync(string jobId)
    {
        await OnToggleSelection.InvokeAsync(new ToggleWorkSelection(jobId));
    }

    private async Task SelectAsync(WorkItem job)
    {
        await OnSelected.InvokeAsync(new WorkItemSelected(job.Id));
    }

    private static string GetRelativeTime(DateTimeOffset? value)
    {
        if (value is null)
            return "—";

        var diff = DateTimeOffset.UtcNow - value.Value;
        if (diff.TotalSeconds < 60) return "just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        return $"{(int)diff.TotalDays}d ago";
    }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "rounded-lg border border-border bg-card"
        : $"rounded-lg border border-border bg-card {Class}";
}
