using MudBlazor;
using MudBlazor.Services;
using Refit;
using UrlShortener.ApiServices;
using UrlShortener.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(opt =>
{
    opt.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    opt.SnackbarConfiguration.PreventDuplicates = false;
    opt.SnackbarConfiguration.NewestOnTop = false;
    opt.SnackbarConfiguration.ShowCloseIcon = true;
    opt.SnackbarConfiguration.VisibleStateDuration = 5000;
    opt.SnackbarConfiguration.HideTransitionDuration = 500;
    opt.SnackbarConfiguration.ShowTransitionDuration = 500;
    opt.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddRefitClient<IUrlShortenerService>()
    .ConfigureHttpClient(config =>
    {
        config.BaseAddress = new Uri(builder.Configuration["UrlShortenerApi:Url"]!);
        config.DefaultRequestHeaders.Clear();
        config.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
