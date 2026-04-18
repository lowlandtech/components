using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LowlandTech.Components.App;
using LowlandTech.Components.App.Services;
using LowlandTech.Components.App.Showcode;
using LowlandTech.Components.Sonner;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<ISonnerService, SonnerService>();
builder.Services.AddScoped<ShowcodeState>();

StoryRegistry.RegisterAllStories(typeof(App).Assembly);

await builder.Build().RunAsync();
