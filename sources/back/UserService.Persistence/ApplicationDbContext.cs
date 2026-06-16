using Microsoft.EntityFrameworkCore;
using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;

using UserService.Domain.Users;
using UserService.Domain.Ranked;
using UserService.Domain.Progression;
using UserService.Domain.Identity;

namespace UserService.Persistence;

public class ApplicationDbContext : DbContextBase
{
  public ApplicationDbContext(
      DbContextOptions<ApplicationDbContext> options,
      IDispatcher dispatcher
  ) : base(options, dispatcher)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // =========================
    // USER
    // =========================
    modelBuilder.Entity<User>(builder =>
    {
      builder.Property(x => x.Username)
          .IsRequired()
          .HasMaxLength(50);

      builder.Property(x => x.State)
          .IsRequired();

      builder.Property(x => x.StatusReason)
          .HasMaxLength(500);

      builder.Property(x => x.SuspendedUntil);
    });

    // =========================
    // USER RANK
    // =========================
    modelBuilder.Entity<UserRank>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.MMR).IsRequired();
      builder.Property(x => x.Wins).IsRequired();
      builder.Property(x => x.Losses).IsRequired();

      builder.HasIndex(x => x.UserId);
    });

    // =========================
    // HERO MASTERY
    // =========================
    modelBuilder.Entity<UserHeroMastery>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.HeroId).IsRequired();

      builder.HasIndex(x => new { x.UserId, x.HeroId });
    });

    // =========================
    // CLASS MASTERY
    // =========================
    modelBuilder.Entity<UserClassMastery>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.HeroClassId).IsRequired();

      builder.HasIndex(x => new { x.UserId, x.HeroClassId });
    });

    // =========================
    // IDENTITY
    // =========================
    modelBuilder.Entity<UserIdentity>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.Provider).IsRequired();
      builder.Property(x => x.ExternalId).IsRequired();

      builder.HasIndex(x => new { x.Provider, x.ExternalId })
             .IsUnique();
    });
  }

  // =========================
  // DBSets
  // =========================
  public DbSet<User> Users => Set<User>();
  public DbSet<UserRank> UserRanks => Set<UserRank>();
  public DbSet<UserHeroMastery> UserHeroMasteries => Set<UserHeroMastery>();
  public DbSet<UserClassMastery> UserClassMasteries => Set<UserClassMastery>();
  public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();
}