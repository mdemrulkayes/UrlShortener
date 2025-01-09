using Refit;
using UrlShortener.Models;

namespace UrlShortener.ApiServices;

public interface IUserShortenerService
{
    [Get("urls")]
    Task<IEnumerable<ShortenedUrl>> GetUrls();

    [Post("shorten")]
    Task<ShortenedUrl> ShortenUrl([Body] ShortenUrlRequest request);
}
