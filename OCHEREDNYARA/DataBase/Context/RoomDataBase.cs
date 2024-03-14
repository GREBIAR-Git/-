using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class RoomDataBase
{
    public static async Task<List<Room>> GetRooms(long id)
    {
        await using ContextDataBase db = new();

        List<Room> rooms = await db.Rooms.Include(u => u.Queues).Where(p => p.OwnerId == id).ToListAsync();
        return rooms;
    }

    public static async Task<Room?> GetRoom(long id)
    {
        await using ContextDataBase db = new();

        return await db.Rooms.Include(u => u.Queues).Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public static async Task<bool> DeleteRoom(long id)
    {
        await using ContextDataBase db = new();

        Room? room = await db.Rooms.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (room is not null)
        {
            db.Rooms.Attach(room);
            db.Rooms.Remove(room);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> AddRooms(long id, string name)
    {
        if (await OrganizationDataBase.IsOrganizationActive(id))
        {
            await using ContextDataBase db = new();
            Room room = new() { OwnerId = id, Name = name };
            await db.Rooms.AddAsync(room);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> EditRoomName(long id, string newName)
    {
        await using ContextDataBase db = new();

        Room? room = await db.Rooms.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (room is not null)
        {
            Room? room1 = await db.Rooms.Where(p => p.Name == newName).FirstOrDefaultAsync();
            if (room1 is null)
            {
                room.Name = newName;
                await db.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }

    public static async Task<List<Room>> GetFavoritesRoom(long id)
    {
        await using ContextDataBase db = new();

        List<Favorite> favorites = await db.Favorites.Where(p => p.ClientId == id).ToListAsync();
        List<Room> rooms = [];
        foreach (Favorite favorite in favorites)
        {
            Room? room = await db.Rooms.Include(r => r.Organization).Where(p => p.Id == favorite.RoomId)
                .FirstOrDefaultAsync();
            if (room is not null)
            {
                rooms.Add(room);
            }
        }

        return rooms;
    }

    public static async Task<bool> AddFavorite(long idClient, long idRoom)
    {
        await using ContextDataBase db = new();
        List<Favorite> favorites =
            await db.Favorites.Where(p => p.ClientId == idClient && p.RoomId == idRoom).ToListAsync();
        if (favorites.Count == 0)
        {
            await db.Favorites.AddAsync(new Favorite
            {
                ClientId = idClient,
                RoomId = idRoom
            });
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> DeleteFavorite(long idClient, long idRoom)
    {
        await using ContextDataBase db = new();
        Favorite? favorite =
            await db.Favorites.Where(p => p.ClientId == idClient && p.RoomId == idRoom).FirstOrDefaultAsync();
        if (favorite is not null)
        {
            db.Favorites.Attach(favorite);
            db.Favorites.Remove(favorite);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}