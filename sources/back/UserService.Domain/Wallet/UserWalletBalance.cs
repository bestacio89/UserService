namespace UserService.Domain.Wallet;

/// <summary>
/// Solde d'un joueur pour UNE monnaie — même forme que UserHeroMastery
/// (UserId, HeroId) / UserClassMastery (UserId, HeroClassId) : une ligne
/// plate par clé, pas un objet Wallet englobant. Ajouter une monnaie ne
/// touche ni ce type ni les handlers — seul le catalogue Currency grandit.
/// </summary>
public sealed class UserWalletBalance : Entity<Guid>
{
  public Guid UserId { get; private set; }

  public Guid CurrencyId { get; private set; }

  /// <summary>
  /// Dénormalisé depuis Currency au moment de la création — évite une
  /// jointure sur la lecture la plus chaude (barre de statut du Home).
  /// CurrencyId reste la clé d'intégrité référentielle ; ces deux champs
  /// sont en lecture seule après Define().
  /// </summary>
  public string CurrencyCode { get; private set; } = string.Empty;
  public string CurrencyDisplayName { get; private set; } = string.Empty;

  public int Amount { get; private set; }

  private UserWalletBalance() { }

  public void Define(Guid userId, Guid currencyId, string currencyCode, string currencyDisplayName)
  {
    UserId = userId;
    CurrencyId = currencyId;
    CurrencyCode = currencyCode;
    CurrencyDisplayName = currencyDisplayName;
    Amount = 0;

    MarkCreated("system");
  }

  public void Credit(int amount, string reason)
  {
    EnsurePositiveAmount(amount);
    EnsureReason(reason);

    Amount += amount;

    MarkUpdated("system");
  }

  /// <returns>false si le solde est insuffisant — un débit refusé n'est pas une erreur serveur, c'est un résultat métier attendu (ex: achat en boutique).</returns>
  public bool Debit(int amount, string reason)
  {
    EnsurePositiveAmount(amount);
    EnsureReason(reason);

    if (amount > Amount)
    {
      return false;
    }

    Amount -= amount;

    MarkUpdated("system");
    return true;
  }

  private static void EnsurePositiveAmount(int amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentException(
          "Amount must be strictly positive.",
          nameof(amount));
    }
  }

  private static void EnsureReason(string reason)
  {
    if (string.IsNullOrWhiteSpace(reason))
    {
      throw new ArgumentException(
          "A reason is required for every currency mutation (audit trail).",
          nameof(reason));
    }
  }
}