using Server.Persistence.Contracts;

namespace Server.Actions;

public sealed record JoinGameParams(string PlayerName, int GameId) : CreatePlayerParams(PlayerName, GameId);

public class JoinGameValidator(
  IGamesRepository gamesRepository,
  IPlayersRepository playersRepository
) : CreatePlayerValidator(
  gamesRepository,
  playersRepository
);

public class JoinGame(
  IGamesRepository gamesRepository,
  IPlayersRepository playersRepository
) : CreatePlayer(
  gamesRepository,
  playersRepository
);
