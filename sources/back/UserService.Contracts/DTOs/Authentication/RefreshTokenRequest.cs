using System.ComponentModel.DataAnnotations;

namespace UserService.Contracts.DTOs.Authentication;

public sealed record RefreshTokenRequest(
    [property: Required] string RefreshToken);