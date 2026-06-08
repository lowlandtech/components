namespace LowlandTech.Components;

public partial class SessionMarkdown : TinyComponentBase
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseSoftlineBreakAsHardlineBreak()
        .DisableHtml()
        .Build();

    [Parameter] public string? Value { get; set; }
    [Parameter] public bool IsStreaming { get; set; }

    [Inject] private IJSRuntime JS { get; set; } = default!;

    private string? _highlightedValue;

    private string RenderedHtml => string.IsNullOrWhiteSpace(Value)
        ? string.Empty
        : Markdown.ToHtml(Value, Pipeline);

    private string GetClass()
        => ThemeClass(
            "SessionMarkdown",
            "Root",
            "markdown-content session-markdown min-w-0",
            consumerClass: Class,
            designClass: DesignModeClass);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsStreaming)
            return;

        if (!firstRender && string.Equals(_highlightedValue, Value, StringComparison.Ordinal))
            return;

        _highlightedValue = Value;

        try
        {
            await JS.InvokeVoidAsync("highlightCode");
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop is unavailable during static prerendering.
        }
        catch (JSException)
        {
            // Hosts without Highlight.js still retain server-rendered Markdown.
        }
    }
}
