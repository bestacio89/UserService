using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Wallet;

/// <summary>
/// Crédite un montant sur le solde d'une monnaie donnée (par code, ex:
/// "SOFT", "HARD", ou toute monnaie ajoutée plus tard sans changement de
/// contrat). Si le joueur n'a encore jamais eu de solde pour cette monnaie
/// (ex: nouvel event token), le handler crée la ligne à zéro avant de créditer.
/// Reason est obligatoire (ex: "MatchReward", "DailyLogin", "IAPPurchase")
/// — sert de piste d'audit minimale tant qu'aucun grand livre de
/// transactions (UserWalletTransaction) n'existe.
/// </summary>
public sealed record CreditCurrencyCommand(
    Guid UserId,
    string CurrencyCode,
    int Amount,
    string Reason
) : ICommand<bool>;