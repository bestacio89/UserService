using Franz.Common.Business.Domain;

namespace UserService.Domain.Progression;

public sealed class UserHeroMastery : Entity<Guid>
{
  public Guid UserId { get; private set; }

  public Guid HeroId { get; private set; }

  public int MasteryLevel { get; private set; }

  public int Experience { get; private set; }

  public int GamesPlayed { get; private set; }

  public void RegisterGame(bool win, int xpGain)
  {
    GamesPlayed++;
    Experience += xpGain;

    if (Experience >= RequiredXpForNextLevel())
    {
      MasteryLevel++;
      Experience = 0;
    }
  }

  private int RequiredXpForNextLevel()
      => 120 + (MasteryLevel * 30);
}