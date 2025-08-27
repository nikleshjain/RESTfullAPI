using Microsoft.Extensions.Options;
using RESTfullAPI.Application.Interfaces;
using RESTfullAPI.Infrastructure.Identity;

namespace RESTfullAPI.Application.Services;

public class AuthService : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly JwtOptions _jwtOptions;
    private readonly IEnumerable<DemoUser> _demoUsers;

    public AuthService(
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenStore refreshTokenStore,
        IOptions<JwtOptions> jwtOptions,
        IEnumerable<DemoUser> demoUsers)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenStore = refreshTokenStore;
        _jwtOptions = jwtOptions.Value;
        _demoUsers = demoUsers;
    }

    public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc)> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = _demoUsers.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var now = DateTime.UtcNow;
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Username, user.Roles, now, out var expiresAtUtc);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        await _refreshTokenStore.StoreAsync(refreshToken, user.Username, now.AddDays(_jwtOptions.RefreshTokenDays), cancellationToken);

        return (accessToken, refreshToken, expiresAtUtc);
    }

    public async Task<(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc)> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var (found, username, expiresAtUtc) = await _refreshTokenStore.GetAsync(refreshToken, cancellationToken);
        if (!found || expiresAtUtc < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Rotate refresh token
        await _refreshTokenStore.RevokeAsync(refreshToken, cancellationToken);

        var user = _demoUsers.First(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        var now = DateTime.UtcNow;
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Username, user.Roles, now, out var accessExpiresAtUtc);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        await _refreshTokenStore.StoreAsync(newRefreshToken, user.Username, now.AddDays(_jwtOptions.RefreshTokenDays), cancellationToken);

        return (accessToken, newRefreshToken, accessExpiresAtUtc);
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return _refreshTokenStore.RevokeAsync(refreshToken, cancellationToken);
    }
}

