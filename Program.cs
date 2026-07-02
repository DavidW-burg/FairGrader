var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// FG_FairGrader als Scoped Service registrieren
builder.Services.AddScoped<FG_FairGrader>();

var app = builder.Build();

// Configure the HTTP request pipeline...
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();