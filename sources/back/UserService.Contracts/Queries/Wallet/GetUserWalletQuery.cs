using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Wallet;

namespace UserService.Contracts.Queries.Wallet;

/// <summary>
/// Retourne tous les soldes du joueur (une entrée par monnaie), même forme
/// que GetUserHeroMasteriesQuery côté Progression.
/// </summary>
public sealed record GetUserWalletQuery(
    Guid UserId
) : IQuery<IReadOnlyList<UserWalletBalanceDto>>;