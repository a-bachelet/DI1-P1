using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record JoinGameParams(string PlayerName, string CompanyName, int GameId) :
  CreatePlayerParams(PlayerName, CompanyName, GameId);

public class JoinGameValidator(
  IGamesRepository gamesRepository,
  IPlayersRepository playersRepository
) : CreatePlayerValidator(
  gamesRepository,
  playersRepository
);

public class JoinGame(
  ICompaniesRepository companiesRepository,
  IGamesRepository gamesRepository,
  IPlayersRepository playersRepository
) : CreatePlayer(
  companiesRepository,
  gamesRepository,
  playersRepository
);
