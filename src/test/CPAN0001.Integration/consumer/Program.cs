using LowlandTech.Components;
using LowlandTech.Components.SessionConsumer;
using LowlandTech.Components.SessionConsumer.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddLocalization();

// The host owns the authority over the plugin owners. The consumer contributes
// one panel for one owner, so a browser can watch what a withdrawal of that
// owner does to two live circuits.
var owner = new SessionPluginOwner("consumer.owner", new ConsumerSessionPanel());

var bridge = new SessionPluginOwnerBridge(
    new SessionPluginOwnerSnapshot(1, [owner.Key]),
    [owner.Panel]);

bridge.Register(owner.Key, owner.Panel);

builder.Services.AddSingleton(owner);
builder.Services.AddSingleton(bridge);
builder.Services.AddSingleton<ISessionPluginRegistry>(bridge);

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// The host withdraws the owner of the panel: the next revision of the
// authority names no active owner, and every live circuit follows it.
app.MapPost("/withdraw", (SessionPluginOwnerBridge ownerBridge) =>
{
    ownerBridge.Deliver(new SessionPluginOwnerSnapshot(2, []));

    return Results.NoContent();
});

// The host activates the owner again, so a browser can prove that a
// reactivation restores the panel.
app.MapPost("/activate", (SessionPluginOwnerBridge ownerBridge) =>
{
    ownerBridge.Deliver(new SessionPluginOwnerSnapshot(3, ["consumer.owner"]));

    return Results.NoContent();
});

app.Run();
