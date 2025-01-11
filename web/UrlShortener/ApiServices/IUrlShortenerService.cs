using Refit;
using UrlShortener.Models;

namespace UrlShortener.ApiServices;

public interface IUrlShortenerService
{
    [Get("/urls")]
    Task<IEnumerable<ShortenedUrl>> GetUrls();

    [Post("/shorten")]
    Task<ShortenedUrl> ShortenUrl([Body] ShortenUrlRequest request);
}
