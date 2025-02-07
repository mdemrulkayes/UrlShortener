using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models;

public class ShortenUrlRequest
{
    [Required(ErrorMessage = "URL can not be empty")]
    [Url(ErrorMessage = "Invalid URL")]
    public string? LongUrl { get; set; }
}
