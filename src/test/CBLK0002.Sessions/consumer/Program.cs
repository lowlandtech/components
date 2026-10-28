using LowlandTech.Components;
using LowlandTech.Components.SessionRouteConsumer.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddLocalization();

// The session surface resolves the agent graph services of the session that it
// renders, so the host registers the discovery of them.
builder.Services.AddScoped<IGraphDiscoveryService, GraphDiscoveryService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
