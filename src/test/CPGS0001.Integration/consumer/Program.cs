using LowlandTech.Components.Features.Pages;
using LowlandTech.PageConsumer;
using LowlandTech.PageConsumer.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// The authority of the host over the plugin owners is the only state the
// circuits share.
builder.Services.AddSingleton<ConsumerPageAuthority>();

// The activation snapshot of the consumer. The plugin stays installed, so its
// route keeps being declared; the authority of the host decides whether the
// declared page is published.
builder.Services.AddSingleton<ConsumerActivationSource>();

// The route host of a circuit: the bridge that projects the authority onto the
// route and page primitives of the Plugins shell.
builder.Services.AddScoped<ConsumerPageHost>();

var app = builder.Build();

// The static assets are mapped as endpoints, so they win over the catch-all
// route of the host. The static file middleware alone skips a request whose
// endpoint was already selected, and the catch-all route selects one for
// every path the host does not serve itself.
app.MapStaticAssets();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// The host withdraws the owner of the page: the next revision of the
// authority names no active owner, and every live circuit follows it.
app.MapPost("/withdraw", (ConsumerPageAuthority authority) =>
{
    authority.Deliver(new ComponentPageOwnerSnapshot(2, []));

    return Results.NoContent();
});

// The host activates the owner again, so a browser can prove that a
// reactivation restores the page.
app.MapPost("/activate", (ConsumerPageAuthority authority) =>
{
    authority.Deliver(new ComponentPageOwnerSnapshot(3, [ConsumerPageAuthority.OwnerKey]));

    return Results.NoContent();
});

// The protected action of the plugin page. The server authorizes it against
// the authority the server holds right now, so a browser projection that was
// rendered before the withdrawal cannot authorize it.
app.MapPost("/plugin-action", (ConsumerPageAuthority authority) =>
{
    if (!authority.IsOwnerActive(ConsumerPageAuthority.OwnerKey))
    {
        return Results.Json(
            new { outcome = "denied" },
            statusCode: StatusCodes.Status403Forbidden);
    }

    return Results.Json(new { outcome = "granted" });
});

app.Run();
