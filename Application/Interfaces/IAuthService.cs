using RESTfullAPI.Infrastructure.Identity;

namespace RESTfullAPI.Application.Interfaces;

public interface IAuthService
{
    Task<(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc)> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc)> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}

