using Refit;
using UrlShortener.Models;

namespace UrlShortener.ApiServices;

public interface IUrlShortenerService
{
    [Get("/api/urls")]
    Task<IEnumerable<ShortenedUrl>> GetUrls();

    [Post("/api/urls")]
    Task<ShortenedUrl> ShortenUrl([Body] ShortenUrlRequest request);

    [Put("/api/urls/{id}")]
    Task UpdateUrl(long id, [Body] UpdateUrlRequest request);

    [Delete("/api/urls/{id}")]
    Task DeleteUrl(long id);

    [Patch("/api/urls/{id}/toggle")]
    Task ToggleUrl(long id);

    [Get("/api/urls/{id}/qrcode")]
    Task<HttpResponseMessage> GetQrCode(long id);

    [Get("/api/analytics/dashboard")]
    Task<DashboardResponse> GetDashboard();

    [Get("/api/analytics/urls/{id}")]
    Task<UrlStatsResponse> GetUrlStats(long id);
}
