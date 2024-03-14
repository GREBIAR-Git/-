using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Context;

public class WindowDataBase
{
    public static async Task<List<Reserved>> GetReserved(long idWindow)
    {
        await using ContextDataBase db = new();
        return await db.Reserveds.Where(p => p.WindowId == idWindow).ToListAsync();
    }


    public static async Task<bool> IsValidTime(long idQueue, long idWeekDay, DateTime? start, DateTime? end)
    {
        List<Window> windows = await GetWindows(idQueue, idWeekDay);
        if (start < end)
        {
            return windows.All(window =>
                (start < window.Start && end <= window.Start) || (start >= window.End && end > window.End));
        }

        return false;
    }

    public static async Task<bool> IsValidTime(long idWindow, DateTime? start, DateTime? end)
    {
        Window? oldWindow = await GetWindow(idWindow);
        if (oldWindow is not null)
        {
            List<Window> windows = await GetWindows(oldWindow.QueueId, oldWindow.WeekDayId);
            if (start < end)
            {
                foreach (Window window in windows)
                {
                    if (oldWindow.Id != window.Id)
                    {
                        if (!((start < window.Start && end <= window.Start) ||
                              (start >= window.End && end > window.End)))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        return false;
    }


    static async Task<List<Window>> GetWeekWindows(long idQueue)
    {
        await using ContextDataBase db = new();
        return await db.Windows.Where(p => p.QueueId == idQueue).ToListAsync();
    }

    public static async Task<bool> AddWindow(long queueId, long weekDayId, DateTime start, DateTime end)
    {
        await using ContextDataBase db = new();
        List<Window> windows = await db.Windows
            .Where(p => p.QueueId == queueId && p.WeekDayId == weekDayId && p.Start == start && p.End == end)
            .ToListAsync();
        if (windows.Count == 0)
        {
            Window window = new() { QueueId = queueId, WeekDayId = weekDayId, Start = start, End = end };
            await db.Windows.AddAsync(window);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async Task<Window?> GetWindow(long idWindow)
    {
        await using ContextDataBase db = new();
        return await db.Windows.Where(p => p.Id == idWindow).FirstOrDefaultAsync();
    }

    public static async Task<List<Window>> GetWindows(long idQueue, long idWeekDay)
    {
        await using ContextDataBase db = new();
        return await db.Windows.Where(p => p.QueueId == idQueue && p.WeekDayId == idWeekDay).ToListAsync();
    }

    public static async Task<bool> DeleteWindow(long id)
    {
        await using ContextDataBase db = new();
        Window? window = await db.Windows.Where(p => p.Id == id).FirstOrDefaultAsync();
        if (window is not null)
        {
            db.Windows.Attach(window);
            db.Windows.Remove(window);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public static async void AutoCommittedDayOfWeek()
    {
        await AddWeekDay("Понедельник");
        await AddWeekDay("Вторник");
        await AddWeekDay("Среда");
        await AddWeekDay("Четверг");
        await AddWeekDay("Пятница");
        await AddWeekDay("Суббота");
        await AddWeekDay("Воскресенье");
    }

    static async Task<bool> AddWeekDay(string name)
    {
        await using ContextDataBase db = new();
        List<WeekDay> weekDays = await db.WeekDays.Where(p => p.Name == name).ToListAsync();
        if (weekDays.Count == 0)
        {
            WeekDay weekDay = new() { Name = name };
            await db.WeekDays.AddAsync(weekDay);
            await db.SaveChangesAsync();
            return true;
        }

        return false;
    }


    public static async Task<WeekDay?> GetWeek(long id)
    {
        await using ContextDataBase db = new();
        return await db.WeekDays.Where(p => p.Id == id).FirstOrDefaultAsync();
    }
}