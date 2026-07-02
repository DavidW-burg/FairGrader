using FairGraderApp;
using FairGraderApp.Components;
using FairGraderApp.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// FG_FairGrader registrieren
builder.Services.AddScoped<FG_FairGrader>();

// Blazor Bootstrap
builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync();
