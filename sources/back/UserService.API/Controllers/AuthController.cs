using Microsoft.AspNetCore.Mvc;
using UserService.Application.Commands.Authentication;
using UserService.Application.Commands.Authentication.Services;
using UserService.Contracts.Commands.Authentication;
using UserService.Contracts.DTOs.Authentication;
using UserService.Domain.Identity;

namespace UserService.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
  private readonly IAuthenticationService _authService;

  public AuthController(IAuthenticationService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  public async Task<ActionResult<LoginResponse>> Login(
      [FromBody] LoginRequest request,
      CancellationToken ct)
  {
    if (!Enum.TryParse<IdentityProvider>(request.Provider, ignoreCase: true, out var provider)
        || !Enum.IsDefined(provider))
    {
      return BadRequest("Unsupported identity provider.");
    }

    try
    {
      var command = new LoginUserCommand(
          Provider: provider,
          Identifier: request.Identifier,
          Secret: request.Secret,
          DeviceId: request.DeviceId);

      var result = await _authService.LoginAsync(command, ct);
      return Ok(ToResponse(result));
    }
    catch (UnauthorizedAccessException)
    {
      return Unauthorized();
    }
  }

  [HttpPost("refresh")]
  public async Task<ActionResult<LoginResponse>> Refresh(
      [FromBody] RefreshTokenRequest request,
      CancellationToken ct)
  {
    try
    {
      var result = await _authService.RefreshAsync(request.RefreshToken, ct);
      return Ok(ToResponse(result));
    }
    catch (UnauthorizedAccessException)
    {
      return Unauthorized();
    }
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout(
      [FromBody] RefreshTokenRequest request,
      CancellationToken ct)
  {
    try
    {
      await _authService.LogoutAsync(request.RefreshToken, ct);
      return NoContent();
    }
    catch (UnauthorizedAccessException)
    {
      return Unauthorized();
    }
  }

  private static LoginResponse ToResponse(LoginResult result)
      => new(result.UserId, result.AccessToken, result.RefreshToken, result.ExpiresAt);
}