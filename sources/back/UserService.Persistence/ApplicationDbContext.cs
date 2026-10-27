using Microsoft.EntityFrameworkCore;
using Franz.Common.Business.Domain;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Dispatchers;
using UserService.Domain.Authentication;
using UserService.Domain.Users;
using UserService.Domain.Ranked;
using UserService.Domain.Progression;
using UserService.Domain.Identity;
using UserService.Domain.Wallet;

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

    modelBuilder.Entity<User>(builder =>
    {
      builder.Property(x => x.Username).IsRequired().HasMaxLength(50);
      builder.Property(x => x.State).IsRequired();
      builder.Property(x => x.StatusReason).HasMaxLength(500);
      builder.Property(x => x.SuspendedUntil);
    });

    modelBuilder.Entity<UserSession>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.DeviceId).HasMaxLength(200);
      builder.Property(x => x.RefreshTokenHash).IsRequired().HasMaxLength(64).IsConcurrencyToken();
      builder.Property(x => x.CreatedAt).IsRequired();
      builder.Property(x => x.RefreshTokenExpiresAt).IsRequired();
      builder.Property(x => x.RevokedAt);
      builder.Property(x => x.LastRotatedAt);
      builder.HasIndex(x => x.UserId);
      builder.HasIndex(x => x.RefreshTokenHash).IsUnique();
    });

    modelBuilder.Entity<UserRank>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.MMR).IsRequired();
      builder.Property(x => x.Wins).IsRequired();
      builder.Property(x => x.Losses).IsRequired();
      builder.HasIndex(x => x.UserId);
    });

    modelBuilder.Entity<UserHeroMastery>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.HeroId).IsRequired();
      builder.HasIndex(x => new { x.UserId, x.HeroId });
    });

    modelBuilder.Entity<UserClassMastery>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.HeroClassId).IsRequired();
      builder.HasIndex(x => new { x.UserId, x.HeroClassId });
    });

    modelBuilder.Entity<Currency>(builder =>
    {
      builder.Property(x => x.Code).IsRequired().HasMaxLength(30);
      builder.Property(x => x.DisplayName).IsRequired().HasMaxLength(100);
      builder.Property(x => x.IsPremium).IsRequired();
      builder.Property(x => x.IsActive).IsRequired();
      builder.HasIndex(x => x.Code).IsUnique();
    });

    modelBuilder.Entity<UserWalletBalance>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.CurrencyId).IsRequired();
      builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(30);
      builder.Property(x => x.CurrencyDisplayName).IsRequired().HasMaxLength(100);
      builder.Property(x => x.Amount).IsRequired();
      builder.HasIndex(x => new { x.UserId, x.CurrencyId }).IsUnique();
    });

    modelBuilder.Entity<UserIdentity>(builder =>
    {
      builder.Property(x => x.UserId).IsRequired();
      builder.Property(x => x.Provider).IsRequired();
      builder.Property(x => x.ExternalId).IsRequired();
      builder.HasIndex(x => new { x.Provider, x.ExternalId }).IsUnique();
    });
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<UserSession> UserSessions => Set<UserSession>();
  public DbSet<UserRank> UserRanks => Set<UserRank>();
  public DbSet<UserHeroMastery> UserHeroMasteries => Set<UserHeroMastery>();
  public DbSet<UserClassMastery> UserClassMasteries => Set<UserClassMastery>();
  public DbSet<UserIdentity> UserIdentities => Set<UserIdentity>();
  public DbSet<Currency> Currencies => Set<Currency>();
  public DbSet<UserWalletBalance> UserWalletBalances => Set<UserWalletBalance>();
}