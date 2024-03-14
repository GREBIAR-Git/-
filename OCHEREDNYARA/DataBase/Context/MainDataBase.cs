using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class MainDataBase
{
    public static string ConnectingString { get; set; } = string.Empty;

    public static async void ResetDB()
    {
        await using (ContextDataBase db = new())
        {
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        WindowDataBase.AutoCommittedDayOfWeek();
    }

    public static async Task<bool> ChangeThemeClient(long id, int number)
    {
        await using ContextDataBase db = new();

        Client? client = await db.Clients.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (client is not null)
        {
            client.Theme = number;
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }


    public static async Task<int> GetThemeClient(long id)
    {
        await using ContextDataBase db = new();

        Client? client = await db.Clients.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (client is not null)
        {
            return client.Theme;
        }

        return -1;
    }

    public static async Task<int> GetThemeOrganization(long id)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            return organization.Theme;
        }

        return -1;
    }

    public static async Task<string> GetThemeDataOrganization(long id)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null && organization.ThemeData is not null)
        {
            return organization.ThemeData;
        }

        return string.Empty;
    }


    public static async Task<bool> ChangeThemeOrganization(long id, int number)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            organization.Theme = number;
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> ChangeThemeDataOrganization(long id, string data)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            organization.ThemeData = data;
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> AddPartThemeDataOrganization(long id, string data)
    {
        await using ContextDataBase db = new();

        Organization? organization = await db.Organizations.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (organization is not null)
        {
            organization.ThemeData += "!" + data;
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}