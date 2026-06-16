using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Domain.Users;

public enum UserAccountState
{
  Active,
  Suspended,
  Banned
}