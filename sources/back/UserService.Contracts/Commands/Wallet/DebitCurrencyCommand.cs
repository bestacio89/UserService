using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Wallet;

/// <summary>
/// Débite un montant du solde d'une monnaie donnée (par code). Retourne
/// false si le solde est insuffisant OU si le joueur n'a jamais eu de
/// solde pour cette monnaie — ce n'est pas une erreur serveur, c'est un
/// résultat métier attendu (ex: le Shop doit gérer ce cas sans planter).
/// </summary>
public sealed record DebitCurrencyCommand(
    Guid UserId,
    string CurrencyCode,
    int Amount,
    string Reason
) : ICommand<bool>;