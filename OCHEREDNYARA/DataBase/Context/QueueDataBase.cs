using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class QueueDataBase
{
    public static async Task<bool> AddQueues(long id, long idRoom)
    {
        if (await OrganizationDataBase.IsOrganizationActive(id))
        {
            await using ContextDataBase db = new();
            Queue queue = new() { RoomId = idRoom };
            await db.Queues.AddAsync(queue);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<List<Queue>> GetQueues(long id)
    {
        await using ContextDataBase db = new();

        List<Queue> queues = await db.Queues.Where(p => p.RoomId == id).ToListAsync();
        await db.Queues.Include(u => u.Windows).ToListAsync();
        return queues;
    }

    public static async Task<bool> DeleteQueue(long id)
    {
        await using ContextDataBase db = new();

        Queue? queue = await db.Queues.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (queue is not null)
        {
            db.Queues.Attach(queue);
            db.Queues.Remove(queue);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> FreezeUnfreeze(long id)
    {
        await using ContextDataBase db = new();

        Queue? queue = await db.Queues.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (queue is not null)
        {
            queue.Freezing = !queue.Freezing;
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<bool> Freeze(long id)
    {
        await using ContextDataBase db = new();

        Queue? queue = await db.Queues.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (queue is not null)
        {
            queue.Freezing = true;
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}