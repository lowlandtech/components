namespace LowlandTech.Components;

public partial class SessionMarkdown : TinyComponentBase
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseSoftlineBreakAsHardlineBreak()
        .DisableHtml()
        .Build();

    [Parameter] public string? Value { get; set; }

    private string RenderedHtml => string.IsNullOrWhiteSpace(Value)
        ? string.Empty
        : Markdown.ToHtml(Value, Pipeline);

    private string GetClass()
        => ThemeClass(
            "SessionMarkdown",
            "Root",
            "session-markdown min-w-0 text-sm leading-6 text-foreground " +
            "[&_a]:text-primary [&_a]:underline [&_a]:underline-offset-4 " +
            "[&_blockquote]:my-3 [&_blockquote]:border-l-2 [&_blockquote]:border-border [&_blockquote]:pl-4 [&_blockquote]:text-muted-foreground " +
            "[&_code]:rounded [&_code]:bg-muted [&_code]:px-1 [&_code]:py-0.5 [&_code]:font-mono [&_code]:text-xs " +
            "[&_h1]:mb-3 [&_h1]:mt-5 [&_h1]:text-xl [&_h1]:font-semibold " +
            "[&_h2]:mb-3 [&_h2]:mt-5 [&_h2]:text-lg [&_h2]:font-semibold " +
            "[&_h3]:mb-2 [&_h3]:mt-4 [&_h3]:text-base [&_h3]:font-semibold " +
            "[&_li]:my-1 [&_ol]:my-3 [&_ol]:list-decimal [&_ol]:pl-6 " +
            "[&_p]:my-3 [&_p:first-child]:mt-0 [&_p:last-child]:mb-0 " +
            "[&_pre]:my-3 [&_pre]:overflow-x-auto [&_pre]:rounded-md [&_pre]:bg-muted [&_pre]:p-3 " +
            "[&_pre_code]:bg-transparent [&_pre_code]:p-0 " +
            "[&_strong]:font-semibold [&_table]:my-3 [&_table]:w-full [&_table]:border-collapse " +
            "[&_td]:border [&_td]:border-border [&_td]:p-2 [&_th]:border [&_th]:border-border [&_th]:p-2 [&_th]:text-left " +
            "[&_ul]:my-3 [&_ul]:list-disc [&_ul]:pl-6",
            consumerClass: Class,
            designClass: DesignModeClass);
}
