using Franz.Common.Business.Domain;

namespace UserService.Domain.Progression;

public sealed class UserClassMastery : Entity<Guid>
{
  public Guid UserId { get; private set; }

  public Guid HeroClassId { get; private set; }

  public int MasteryLevel { get; private set; }

  public int Experience { get; private set; }

  public void AddExperience(int xp)
  {
    Experience += xp;

    // simple deterministic progression rule (can evolve later)
    if (Experience >= RequiredXpForNextLevel())
    {
      MasteryLevel++;
      Experience = 0;
    }
  }

  private int RequiredXpForNextLevel()
      => 100 + (MasteryLevel * 25);
}