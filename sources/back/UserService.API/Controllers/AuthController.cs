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
    var command = new LoginUserCommand(
        Provider: Enum.Parse<IdentityProvider>(request.Provider, ignoreCase: true),
        Identifier: request.Identifier,
        Secret: request.Secret,
        DeviceId: request.DeviceId
    );

    var result = await _authService.LoginAsync(command, ct);

    return Ok(new LoginResponse(
        result.UserId,
        result.AccessToken,
        result.RefreshToken,
        result.ExpiresAt
    ));
  }
}