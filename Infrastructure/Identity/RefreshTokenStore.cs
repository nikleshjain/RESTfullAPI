namespace RESTfullAPI.Infrastructure.Identity;

public interface IRefreshTokenStore
{
    Task StoreAsync(string refreshToken, string username, DateTime expiresAtUtc, CancellationToken cancellationToken = default);
    Task<(bool Found, string Username, DateTime ExpiresAtUtc)> GetAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default);
}

public class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly Dictionary<string, (string Username, DateTime ExpiresAtUtc)> _store = new();
    private readonly object _gate = new();

    public Task StoreAsync(string refreshToken, string username, DateTime expiresAtUtc, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            _store[refreshToken] = (username, expiresAtUtc);
        }
        return Task.CompletedTask;
    }

    public Task<(bool Found, string Username, DateTime ExpiresAtUtc)> GetAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (_store.TryGetValue(refreshToken, out var value))
            {
                return Task.FromResult((true, value.Username, value.ExpiresAtUtc));
            }
        }
        return Task.FromResult((false, string.Empty, DateTime.MinValue));
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            _store.Remove(refreshToken);
        }
        return Task.CompletedTask;
    }
}

