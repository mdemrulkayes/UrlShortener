using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Features.Auth.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        Group<AuthGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email,
            FullName = req.FullName
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            await SendAsync(new RegisterResponse
            {
                Success = false,
                Message = "Registration failed",
                Errors = result.Errors.Select(e => e.Description)
            }, 400, ct);
            return;
        }

        await SendAsync(new RegisterResponse
        {
            Success = true,
            Message = "User registered successfully"
        }, 201, ct);
    }
}
