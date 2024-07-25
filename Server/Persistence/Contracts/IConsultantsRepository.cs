using Server.Models;

namespace Server.Persistence.Contracts;

public interface IConsultantsRepository
{
    Task SaveConsultant(Consultant consultant);
}
