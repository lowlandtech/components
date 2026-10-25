using LowlandTech.Components;
using LowlandTech.Components.Core;
using LowlandTech.BlockConsumer;
using LowlandTech.BlockConsumer.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddLocalization();

// The authority of the host over the plugin owners is the only state the
// circuits share.
builder.Services.AddSingleton<ConsumerBlockAuthority>();

// The bridge is scoped: it projects the authority onto the session block
// registries of its own circuit.
builder.Services.AddScoped(serviceProvider =>
{
    var authority = serviceProvider.GetRequiredService<ConsumerBlockAuthority>();

    // The trusted startup map of the host: the symbolic identity of the
    // contributed block resolves to its type here and nowhere else.
    var startupMap = new ComponentsStartupMap(
        [
            new ComponentsStartupBinding(
                ConsumerBlockAuthority.OwnerKey,
                ConsumerBlockAuthority.SymbolicIdentity,
                typeof(ConsumerOwnedBlock)),
        ],
        [ConsumerBlockAuthority.OwnerKey]);

    return new SessionBlockOwnerBridge(
        startupMap,
        authority.Snapshot,
        [new DefaultSessionBlockRegistry(), new AssemblyScanningSessionBlockRegistry()]);
});

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// The host withdraws the owner of the block: the next revision of the
// authority names no active owner, and every live circuit follows it.
app.MapPost("/withdraw", (ConsumerBlockAuthority authority) =>
{
    authority.Deliver(new SessionBlockOwnerSnapshot(2, []));

    return Results.NoContent();
});

// The host activates the owner again, so a browser can prove that a
// reactivation restores the block.
app.MapPost("/activate", (ConsumerBlockAuthority authority) =>
{
    authority.Deliver(new SessionBlockOwnerSnapshot(3, [ConsumerBlockAuthority.OwnerKey]));

    return Results.NoContent();
});

app.Run();
