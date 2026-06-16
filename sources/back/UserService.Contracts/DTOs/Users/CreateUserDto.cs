using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Users;


public sealed record CreateUserDto
{
  public string Username { get; init; } = null!;
}
