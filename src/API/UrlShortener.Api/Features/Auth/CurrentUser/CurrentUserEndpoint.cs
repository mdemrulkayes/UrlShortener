using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Features.Auth.CurrentUser;

public class CurrentUserEndpoint : EndpointWithoutRequest<CurrentUserResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CurrentUserEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Get("/api/auth/me");
        Group<AuthGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new CurrentUserResponse
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName
        }, cancellation: ct);
    }
}
