
using Server.Models;
using Server.Persistence.Contracts;

namespace Server.Persistence;

public class ConsultantsRepository(WssDbContext context) : IConsultantsRepository
{
    public async Task SaveConsultant(Consultant consultant)
    {
        if (consultant.Id is null)
        {
            await context.AddAsync(consultant);
        }

        await context.SaveChangesAsync();
    }
}
