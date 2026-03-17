using System.Security.Claims;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Urls.GetQrCode;

public class GetQrCodeEndpoint : EndpointWithoutRequest
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;

    public GetQrCodeEndpoint(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public override void Configure()
    {
        Get("/api/urls/{id}/qrcode");
        Group<UrlsGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<long>("id");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var url = await _db.ShortenedUrls
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

        if (url is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var baseUrl = _config["AppSettings:BaseUrl"] ?? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
        var fullUrl = $"{baseUrl}/{url.ShortCode}";

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(fullUrl, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(10);

        await SendBytesAsync(qrCodeBytes, contentType: "image/png", cancellation: ct);
    }
}
