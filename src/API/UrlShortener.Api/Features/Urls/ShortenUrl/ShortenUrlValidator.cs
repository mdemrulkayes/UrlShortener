using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;

namespace UrlShortener.Api.Features.Urls.ShortenUrl;

public class ShortenUrlValidator : Validator<ShortenUrlRequest>
{
    public ShortenUrlValidator()
    {
        RuleFor(x => x.LongUrl)
            .NotEmpty().WithMessage("URL is required")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Must be a valid absolute URL");

        RuleFor(x => x.CustomAlias)
            .MaximumLength(50).WithMessage("Custom alias must be 50 characters or less")
            .Matches("^[a-zA-Z0-9-_]+$").WithMessage("Custom alias can only contain letters, numbers, hyphens, and underscores")
            .When(x => !string.IsNullOrEmpty(x.CustomAlias));

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future")
            .When(x => x.ExpiresAt.HasValue);
    }
}
