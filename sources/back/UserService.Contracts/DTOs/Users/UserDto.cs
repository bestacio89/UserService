using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Users;

public sealed record UserDto
{
  public Guid Id { get; init; }
  public string Username { get; init; } = null!;
  public DateTime CreatedAt { get; init; }
  public string State { get; init; } = null!;
}