using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class ReservedDataBase
{
    public static async Task<bool> AddReserved(long idClient, long idWindow, DateTime dateTime)
    {
        await using ContextDataBase db = new();
        List<Reserved> favorites =
            await db.Reserveds.Where(p => p.WindowId == idWindow && p.Date == dateTime).ToListAsync();
        if (favorites.Count == 0)
        {
            await db.Reserveds.AddAsync(new Reserved
            {
                ClientId = idClient,
                WindowId = idWindow,
                Date = dateTime
            });
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> DeleteReserved(long idClient, long idWindow)
    {
        await using ContextDataBase db = new();
        Reserved? reserved = await db.Reserveds.Where(p => p.ClientId == idClient && p.WindowId == idWindow)
            .FirstOrDefaultAsync();
        if (reserved is not null)
        {
            db.Reserveds.Attach(reserved);
            db.Reserveds.Remove(reserved);

            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}