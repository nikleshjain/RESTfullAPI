using Microsoft.AspNetCore.Mvc;
using RESTfullAPI.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RESTfullAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[SwaggerTag("Authentication endpoints")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    public record LoginRequest(string Username, string Password);
    public record TokenResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);
    public record RefreshRequest(string RefreshToken);

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login", Description = "Authenticate and obtain JWT tokens")]
    [SwaggerResponse(200, "Login succeeded", typeof(TokenResponse))]
    [SwaggerResponse(401, "Invalid credentials")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var (accessToken, refreshToken, expiresAtUtc) = await _authService.LoginAsync(request.Username, request.Password);
            return Ok(new TokenResponse(accessToken, refreshToken, expiresAtUtc));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("refresh")]
    [SwaggerOperation(Summary = "Refresh token", Description = "Refresh access token using a refresh token")]
    [SwaggerResponse(200, "Refresh succeeded", typeof(TokenResponse))]
    [SwaggerResponse(401, "Invalid or expired refresh token")]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            var (accessToken, refreshToken, expiresAtUtc) = await _authService.RefreshAsync(request.RefreshToken);
            return Ok(new TokenResponse(accessToken, refreshToken, expiresAtUtc));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("revoke")]
    [SwaggerOperation(Summary = "Revoke refresh token", Description = "Revoke a refresh token")]
    [SwaggerResponse(204, "Revoked")]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequest request)
    {
        await _authService.RevokeAsync(request.RefreshToken);
        return NoContent();
    }
}

