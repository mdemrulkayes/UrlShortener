using FluentValidation;

namespace UrlShortener.Dtos;

public record ShortenedUrlRequest(string LongUrl);

public class ShortenedUrlRequestValidator : AbstractValidator<ShortenedUrlRequest>
{
    public ShortenedUrlRequestValidator()
    {
        RuleFor(x => x.LongUrl)
            .NotEmpty()
            .WithMessage("LongUrl is required")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("LongUrl must be a valid URL");
    }
}
