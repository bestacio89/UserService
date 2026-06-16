using Franz.Common.Business.Domain;

namespace UserService.Domain.Ranked;

public sealed class UserRank : Entity<Guid>
{
  public Guid UserId { get; private set; }

  public int MMR { get; private set; }

  public int Wins { get; private set; }

  public int Losses { get; private set; }

  public void ApplyMatchResult(bool win, int mmrDelta)
  {
    if (win)
    {
      Wins++;
      MMR += mmrDelta;
    }
    else
    {
      Losses++;
      MMR -= mmrDelta;
    }

    if (MMR < 0) MMR = 0;
  }
}