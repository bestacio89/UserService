namespace UserService.Domain.Wallet;

/// <summary>
/// Entrée de catalogue pour un type de monnaie (ex: "SOFT", "HARD", et à
/// terme n'importe quel jeton d'event/battle-pass). Ajouter une monnaie
/// devient une ligne de donnée, pas une migration de schéma.
/// </summary>
public sealed class Currency : Entity<Guid>
{
  public string Code { get; private set; } = string.Empty;

  public string DisplayName { get; private set; } = string.Empty;

  /// <summary>
  /// True pour toute monnaie obtenue via achat argent réel (validation de
  /// reçu, remboursements, conformité store) — ex: HARD. False pour une
  /// monnaie uniquement gagnable en jeu — ex: SOFT.
  /// </summary>
  public bool IsPremium { get; private set; }

  /// <summary>
  /// Permet de retirer une monnaie de la circulation (event terminé) sans
  /// supprimer l'historique des soldes déjà attribués.
  /// </summary>
  public bool IsActive { get; private set; }

  private Currency() { }

  public void Define(string code, string displayName, bool isPremium)
  {
    if (string.IsNullOrWhiteSpace(code))
    {
      throw new ArgumentException("Currency code is required.", nameof(code));
    }

    if (string.IsNullOrWhiteSpace(displayName))
    {
      throw new ArgumentException("Currency display name is required.", nameof(displayName));
    }

    Code = code.Trim().ToUpperInvariant();
    DisplayName = displayName;
    IsPremium = isPremium;
    IsActive = true;

    MarkCreated("system");
  }

  public void Deactivate()
  {
    IsActive = false;
    MarkUpdated("system");
  }

  public void Reactivate()
  {
    IsActive = true;
    MarkUpdated("system");
  }
}