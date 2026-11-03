using Cpkg0006.Consumer;
using LowlandTech.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// The probe renders the three component surfaces whose reflection the packages suppress.
// A trimmed publish removes what nothing proved is needed, so every observation here is a
// preservation check: the same code that works untrimmed has to work in a trimmed process.
var services = new ServiceCollection()
    .AddLogging()
    .AddLocalization()
    .AddSingleton<BlockContext>()
    .BuildServiceProvider();

await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());

var grid = await RenderAsync<GridHost>();
var hooks = await RenderAsync<HookHost>();
var outbox = await RenderAsync<OutboxHost>();

Console.WriteLine("GRID_ADA=" + grid.Contains("Ada", StringComparison.Ordinal));
Console.WriteLine("GRID_GRACE=" + grid.Contains("Grace", StringComparison.Ordinal));
Console.WriteLine("HOOK_RUNS=" + HookHost.HookRuns);
Console.WriteLine("OUTBOX_VALUE=" + outbox.Contains("value", StringComparison.OrdinalIgnoreCase));
Console.WriteLine("OUTBOX_SEVEN=" + outbox.Contains("7", StringComparison.Ordinal));

async Task<string> RenderAsync<TComponent>() where TComponent : IComponent =>
    await renderer.Dispatcher.InvokeAsync(async () =>
        (await renderer.RenderComponentAsync<TComponent>(ParameterView.Empty)).ToHtmlString());
