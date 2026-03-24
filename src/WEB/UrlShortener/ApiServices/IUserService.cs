using Refit;
using UrlShortener.Models;

namespace UrlShortener.ApiServices;

public interface IUserService
{
    [Get("/api/auth/me")]
    Task<CurrentUserResponse> GetCurrentUser();
}
