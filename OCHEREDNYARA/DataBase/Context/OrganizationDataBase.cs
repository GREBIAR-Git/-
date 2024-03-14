using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class OrganizationDataBase
{
    public static async Task<List<Room>> SearchNameOrg(string name)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Name == name).FirstOrDefaultAsync();
        await db.Organizations.Include(u => u.Rooms).ToListAsync();
        List<Organization> organizations = await db.Organizations.ToListAsync();
        if (organization?.Rooms is not null)
        {
            await db.Rooms.Include(u => u.Queues).ToListAsync();
            await db.Rooms.Include(u => u.Favorites).ToListAsync();
            return organization.Rooms;
        }

        return [];
    }

    public static async Task<List<Room>> SearchIdOrg(long id)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        await db.Organizations.Include(u => u.Rooms).ToListAsync();

        if (organization?.Rooms != null)
        {
            await db.Rooms.Include(u => u.Favorites).ToListAsync();
            await db.Rooms.Include(u => u.Queues).ToListAsync();
            return organization.Rooms;
        }

        return [];
    }

    public static async Task<bool> EditOrganizationName(long id, string newName)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            Organization? organization1 = await db.Organizations.Where(p => p.Name == newName).FirstOrDefaultAsync();
            if (organization1 == null)
            {
                organization.Name = newName;
                await db.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }

    public static async Task<bool> IsOrganizationActive(long id)
    {
        await using ContextDataBase db = new();

        return await db.Organizations.AnyAsync(o => o.Id == id);
    }

    public static async Task<bool> AddOrganization(long id, string name)
    {
        await using ContextDataBase db = new();

        if (!await IsOrganizationActive(id))
        {
            Organization organization = new() { Id = id, Name = name };
            await db.Organizations.AddAsync(organization);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }


    public static async Task<Organization?> GetOrganization(long id)
    {
        await using ContextDataBase db = new();

        return await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
    }
}