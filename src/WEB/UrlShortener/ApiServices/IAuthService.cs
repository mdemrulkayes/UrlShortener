using Refit;
using UrlShortener.Models;

namespace UrlShortener.ApiServices;

public interface IAuthService
{
    [Post("/api/auth/login")]
    Task<LoginResponse> Login([Body] LoginRequest request);

    [Post("/api/auth/register")]
    Task<RegisterResponse> Register([Body] RegisterRequest request);
}
