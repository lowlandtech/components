using LowlandTech.Components;
using LowlandTech.Components.Consumer.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddLocalization();
builder.Services.AddScoped<IPageOverlayService, PageOverlayService>();
builder.Services.AddScoped<IPopoverService, PopoverService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
