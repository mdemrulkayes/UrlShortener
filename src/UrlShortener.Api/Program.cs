using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Db;
using UrlShortener.Api.Dtos;
using UrlShortener.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddDbContextPool<ApplicationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("UrlShortenerDb"));
});

builder.Services.AddScoped<ShortenedUrlService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/openapi/v1.json", "Open API v1");
    });
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors(opt => opt
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

app.MapPost("shorten", async (ShortenedUrlRequest request, ShortenedUrlService service) =>
{
    if (!Uri.TryCreate(request.LongUrl, UriKind.Absolute, out _))
    {
        return Results.BadRequest("Invalid URL");
    }

    var shortCode = await service.ShortenUrlAsync(request.LongUrl);
    return Results.Created($"/{shortCode}", shortCode);
});

app.MapGet("{shortCode}", async (string shortCode, ShortenedUrlService service) =>
{
    var longUrl = await service.GetLongUrlAsync(shortCode);
    return longUrl is null ? Results.NotFound() : Results.Redirect(longUrl);
});

app.MapGet("urls", async (ShortenedUrlService urlService) => Results.Ok(await urlService.GetUrls()));

app.Run();
