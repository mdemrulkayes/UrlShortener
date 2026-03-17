using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using Refit;
using UrlShortener.ApiServices;
using UrlShortener.Auth;
using UrlShortener.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

// Auth
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthHeaderHandler>();

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

var apiBaseUrl = builder.Configuration["UrlShortenerApi:Url"]!;

builder.Services.AddRefitClient<IUrlShortenerService>()
    .ConfigureHttpClient(config =>
    {
        config.BaseAddress = new Uri(apiBaseUrl);
        config.DefaultRequestHeaders.Clear();
        config.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    });

builder.Services.AddRefitClient<IAuthService>()
    .ConfigureHttpClient(config =>
    {
        config.BaseAddress = new Uri(apiBaseUrl);
        config.DefaultRequestHeaders.Clear();
        config.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
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
