using Server.Models;

namespace Server.Persistence.Contracts;

public interface IRoundsRepository
{
    Task SaveRound(Round round);
    Task<Round?> GetById(int roundId);
}
