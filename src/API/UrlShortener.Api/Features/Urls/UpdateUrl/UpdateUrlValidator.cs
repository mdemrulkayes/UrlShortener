using FastEndpoints;
using FluentValidation;

namespace UrlShortener.Api.Features.Urls.UpdateUrl;

public class UpdateUrlValidator : Validator<UpdateUrlRequest>
{
    public UpdateUrlValidator()
    {
        RuleFor(x => x.LongUrl)
            .NotEmpty().WithMessage("URL is required")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Must be a valid absolute URL");

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future")
            .When(x => x.ExpiresAt.HasValue);
    }
}
