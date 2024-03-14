using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class ClientDataBase
{
    public static async Task<List<Reserved>> GetReserved(long idClient)
    {
        await using ContextDataBase db = new();
        return await db.Reserveds.Where(p => p.ClientId == idClient).ToListAsync();
    }

    public static async Task<bool> IsClientActive(long id)
    {
        await using ContextDataBase db = new();

        return await db.Clients.AnyAsync(o => o.Id == id);
    }

    public static async Task<bool> AddClient(long id)
    {
        await using ContextDataBase db = new();

        if (!await IsClientActive(id))
        {
            Client client = new() { Id = id };
            await db.Clients.AddAsync(client);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }


    public static async Task<bool> EditLevel(long id)
    {
        await using ContextDataBase db = new();

        Client? client = await db.Clients.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (client is not null && client.Level == 0)
        {
            client.Level = 1;
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}