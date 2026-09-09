using Franz.Common.Business.Domain.Factories;
using System.Linq;
using UserService.Domain.Wallet;

namespace UserService.Persistence.Seeders;

/// <summary>
/// Seed les deux monnaies de base (SOFT, HARD) au démarrage en dev, si elles
/// n'existent pas déjà. Ajouter une monnaie plus tard (event token,
/// battle-pass points) n'exige aucun changement de code — juste une entrée
/// Currency de plus, ici ou via un futur outil d'admin.
/// </summary>
public sealed class CurrencySeeder : ISeeder
{
  private readonly ApplicationDbContext _db;
  private readonly IEntityFactory<Guid, Currency> _currencyFactory;

  // En premier : Wallet et le futur Shop dépendent du catalogue Currency.
  public int Order => 1;

  public CurrencySeeder(
      ApplicationDbContext db,
      IEntityFactory<Guid, Currency> currencyFactory)
  {
    _db = db;
    _currencyFactory = currencyFactory;
  }

  public async Task SeedAsync(CancellationToken cancellation)
  {
    if (_db.Currencies.Any())
    {
      return;
    }

    var soft = _currencyFactory.Create();
    soft.Define("SOFT", "Gold", isPremium: false);

    var hard = _currencyFactory.Create();
    hard.Define("HARD", "Gems", isPremium: true);

    await _db.Currencies.AddRangeAsync(new[] { soft, hard }, cancellation);
    await _db.SaveChangesAsync(cancellation);
  }
}