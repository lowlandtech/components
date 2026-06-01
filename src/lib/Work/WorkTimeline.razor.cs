namespace LowlandTech.Components.Work;

public partial class WorkTimeline : TinyComponentBase
{
    [Parameter] public IReadOnlyList<WorkStateHistory> History { get; set; } = [];

    private static string GetDotClass(string stateName) => stateName.ToLowerInvariant() switch
    {
        "succeeded" => "border-emerald-500",
        "failed" => "border-rose-500",
        "processing" => "border-amber-500",
        "scheduled" => "border-blue-500",
        "enqueued" => "border-muted-foreground",
        "deleted" => "border-slate-400",
        "awaiting" => "border-violet-500",
        "retry" => "border-orange-500",
        _ => "border-muted-foreground"
    };

    private static string GetTextClass(string stateName) => stateName.ToLowerInvariant() switch
    {
        "succeeded" => "text-emerald-700 dark:text-emerald-300",
        "failed" => "text-rose-700 dark:text-rose-300",
        "processing" => "text-amber-700 dark:text-amber-300",
        "scheduled" => "text-blue-700 dark:text-blue-300",
        "enqueued" => "text-muted-foreground",
        "deleted" => "text-slate-500 line-through",
        "awaiting" => "text-violet-700 dark:text-violet-300",
        "retry" => "text-orange-700 dark:text-orange-300",
        _ => "text-muted-foreground"
    };

    private static string GetRelativeTime(DateTimeOffset time)
    {
        var diff = DateTimeOffset.UtcNow - time;
        if (diff.TotalSeconds < 60) return "just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
        return $"{(int)diff.TotalDays}d ago";
    }

    private string GetClass() => string.IsNullOrEmpty(Class)
        ? "w-full"
        : $"w-full {Class}";
}
